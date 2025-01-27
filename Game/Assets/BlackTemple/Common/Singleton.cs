using UnityEngine;

namespace BlackTemple.Common
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        private static bool _isApplicationQuiting;

        public static T Instance
        {
            get
            {
                if (_isApplicationQuiting)
                    return null;

                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();

                if (_instance != null)
                    return _instance;

                var obj = new GameObject { name = typeof(T).Name };

                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        public virtual void Touch() { }

        protected virtual void Awake()
        {
            if (_instance == null || _instance == this)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isApplicationQuiting = true;
        }
    }
}