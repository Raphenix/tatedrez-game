using DG.Tweening;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.UI
{
    public class UIScreenWithSubscreens : UIScreen
    {
        [SerializeField]
        protected RectTransform _player1Subscreen;
        [SerializeField]
        protected float _player1SubscreenShownPosition;
        [SerializeField]
        protected float _player1SubscreenHiddenPosition;
        [SerializeField]
        protected RectTransform _player2Subscreen;
        [SerializeField]
        protected float _player2SubscreenShownPosition;
        [SerializeField]
        protected float _player2SubscreenHiddenPosition;

        public override void Hide(float duration)
        {
            base.Hide(duration);

            MoveSubscreen(_player1Subscreen, _player1SubscreenHiddenPosition, duration);
            MoveSubscreen(_player2Subscreen, _player2SubscreenHiddenPosition, duration);
        }

        public void MoveSubscreen(RectTransform subscreen, float endValue, float duration)
        {
            subscreen.DOAnchorPosY(endValue, duration);
        }
    }
}