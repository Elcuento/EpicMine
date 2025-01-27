using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace AMTServer.Common
{
    public static class Utils
    {

        public static T RandomElement<T>(this IList<T> array)
        {
            return array[ new Random().Next(0, array.Count)];
        }


        public static void CreateDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static T FromJson<T>(this string str)
        {
            //return fastJSON.JSON.ToObject<T>(str);
            //  return (T)MiniJSON.Json.Deserialize(str);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject<T>(str, settings);
        }

        public static string GetTime(long timeLeft)
        {
            if (timeLeft <= 0)
                return "";

            var date = new DateTime();
            date = date.AddSeconds(timeLeft);

            if (date.Day > 1)
            {
                var day = "day";
                var hour = "hour";
                var minute = "minute";
                return string.Format($"{date.Day - 1} {day} {date.Hour} {hour} {date.Minute} {minute}");
            }
            else
            {
                var hour = "hour";
                var minute = "minute";
                var sec = "sec";
                return string.Format($"{date.Hour} {hour} {date.Minute} {minute} {date.Second} {sec}");
            }
        }

        public static string ToJson<T>(this T obj, Formatting formatting = Formatting.None)
        {
            //return fastJSON.JSON.ToJSON(obj);
            // return MiniJSON.Json.Serialize(obj);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }


        public static long GetUnixTime()
        {
            return new DateTimeOffset(DateTime.Now.ToUniversalTime()).ToUnixTimeSeconds();
        }

        public static long GetUnixTime(DateTime time)
        {
            return new DateTimeOffset(time).ToUnixTimeSeconds();
        }

        public static DateTime FromUnix(long time)
        {
            return DateTimeOffset.FromUnixTimeSeconds(time).UtcDateTime;
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}
