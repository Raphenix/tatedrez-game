using RaphaelHerve.Tatedrez.Enums;
using RaphaelHerve.Tatedrez.Game;
using TMPro;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.UI
{
    public class GameOverScreen : UIScreen
    {
        [SerializeField]
        public TextMeshProUGUI _winnerText;

        public override void Init()
        {
            GameManager.OnGameStateChanged += GameStateChanged;
        }

        private void OnDestroy()
        {
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