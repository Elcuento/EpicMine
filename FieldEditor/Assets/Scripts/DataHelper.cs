    using System.Collections.Generic;
    using System.IO;
    using BlackTemple.EpicMine.Core;
    using Newtonsoft.Json;
    using UnityEngine;

public static class DataHelper
{
    public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

    public static List<BlackTemple.EpicMine.Dto.Field> Load()
    {
        var path = Application.persistentDataPath + "/" + DefaultConstrains.DataName;
        if (File.Exists(path))
        {
            var data = File.ReadAllText(path);
            var fields = JsonConvert.DeserializeObject<List<BlackTemple.EpicMine.Dto.Field>>(data, JsonSettings);
            return fields;
        }

        return null;
    }


    public static void Save(List<BlackTemple.EpicMine.Core.Field> fields)
    {
        var dtoFields = new List<BlackTemple.EpicMine.Dto.Field>();
        foreach (var field1 in fields)
        {
            dtoFields.Add(field1.ExportData());
        }

        var path = Application.persistentDataPath + "/" + DefaultConstrains.DataName;
        Debug.Log("Save " + path);
        var data = JsonConvert.SerializeObject(dtoFields, JsonSettings);
        File.WriteAllText(path, data);

    }

}

