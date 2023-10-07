using NaughtyAttributes;
using RaphaelHerve.Tatedrez.DesignPatterns;
using RaphaelHerve.Tatedrez.Enums;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.Game
{
    [RequireComponent(typeof(InputController))]
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private Board _boardPrefab;

        [ShowNonSerializedField]
        private PlayerType _currentPlayer;

        public static InputController InputController { get; private set; }
        public static Board Board { get; private set; }

        public static PlayerType CurrentPlayer
        {
            get => Instance._currentPlayer;
            set => Instance._currentPlayer = value;
        }

        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 60;

            InputController = GetComponent<InputController>();
            Board = Instantiate(_boardPrefab, transform);
        }
    }
}