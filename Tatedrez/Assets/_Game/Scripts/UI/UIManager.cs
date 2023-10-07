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

            GameManager.OnGameStateChanged += GameStateChanged;

            HomeScreen.Init();
            GameScreen.Init();
            GameOverScreen.Init();

            HomeScreen.Show();
            GameScreen.Hide();
            GameOverScreen.Hide();
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= GameStateChanged;
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