using RaphaelHerve.Tatedrez.Enums;
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
        private Transform _playerAPawnsLocator;
        [SerializeField]
        private Transform _playerBPawnsLocator;

        private Transform _tilesParent;
        private Transform _pawnsParent;

        private Tile[,] _tiles = new Tile[COLUMN_COUNT, ROW_COUNT];
        private Dictionary<PlayerType, List<Pawn>> _pawnsByPlayer = new()
        {
            { PlayerType.PlayerA, new List<Pawn>() },
            { PlayerType.PlayerB, new List<Pawn>() },
        };

        public const int COLUMN_COUNT = 3;
        public const int ROW_COUNT = 3;

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
                }
            }
        }

        private void CreatePawns()
        {
            _pawnsParent = new GameObject("PawnsParent").transform;
            _pawnsParent.parent = transform;

            CreatePawnsForPlayer(PlayerType.PlayerA, _playerAPawnsLocator.position, Quaternion.LookRotation(Vector3.forward), _pawnsParent);
            CreatePawnsForPlayer(PlayerType.PlayerB, _playerBPawnsLocator.position, Quaternion.LookRotation(Vector3.back), _pawnsParent);
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

        public void PlacePawnOnTile(Pawn pawn, Tile tile)
        {

        }
    }
}