using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.Common
{
    public class SingletonPool : MonoBehaviour
    {
        public static SingletonPool Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SingletonPool>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("SingletonPool")
                            .AddComponent<SingletonPool>();
                    }
                }

                return _instance;
            }
        }

        private List<GameObject> _pool = new List<GameObject>();

        private static SingletonPool _instance;


        public void Clear()
        {
            foreach (var o in _pool)
            {
                if(o!=null)
                    Destroy(o.gameObject);
            }
            _pool.Clear();
        }

        public void ToPool(GameObject obj)
        {
            obj.transform.SetParent(transform);

            if (!_pool.Contains(obj))
            _pool.Add(obj);

            obj.SetActive(false);
        }

        public T FromPool<T>()
        {
            for (var i = 0; i < _pool.Count; i++)
            {
                if (_pool[i] == null)
                {
                    _pool.Remove(_pool[i]);
                    i--;
                }
            }

            var first = _pool.Find(x => !x.activeSelf);
            if (first != null)
            {
                first.SetActive(true);
                return first.GetComponent<T>();
            }
            return default(T);
        }
        
    }
}