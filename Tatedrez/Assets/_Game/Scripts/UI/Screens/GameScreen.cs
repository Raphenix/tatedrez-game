using DG.Tweening;
using RaphaelHerve.Tatedrez.Enums;
using RaphaelHerve.Tatedrez.Game;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.UI
{
    public class GameScreen : UIScreenWithSubscreens
    {
        public override void Init()
        {
            base.Init();

            GameManager.OnCurrentPlayerChanged += ToggleScreens;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            GameManager.OnCurrentPlayerChanged -= ToggleScreens;
        }

        private void ToggleScreens(PlayerType from, PlayerType to)
        {
            switch (to)
            {
                case PlayerType.Player1:
                    MoveSubscreen(_player2Subscreen, _player2SubscreenHiddenPosition, .5f);
                    MoveSubscreen(_player1Subscreen, _player1SubscreenShownPosition, .5f);
                    break;
                case PlayerType.Player2:
                    MoveSubscreen(_player1Subscreen, _player1SubscreenHiddenPosition, .5f);
                    MoveSubscreen(_player2Subscreen, _player2SubscreenShownPosition, .5f);
                    break;
            }
        }
    }

    public partial class UIManager
    {
        [SerializeField]
        private GameScreen _gameScreen;
        public GameScreen GameScreen => _gameScreen;
    }
}