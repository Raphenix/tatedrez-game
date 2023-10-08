using RaphaelHerve.Tatedrez.DesignPatterns;
using RaphaelHerve.Tatedrez.Enums;
using RaphaelHerve.Tatedrez.Game;

namespace RaphaelHerve.Tatedrez.UI
{
    public partial class UIManager : Singleton<UIManager>
    {
        protected override void Awake()
        {
            base.Awake();

            HomeScreen.Init();
            GameScreen.Init();
            GameOverScreen.Init();

            Reset();

            GameManager.OnGameReset += Reset;
            GameManager.OnGameStateChanged += GameStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.OnGameReset -= Reset;
            GameManager.OnGameStateChanged -= GameStateChanged;
        }

        public void Reset()
        {
            HomeScreen.Show();
            GameScreen.Hide();
            GameOverScreen.Hide();
        }

        private void GameStateChanged(GameState from, GameState to)
        {
            switch (to)
            {
                case GameState.PiecePlacement:
                    HomeScreen.Hide();
                    GameScreen.Show();
                    break;
                case GameState.GameOver:
                    GameScreen.Hide();
                    GameOverScreen.Show();
                    break;
            }
        }
    }
}