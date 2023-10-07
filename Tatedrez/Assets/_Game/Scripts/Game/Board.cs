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
        private Pawn[] _pawnsPrefabs;

        [Header("Start locators")]
        [SerializeField]
        private Transform _player1PawnsLocator;
        [SerializeField]
        private Transform _player2PawnsLocator;

        private Transform _tilesParent;
        private Transform _pawnsParent;

        private Tile[,] _tiles = new Tile[COLUMN_COUNT, ROW_COUNT];
        private List<Tile> _highlightedTiles = new();
        private Dictionary<PlayerType, List<Pawn>> _pawnsByPlayer = new()
        {
            { PlayerType.Player1, new List<Pawn>() },
            { PlayerType.Player2, new List<Pawn>() },
        };

        public const int COLUMN_COUNT = 3;
        public const int ROW_COUNT = 3;

        public event Action<Pawn> OnPawnPlacedOnTile;

        private void Awake()
        {
            CreateTiles();
            CreatePawns();
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

        private void CreatePawns()
        {
            _pawnsParent = new GameObject("PawnsParent").transform;
            _pawnsParent.parent = transform;

            CreatePawnsForPlayer(PlayerType.Player1, _player1PawnsLocator.position, _pawnsParent);
            CreatePawnsForPlayer(PlayerType.Player2, _player2PawnsLocator.position, _pawnsParent);
        }

        private void CreatePawnsForPlayer(PlayerType playerType, Vector3 locatorPosition, Transform parent)
        {
            for (int i = 0; i < _pawnsPrefabs.Length; i++)
            {
                Pawn pawn = Instantiate(_pawnsPrefabs[i], locatorPosition + playerType.Rotation() * Vector3.right * (i - (COLUMN_COUNT - 1) * .5f), Quaternion.identity, parent);
                pawn.Init(playerType);
                _pawnsByPlayer[playerType].Add(pawn);
            }
        }

        private Vector3 GetTilePosition(int x, int y) => new Vector3(x - (COLUMN_COUNT - 1) * .5f, 0f, y - (ROW_COUNT - 1) * .5f);

        public bool CanPlacePawnOnTile(Pawn pawn, Tile tile)
        {
            // Tile already has a pawn on
            if (tile.HasPawnOn)
            {
                return false;
            }

            // First placement on board, no move check needed
            if (!pawn.IsPlacedOnTile)
            {
                return true;
            }

            // Allow placement if movement possible
            return GetMoves(pawn.PawnType, pawn.Coordinates).Contains(tile);
        }

        public List<Tile> GetMoves(PawnType pawnType, Vector2Int pawnCoordinates)
            => pawnType switch
            {
                PawnType.Knight => GetKnightMoves(pawnCoordinates),
                PawnType.Rook => GetRookMoves(pawnCoordinates),
                PawnType.Bishop => GetBishopMoves(pawnCoordinates),
                _ => null
            };

        public List<Tile> GetKnightMoves(Vector2Int pawnCoordinates)
        {
            List<Tile> moves = new();

            // Right, Up, Up
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(1, 2), 1));
            // Right, Right, Up
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(2, 1), 1));
            // Right, Right, Down
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(2, -1), 1));
            // Right, Down, Down
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(1, -2), 1));
            // Left, Down, Down
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(-1, -2), 1));
            // Left, Left, Down
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(-2, -1), 1));
            // Left, Left, Up
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(-2, 1), 1));
            // Left, Up, Up
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(-1, 2), 1));

            return moves;
        }

        public List<Tile> GetRookMoves(Vector2Int pawnCoordinates)
        {
            List<Tile> moves = new();

            // Left
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(-1, 0)));
            // Right
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(1, 0)));
            // Down
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(0, -1)));
            // Up
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(0, 1)));

            return moves;
        }

        public List<Tile> GetBishopMoves(Vector2Int pawnCoordinates)
        {
            List<Tile> moves = new();

            // Right & Up
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(1, 1)));
            // Right & Down
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(1, -1)));
            // Left & Down
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(-1, -1)));
            // Left & Up
            moves.AddRange(GetTilesInDirection(pawnCoordinates, new Vector2Int(-1, 1)));

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
            return !tile.HasPawnOn;
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

        public bool TryPlacePawnOnTile(Pawn pawn, Tile tile)
        {
            // Tile already has a pawn on
            if (tile.HasPawnOn)
            {
                return false;
            }

            // Making an invalid move
            if (GameManager.GameState == GameState.Dynamic && !GetMoves(pawn.PawnType, pawn.Coordinates).Contains(tile))
            {
                return false;
            }

            if (pawn.IsPlacedOnTile)
            {
                pawn.CurrentTile.RemovePawn(pawn);
            }

            tile.PlacePawnOn(pawn);
            pawn.PlaceOnTile(tile);

            OnPawnPlacedOnTile?.Invoke(pawn);
            return true;
        }

        public bool HasPlayerFormedATicTacToe(PlayerType playerType)
        {
            List<Pawn> playerPawns = _pawnsByPlayer[playerType];
            Vector2? direction = null;

            for (int i = 0; i < playerPawns.Count; i++)
            {
                // A pawn is not placed on a tile, so not possible to form a TicTacToe
                if (!playerPawns[i].IsPlacedOnTile)
                {
                    return false;
                }

                // Direction checking can only be done from second iteration
                if (i == 0)
                {
                    continue;
                }

                Vector2 tmpDirection = (Vector2)(playerPawns[0].Coordinates - playerPawns[i].Coordinates);
                tmpDirection.Normalize();

                // First direction calculated, store it for next iterations
                if (!direction.HasValue)
                {
                    direction = tmpDirection;
                    continue;
                }

                float cross = direction.Value.x * tmpDirection.y - direction.Value.y * tmpDirection.x;

                // The resulting vectors are not aligned, meaning pawns are not aligned either
                if (cross != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool AreAllPawnsPlaced()
        {
            foreach (List<Pawn> pawnList in _pawnsByPlayer.Values)
            {
                foreach (Pawn pawn in pawnList)
                {
                    if (!pawn.IsPlacedOnTile)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanPlayerPlay(PlayerType playerType)
        {
            foreach (Pawn pawn in _pawnsByPlayer[playerType])
            {
                if (GetMoves(pawn.PawnType, pawn.Coordinates).Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}