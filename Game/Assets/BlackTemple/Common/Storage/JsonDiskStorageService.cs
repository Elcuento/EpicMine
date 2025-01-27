using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace BlackTemple.Common
{
    public class JsonDiskStorageService : IStorageService
    {
        public const  string EncodeFilePrefix = ".emc";

        private readonly JsonSerializerSettings _serializerSettings;

        public JsonDiskStorageService()
        {
            _serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        }

        public void Save(string fileName, object data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented, _serializerSettings);
            var codeData = json;
            var path = Path.Combine(Application.persistentDataPath, fileName + EncodeFilePrefix);
            File.WriteAllText(path, codeData);
        }

        public T Load<T>(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName + EncodeFilePrefix);
            
            try
            {
                var codeData = File.ReadAllText(path);

                var json = codeData; //Extensions.Base64Decode(codeData);
                var data = JsonConvert.DeserializeObject<T>(json, _serializerSettings);
                return data;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public void Remove(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName + EncodeFilePrefix);
            if (File.Exists(path))
                File.Delete(path);
        }

        public void Clear() { }

        public bool IsDataExists(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName + EncodeFilePrefix);
            return File.Exists(path);
        }

        public T LoadDefault<T>(string key)
        {
            return default(T);
        }
    }
}