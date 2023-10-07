using UnityEngine;

namespace RaphaelHerve.Tatedrez.DesignPatterns
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                // Find instance of singleton, and create one if it doesn't exist
                if ((_instance = FindObjectOfType<T>()) == null)
                {
                    GameObject singletonGameObject = new(typeof(T).Name);
                    _instance = singletonGameObject.AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            // Destroy this GameObject if different from the Instance
            if (Instance != this)
            {
                Destroy(this);
                return;
            }
        }
    }
}