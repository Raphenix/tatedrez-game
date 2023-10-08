using DG.Tweening;
using RaphaelHerve.Tatedrez.DesignPatterns;
using RaphaelHerve.Tatedrez.Enums;
using RaphaelHerve.Tatedrez.Game;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.UI
{
    public partial class UIManager : Singleton<UIManager>
    {
        [SerializeField]
        private float _screenTransitionDuration = .5f;

        protected override void Awake()
        {
            base.Awake();

            HomeScreen.Init();
            GameScreen.Init();
            GameOverScreen.Init();

            Reset();

            GameManager.OnGameStateChanged += GameStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= GameStateChanged;
        }

        public void Reset()
        {
            HomeScreen.Show(0f);
            GameScreen.Hide(0f);
            GameOverScreen.Hide(0f);
        }

        private void GameStateChanged(GameState from, GameState to)
        {
            switch (to)
            {
                case GameState.PiecePlacement:
                    HomeScreen.Hide(_screenTransitionDuration);
                    GameScreen.Show(_screenTransitionDuration);
                    break;
                case GameState.GameOver:
                    GameScreen.Hide(_screenTransitionDuration);
                    GameOverScreen.Show(_screenTransitionDuration);
                    break;
                case GameState.None:
                    GameOverScreen.Hide(_screenTransitionDuration);
                    DOVirtual.DelayedCall(_screenTransitionDuration,
                        () => HomeScreen.Show(_screenTransitionDuration));
                    break;
            }
        }
    }
}