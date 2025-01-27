using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.Common
{
    public class Pool<T> where T : Object
    {
        private readonly Stack<T> _pooledObjects;

        private readonly T _prefab;

        private readonly Transform _container;


        public Pool(T prefab, Transform parent = null, int size = 1)
        {
            _pooledObjects = new Stack<T>();
            _prefab = prefab;
            _container = parent;

            for (var i = 0; i < size; i++)
                _pooledObjects.Push(CreateNewObject());
        }

        public T Instantiate()
        {
            var o = _pooledObjects.Count == 0 
                ? CreateNewObject() 
                : _pooledObjects.Pop();

            var go = o as GameObject;
            if (go != null)
                go.gameObject.SetActive(true);
            else
            {
                var monoBehaviour = o as MonoBehaviour;
                if (monoBehaviour != null)
                    monoBehaviour.gameObject.SetActive(true);
            }

            return o;
        }

        public void Destroy(T o)
        {
            var go = o as GameObject;
            if (go != null)
                go.gameObject.SetActive(false);
            else
            {
                var monoBehaviour = o as MonoBehaviour;
                if (monoBehaviour != null)
                {
                    monoBehaviour.gameObject.SetActive(false);
                    monoBehaviour.transform.localPosition = Vector3.zero;
                }
            }

            _pooledObjects.Push(o);
        }


        private T CreateNewObject()
        {
            if (_prefab == null)
                return default(T);

            var o = Object.Instantiate(_prefab, _container, false);
            o.name = _prefab.name;

            var go = o as GameObject;
            if (go != null)
            {
                go.SetActive(false);
                go.transform.localPosition = Vector3.zero;
            }

            return o;
        }
    }
}