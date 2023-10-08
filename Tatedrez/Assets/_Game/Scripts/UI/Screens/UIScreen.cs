using DG.Tweening;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.UI
{
    public class UIScreen : MonoBehaviour
    {
        public virtual void Init() { }

        public virtual void OnDestroy() { }

        public virtual void Reset() { }

        public virtual void Show(float duration)
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide(float duration)
        {
            DOVirtual.DelayedCall(duration, () => gameObject.SetActive(false));
        }
    }
}