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

            CreatePawnsForPlayer(PlayerType.Player1, _player1PawnsLocator.position, Quaternion.LookRotation(Vector3.forward), _pawnsParent);
            CreatePawnsForPlayer(PlayerType.Player2, _player2PawnsLocator.position, Quaternion.LookRotation(Vector3.back), _pawnsParent);
        }

        private void CreatePawnsForPlayer(PlayerType playerType, Vector3 locatorPosition, Quaternion rotation, Transform parent)
        {
            for (int i = 0; i < _pawnsPrefabs.Length; i++)
            {
                Pawn pawn = Instantiate(_pawnsPrefabs[i], locatorPosition + rotation * Vector3.right * (i - (COLUMN_COUNT - 1) * .5f), rotation, parent);
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

            return IsMoveAuthorized(pawn.PawnType, pawn.CurrentTile, tile);
        }

        // TODO implement movement checking
        public bool IsMoveAuthorized(PawnType pawnType, Tile from, Tile to)
        {
            return true;
        }

        public void PlacePawnOnTile(Pawn pawn, Tile tile)
        {
            if (pawn.IsPlacedOnTile)
            {
                pawn.CurrentTile.RemovePawn(pawn);
            }

            tile.PlacePawnOn(pawn);
            pawn.PlaceOnTile(tile);

            OnPawnPlacedOnTile?.Invoke(pawn);
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

                Vector2 tmpDirection = playerPawns[0].Coordinates - playerPawns[i].Coordinates;
                tmpDirection.Normalize();

                // First direction calculated, store it for next iterations
                if (!direction.HasValue)
                {
                    direction = tmpDirection;
                    continue;
                }

                float dot = Vector2.Dot(direction.Value, tmpDirection);

                // The resulting vectors are not aligned, meaning pawns are not aligned either
                if (dot != 1 && dot != -1)
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

        // TODO check if player can make a move with any of its pawns
        public bool CanPlayerPlay(PlayerType playerType)
        {
            return true;
        }
    }
}