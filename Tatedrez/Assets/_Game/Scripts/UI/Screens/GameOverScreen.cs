using RaphaelHerve.Tatedrez.Enums;
using RaphaelHerve.Tatedrez.Game;
using UnityEngine;
using UnityEngine.UI;

namespace RaphaelHerve.Tatedrez.UI
{
    public class GameOverScreen : UIScreenWithSubscreens
    {
        [SerializeField]
        private Button[] _replayButtons;

        public override void Init()
        {
            base.Init();

            foreach (Button replayButton in _replayButtons)
            {
                replayButton.onClick.AddListener(GameManager.Instance.ReplayGame);
            }

            GameManager.OnGameStateChanged += GameStateChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            foreach (Button replayButton in _replayButtons)
            {
                replayButton.onClick.RemoveAllListeners();
            }

            GameManager.OnGameStateChanged -= GameStateChanged;
        }

        private void GameStateChanged(GameState from, GameState to)
        {
            if (to != GameState.GameOver)
            {
                return;
            }

            switch (GameManager.CurrentPlayer)
            {
                case PlayerType.Player1:
                    MoveSubscreen(_player1Subscreen, _player1SubscreenShownPosition, .5f);
                    break;
                case PlayerType.Player2:
                    MoveSubscreen(_player2Subscreen, _player2SubscreenShownPosition, .5f);
                    break;
            }
        }
    }

    public partial class UIManager
    {
        [SerializeField]
        GameOverScreen _gameOverScreen;
        public GameOverScreen GameOverScreen => _gameOverScreen;
    }
}