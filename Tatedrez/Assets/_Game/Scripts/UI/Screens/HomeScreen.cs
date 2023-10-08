using DG.Tweening;
using RaphaelHerve.Tatedrez.Game;
using UnityEngine;
using UnityEngine.UI;

namespace RaphaelHerve.Tatedrez.UI
{
    public class HomeScreen : UIScreen
    {
        [SerializeField]
        private RectTransform _top;
        [SerializeField]
        private float _topShownPosition;
        [SerializeField]
        private float _topHiddenPosition;
        [SerializeField]
        private RectTransform _bottom;
        [SerializeField]
        private float _bottomShownPosition;
        [SerializeField]
        private float _bottomHiddenPosition;
        [SerializeField]
        private Button _startButton;

        public override void Init()
        {
            _startButton.onClick.AddListener(GameManager.Instance.StartGame);
        }

        public override void OnDestroy()
        {
            _startButton.onClick.RemoveAllListeners();
        }

        public override void Show(float duration)
        {
            base.Show(duration);

            _top.DOAnchorPosY(_topShownPosition, duration);
            _bottom.DOAnchorPosY(_bottomShownPosition, duration);
        }

        public override void Hide(float duration)
        {
            base.Hide(duration);

            _top.DOAnchorPosY(_topHiddenPosition, duration);
            _bottom.DOAnchorPosY(_bottomHiddenPosition, duration);
        }
    }

    public partial class UIManager
    {
        [SerializeField]
        private HomeScreen _homeScreen;
        public HomeScreen HomeScreen => _homeScreen;
    }
}