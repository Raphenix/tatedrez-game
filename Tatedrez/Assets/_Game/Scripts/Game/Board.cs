using RaphaelHerve.Tatedrez.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.Game
{
    public class Board : MonoBehaviour
    {
        [SerializeField]
        private Tile _tilePrefab;

        [SerializeField]
        private Piece[] _piecesPrefabs;

        [Header("Start locators")]
        [SerializeField]
        private Transform _player1PiecesLocator;
        [SerializeField]
        private Transform _player2PiecesLocator;

        private Transform _tilesParent;
        private Transform _piecesParent;

        private Tile[,] _tiles = new Tile[COLUMN_COUNT, ROW_COUNT];
        private List<Tile> _highlightedTiles = new();
        private Dictionary<PlayerType, List<Piece>> _piecesByPlayer = new()
        {
            { PlayerType.Player1, new List<Piece>() },
            { PlayerType.Player2, new List<Piece>() },
        };

        public const int COLUMN_COUNT = 3;
        public const int ROW_COUNT = 3;

        public event Action<Piece> OnPiecePlacedOnTile;

        private void Awake()
        {
            CreateTiles();
            CreatePieces();
        }

        private void CreateTiles()
        {
            _tilesParent = new GameObject("TilesParent").transform;
            _tilesParent.parent = transform;

            for (int y = 0; y < ROW_COUNT; y++)
            {
                for (int x = 0; x < COLUMN_COUNT; x++)
                {
                    _tiles[x, y] = Instantiate(_tilePrefab,
                                               GetTilePosition(x, y),
                                               Quaternion.identity,
                                               _tilesParent);
                    _tiles[x, y].Init(x, y);
                }
            }
        }

        private Vector3 GetTilePosition(int x, int y) => new Vector3(x - (COLUMN_COUNT - 1) * .5f, 0f, y - (ROW_COUNT - 1) * .5f);

        private void CreatePieces()
        {
            _piecesParent = new GameObject("PiecesParent").transform;
            _piecesParent.parent = transform;

            CreatePiecesForPlayer(PlayerType.Player1, _player1PiecesLocator.position, _piecesParent);
            CreatePiecesForPlayer(PlayerType.Player2, _player2PiecesLocator.position, _piecesParent);
        }

        private void CreatePiecesForPlayer(PlayerType playerType, Vector3 locatorPosition, Transform parent)
        {
            for (int i = 0; i < _piecesPrefabs.Length; i++)
            {
                Piece piece = Instantiate(_piecesPrefabs[i], locatorPosition + playerType.Rotation() * Vector3.right * (i - (COLUMN_COUNT - 1) * .5f), Quaternion.identity, parent);
                piece.Init(playerType);
                _piecesByPlayer[playerType].Add(piece);
            }
        }

        public void Reset()
        {
            foreach (List<Piece> pieces in _piecesByPlayer.Values)
            {
                foreach (Piece piece in pieces)
                {
                    if (!piece.IsPlacedOnTile)
                    {
                        continue;
                    }

                    piece.CurrentTile.RemovePiece(piece);
                    piece.Reset();
                }
            }
        }

        public bool CanPlacePieceOnTile(Piece piece, Tile tile)
        {
            // Tile already has a piece on
            if (tile.HasPieceOn)
            {
                return false;
            }

            // First placement on board, no move check needed
            if (!piece.IsPlacedOnTile)
            {
                return true;
            }

            // Allow placement if movement possible
            return GetMoves(piece.PieceType, piece.Coordinates).Contains(tile);
        }

        public List<Tile> GetMoves(PieceType pieceType, Vector2Int pieceCoordinates)
            => pieceType switch
            {
                PieceType.Knight => GetKnightMoves(pieceCoordinates),
                PieceType.Rook => GetRookMoves(pieceCoordinates),
                PieceType.Bishop => GetBishopMoves(pieceCoordinates),
                _ => null
            };

        public List<Tile> GetKnightMoves(Vector2Int pieceCoordinates)
        {
            List<Tile> moves = new();

            // Right, Up, Up
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(1, 2), 1));
            // Right, Right, Up
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(2, 1), 1));
            // Right, Right, Down
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(2, -1), 1));
            // Right, Down, Down
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(1, -2), 1));
            // Left, Down, Down
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(-1, -2), 1));
            // Left, Left, Down
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(-2, -1), 1));
            // Left, Left, Up
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(-2, 1), 1));
            // Left, Up, Up
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(-1, 2), 1));

            return moves;
        }

        public List<Tile> GetRookMoves(Vector2Int pieceCoordinates)
        {
            List<Tile> moves = new();

            // Left
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(-1, 0)));
            // Right
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(1, 0)));
            // Down
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(0, -1)));
            // Up
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(0, 1)));

            return moves;
        }

        public List<Tile> GetBishopMoves(Vector2Int pieceCoordinates)
        {
            List<Tile> moves = new();

            // Right & Up
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(1, 1)));
            // Right & Down
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(1, -1)));
            // Left & Down
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(-1, -1)));
            // Left & Up
            moves.AddRange(GetTilesInDirection(pieceCoordinates, new Vector2Int(-1, 1)));

            return moves;
        }

        public List<Tile> GetTilesInDirection(Vector2Int coordinates, Vector2Int direction, int maxCheckCount = int.MaxValue)
        {
            List<Tile> tiles = new();

            if (direction == Vector2Int.zero)
            {
                Debug.LogError($"Direction can't be {Vector2Int.zero}");
                return tiles;
            }

            int checkCount = 0;

            while (checkCount < maxCheckCount)
            {
                coordinates += direction;

                if (!TryGetFreeTileAtCoordinates(coordinates, out Tile tile))
                {
                    break;
                }

                tiles.Add(tile);

                checkCount++;
            }

            return tiles;
        }

        public bool TryGetFreeTileAtCoordinates(Vector2Int coordinates, out Tile tile)
        {
            // Out of board
            if (coordinates.x is < 0 or >= COLUMN_COUNT || coordinates.y is < 0 or >= ROW_COUNT)
            {
                tile = null;
                return false;
            }

            tile = _tiles[coordinates.x, coordinates.y];
            return !tile.HasPieceOn;
        }

        public void ShowTilesHighlight(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                tile.ShowHighlight();
            }

            _highlightedTiles.AddRange(tiles);
        }

        public void HideTilesHighlights()
        {
            foreach (Tile tile in _highlightedTiles)
            {
                tile.HideHighlight();
            }

            _highlightedTiles.Clear();
        }

        public bool TryPlacePieceOnTile(Piece piece, Tile tile)
        {
            // Tile already has a piece on
            if (tile.HasPieceOn)
            {
                return false;
            }

            // Making an invalid move
            if (GameManager.GameState == GameState.Dynamic && !GetMoves(piece.PieceType, piece.Coordinates).Contains(tile))
            {
                return false;
            }

            if (piece.IsPlacedOnTile)
            {
                piece.CurrentTile.RemovePiece(piece);
            }

            tile.PlacePieceOn(piece);
            piece.PlaceOnTile(tile);

            OnPiecePlacedOnTile?.Invoke(piece);
            return true;
        }

        public bool HasPlayerFormedATicTacToe(PlayerType playerType)
        {
            List<Piece> playerPieces = _piecesByPlayer[playerType];
            Vector2? direction = null;

            for (int i = 0; i < playerPieces.Count; i++)
            {
                // A piece is not placed on a tile, so not possible to form a TicTacToe
                if (!playerPieces[i].IsPlacedOnTile)
                {
                    return false;
                }

                // Direction checking can only be done from second iteration
                if (i == 0)
                {
                    continue;
                }

                Vector2 tmpDirection = (Vector2)(playerPieces[0].Coordinates - playerPieces[i].Coordinates);
                tmpDirection.Normalize();

                // First direction calculated, store it for next iterations
                if (!direction.HasValue)
                {
                    direction = tmpDirection;
                    continue;
                }

                float cross = direction.Value.x * tmpDirection.y - direction.Value.y * tmpDirection.x;

                // The resulting vectors are not aligned, meaning pieces are not aligned either
                if (cross != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool AreAllPiecesPlaced()
        {
            foreach (List<Piece> pieceList in _piecesByPlayer.Values)
            {
                foreach (Piece piece in pieceList)
                {
                    if (!piece.IsPlacedOnTile)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanPlayerPlay(PlayerType playerType)
        {
            foreach (Piece piece in _piecesByPlayer[playerType])
            {
                if (GetMoves(piece.PieceType, piece.Coordinates).Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}