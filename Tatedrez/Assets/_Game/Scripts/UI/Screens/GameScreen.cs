using RaphaelHerve.Tatedrez.Enums;
using RaphaelHerve.Tatedrez.Game;
using TMPro;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.UI
{
    public class GameScreen : UIScreen
    {
        [SerializeField]
        private TextMeshProUGUI _playerTurnText;

        public override void Init()
        {
            GameManager.OnCurrentPlayerChanged += CurrentPlayerChanged;
        }

        private void OnDestroy()
        {
            GameManager.OnCurrentPlayerChanged -= CurrentPlayerChanged;
        }

        private void CurrentPlayerChanged(PlayerType from, PlayerType to)
        {
            _playerTurnText.text = $"{to.Name()} turn";
        }
    }

    public partial class UIManager
    {
        [SerializeField]
        private GameScreen _gameScreen;
        public GameScreen GameScreen => _gameScreen;
    }
}