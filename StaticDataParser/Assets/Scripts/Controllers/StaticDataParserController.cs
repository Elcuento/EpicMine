using System;
using System.IO;
using System.Linq;
using BlackTemple.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class StaticDataParserController : MonoBehaviour
    {
        public TextAsset Static;
        public TextAsset ExtractStatic;

        public void Start()
        {
             SerializeAndSave(Static.text);

             //ReadData();
           // Unzip();
        }

        public void Unzip()
        {
            //  
            var path = $"{Application.persistentDataPath}/99.json";
            var file = File.ReadAllBytes(path);
            var zippedData = Extensions.Unzip(file);
            var codeData = Extensions.Base64Decode(zippedData);
            File.WriteAllText(path, codeData);
        }


        public void SerializeAndSave(string str)
        {
            try
            {
                var path = $"{Application.persistentDataPath}/{Static.name}.json";
                var path2 = $"{Application.persistentDataPath}/{Static.name}.txt";
                var data = str.FromJson<StaticData>();

                var codeData = Extensions.Base64Encode(data.ToJson(Formatting.None));
                var zippedData = Extensions.Zip(codeData);

                File.WriteAllBytes(path, zippedData);
                File.WriteAllText(path2, data.ToJson(Formatting.None));
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

            Debug.Log("Parse ok");
           
        }
    }
}
