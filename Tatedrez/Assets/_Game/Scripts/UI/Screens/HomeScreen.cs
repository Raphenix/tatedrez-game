using RaphaelHerve.Tatedrez.Game;
using UnityEngine;
using UnityEngine.UI;

namespace RaphaelHerve.Tatedrez.UI
{
    public class HomeScreen : UIScreen
    {
        [SerializeField]
        private Button _startButton;

        public override void Init()
        {
            _startButton.onClick.AddListener(GameManager.Instance.StartGame);
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveAllListeners();
        }
    }

    public partial class UIManager
    {
        [SerializeField]
        private HomeScreen _homeScreen;
        public HomeScreen HomeScreen => _homeScreen;
    }
}