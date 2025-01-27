using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;
// ReSharper disable All

namespace BlackTemple.EpicMine
{
    public class PlayerPrefsHelper
    {
        public static void Save (PlayerPrefsType type, object val)
        {
            PlayerPrefs.SetString(type.ToString(), val.ToJson());
            PlayerPrefs.Save();
        }

        public static void Remove(PlayerPrefsType type)
        {
            var key = type.ToString();

            if (!PlayerPrefs.HasKey(key))
                return;

            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        public static T Load<T>(PlayerPrefsType type)
        {
            var json = PlayerPrefs.GetString(type.ToString(), "");

            if (!string.IsNullOrEmpty(json))
                return json.FromJson<T>();

            return default(T);
        }

        public static T LoadDefault<T>(PlayerPrefsType type, T def)
        {
            var json = PlayerPrefs.GetString(type.ToString(), "");

            if (!string.IsNullOrEmpty(json))
                return json.FromJson<T>();

            return def;
        }

        public static bool IsExist(PlayerPrefsType type)
        {
            return PlayerPrefs.HasKey(type.ToString());
        }
    }
}
