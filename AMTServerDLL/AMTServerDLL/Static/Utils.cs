using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;

namespace AMTServerDLL
{
    public static class Utils
    {
        public enum LogType
        {
            Important,
            Errors,
            Rest,
            All,
            Deep,
        }

        public static string Log(LogType log, string message, LogType type = LogType.Rest)
        {
            switch (log)
            {
                case LogType.Important:
                    if (type == LogType.Errors || type == LogType.Important)
                        return message;
                    break;
                case LogType.Errors:
                    if (type == LogType.Errors)
                        return message;
                    break;
                case LogType.All:
                    return message;
                case LogType.Rest:
                    if (type == LogType.Rest)
                        return message;
                    break;
                case LogType.Deep:
                    if (type == LogType.Deep)
                        return message;
                    break;
            }

            return "";
        }

        public static long GetUnixTime()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public static long GetUnixTime(DateTime time)
        {
            return new DateTimeOffset(time).ToUnixTimeSeconds();
        }

        public static DateTime FromUnix(long time)
        {
            return DateTimeOffset.FromUnixTimeSeconds(time).UtcDateTime;
        }


        public static long GetMidnightTimeLeft()
        {
            var ts = DateTime.Today.AddDays(1).Subtract(DateTime.UtcNow);
            var secondsToMidnight = (int)ts.TotalSeconds;

            return secondsToMidnight;
        }

        public static T FromJson<T>(this string str)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject<T>(str, settings);
        }

        public static string ToJson<T>(this T obj, Formatting formatting = Formatting.None)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }

        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(outStream, CompressionMode.Compress))
                using (MemoryStream srcStream = new MemoryStream(data))
                    srcStream.CopyTo(gzipStream);
                return outStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] compressed)
        {
            using (MemoryStream inStream = new MemoryStream(compressed))
            using (GZipStream gzipStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (MemoryStream outStream = new MemoryStream())
            {
                gzipStream.CopyTo(outStream);
                return outStream.ToArray();
            }
        }

        public static string DecompressToString(byte[] compressed)
        {
            using (MemoryStream inStream = new MemoryStream(compressed))
            using (GZipStream gzipStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (MemoryStream outStream = new MemoryStream())
            {
                gzipStream.CopyTo(outStream);
                return Encoding.UTF8.GetString(outStream.ToArray());
            }
        }

    }
}
