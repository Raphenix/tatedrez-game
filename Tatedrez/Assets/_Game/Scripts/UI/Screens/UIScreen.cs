using UnityEngine;

namespace RaphaelHerve.Tatedrez.UI
{
    public class UIScreen : MonoBehaviour
    {
        public virtual void Init() { }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}