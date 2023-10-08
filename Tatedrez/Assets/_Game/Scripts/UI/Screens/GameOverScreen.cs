using RaphaelHerve.Tatedrez.Enums;
using RaphaelHerve.Tatedrez.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaphaelHerve.Tatedrez.UI
{
    public class GameOverScreen : UIScreen
    {
        [SerializeField]
        private TextMeshProUGUI _winnerText;
        [SerializeField]
        private Button _replayButton;

        public override void Init()
        {
            _replayButton.onClick.AddListener(GameManager.Instance.ReplayGame);
            GameManager.OnGameStateChanged += GameStateChanged;
        }

        private void OnDestroy()
        {
            _replayButton.onClick.RemoveAllListeners();
            GameManager.OnGameStateChanged -= GameStateChanged;
        }

        private void GameStateChanged(GameState from, GameState to)
        {
            if (to == GameState.GameOver)
            {
                _winnerText.text = $"{GameManager.CurrentPlayer.Name()} won!";
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