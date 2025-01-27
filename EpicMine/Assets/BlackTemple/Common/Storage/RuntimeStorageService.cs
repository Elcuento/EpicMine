using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.Common
{
    public class RuntimeStorageService : IStorageService
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public void Save(string key, object data)
        {
            _data[key] = data;
        }

        public T Load<T>(string key)
        {
            object data;

            if (_data.TryGetValue(key, out data))
            {
                return (T)data;
            }

            return default(T);
        }

        public void Remove(string key)
        {
            _data.Remove(key);
        }

        public void ConvertToEncode(string key)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool IsDataExists(string key)
        {
            return _data.ContainsKey(key);
        }

        public T LoadDefault<T>(string key)
        {
            return IsDataExists(key) ? Load<T>(key) : default(T);
        }
    }
}