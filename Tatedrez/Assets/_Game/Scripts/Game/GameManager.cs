using NaughtyAttributes;
using RaphaelHerve.Tatedrez.DesignPatterns;
using RaphaelHerve.Tatedrez.Enums;
using System;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.Game
{
    [RequireComponent(typeof(InputController))]
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private Board _boardPrefab;

        [ShowNonSerializedField]
        private GameState _gameState;
        [ShowNonSerializedField]
        private PlayerType _currentPlayer;

        public static InputController InputController { get; private set; }
        public static Board Board { get; private set; }

        public static PlayerType CurrentPlayer
        {
            get => Instance._currentPlayer;
            private set
            {
                // Can be called with same player if other player can't play
                PlayerType from = Instance._currentPlayer;
                Instance._currentPlayer = value;
                OnCurrentPlayerChanged?.Invoke(from, value);
            }
        }

        public static GameState GameState
        {
            get => Instance._gameState;
            private set
            {
                // Same state
                if (value == Instance._gameState)
                {
                    return;
                }

                GameState from = Instance._gameState;
                Instance._gameState = value;
                OnGameStateChanged?.Invoke(from, value);
            }
        }

        public delegate void CurrentPlayerChangedHandler(PlayerType from, PlayerType to);
        public static event CurrentPlayerChangedHandler OnCurrentPlayerChanged;

        public delegate void GameStateChangedHandler(GameState from, GameState to);
        public static event GameStateChangedHandler OnGameStateChanged;

        public static event Action OnGameReset;

        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 60;

            InputController = GetComponent<InputController>();
            Board = Instantiate(_boardPrefab, transform);
            Board.OnPiecePlacedOnTile += CheckEndOfTurn;
        }

        private void OnDestroy()
        {
            if (Board != null)
            {
                Board.OnPiecePlacedOnTile -= CheckEndOfTurn;
            }
        }

        public void StartGame()
        {
            GameState = GameState.PiecePlacement;
            CurrentPlayer = UnityEngine.Random.value < .5f ? PlayerType.Player1 : PlayerType.Player2;
        }

        private void CheckEndOfTurn(Piece piece)
        {
            if (HasPlayerWon())
            {
                GameState = GameState.GameOver;
                return;
            }

            if (GameState == GameState.PiecePlacement && AreAllPiecesPlaced())
            {
                GameState = GameState.Dynamic;
            }

            CurrentPlayer = GameState switch
            {
                // Current player plays again if other can't play
                GameState.Dynamic when !CanOtherPlayerPlay() => CurrentPlayer,
                // Else change player
                _ => CurrentPlayer.OtherPlayer()
            };
        }

        public bool HasPlayerWon() => Board.HasPlayerFormedATicTacToe(CurrentPlayer);

        public bool AreAllPiecesPlaced() => Board.AreAllPiecesPlaced();

        public bool CanOtherPlayerPlay() => Board.CanPlayerPlay(CurrentPlayer.OtherPlayer());

        public void ReplayGame()
        {
            Board.Reset();
            CurrentPlayer = PlayerType.None;
            GameState = GameState.None;
            OnGameReset?.Invoke();
        }
    }
}