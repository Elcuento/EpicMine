using System;
using System.IO;
using System.Linq;
using AMTServerDLL.Dto;

namespace AMTServer.Core
{
    public class LogSystem : IDisposable
    {
        private class LogData
        {
            public string FullPath;

            public int MaxFileCount;
            public long MaxSizePerFile;

            public string Name;
            public string Path;
            public long Size;
            public int FileCount;

            private object _writeLock = new object();

            public LogData(string path, string name, int maxFiles, long maxSizePerFile)
            {
                Name = name;
                Path = path;

                FullPath = Path + Name;

                MaxFileCount = maxFiles;
                MaxSizePerFile = maxSizePerFile;

                Common.Utils.CreateDirectoryIfNotExist(Path);

                GetData();
            }

            private void GetData()
            {
                Size = File.Exists(FullPath) ? File.ReadAllText(FullPath).Length : 0;
                FileCount = Directory.GetFiles(Path).Length;
            }

            public void Check()
            {
                if (FileCount > MaxFileCount)
                {
                    FileCount = MaxFileCount;

                    var info = new DirectoryInfo(Path);
                    
                    var files = info.GetFiles().OrderBy(p => p.CreationTime).ToList();

                    for (var index = MaxFileCount; index < files.Count; index++)
                    {
                        var file = files[index];

                        if (file.Name == Name)
                            continue;

                        File.Delete(Path + file.Name);
                    }
                }
            }

            public void Write(string strLength)
            {
                lock (_writeLock)
                {
                    Size += strLength.Length;

                    File.AppendAllText(FullPath, strLength);

                    if (Size * Constants.SymbolSize > MaxSizePerFile)
                    {
                        Save();
                    }
                }

            }

            private void Save()
            {
                var date = DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss");

                if (File.Exists(FullPath))
                {
                    File.Copy(FullPath, FullPath + $"_{date}.log");
                    File.Delete(FullPath);
                }

                Size = 0;
                FileCount++;

                Check();
            }
        }

        private const long MaxFileSize = 1048576 * 10 * 5;
        private const int MaxFiles = 5;

        private const string DirectoryName = "\\Logs\\";
        private const string DirectoryMessagesName = "\\LogMessages\\";
        private const string DirectoryErrorsName = "\\LogErrors\\";
        private const string FileMessageName = "Log_Messages.log";
        private const string FileErrorsName = "Log_Errors.log";

        public static LogSystem Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var path = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\");

                _instance = new LogSystem(path);
                return _instance;
            }
        }

        private static LogSystem _instance;

        private readonly string _path;

        private readonly object _locker = new object();

        private readonly LogData _messages;

        private readonly LogData _errors;

        public LogSystem(string path)
        {
            _path = path + DirectoryName;

            _instance = this;

            try
            {

                Common.Utils.CreateDirectoryIfNotExist(path + DirectoryName);

                _messages = new LogData(_path + DirectoryMessagesName, FileMessageName, MaxFiles, MaxFileSize);
                _errors = new LogData(_path + DirectoryErrorsName, FileErrorsName, MaxFiles, MaxFileSize);

            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }

            _messages.Check();
            _errors.Check();
        }

        private void LogInner(string txt, bool isError)
        {
            var str =
                $"[{DateTime.Now.ToString("yy-MM-dd")}][{DateTime.Now.ToString("T")}][{(isError ? "ERROR" : "MESSAGE")}]" +
                txt + "\n";

            lock (_locker)
            {
                Common.Utils.CreateDirectoryIfNotExist(_path);

                if (isError)
                {
                    _errors.Write(str);
                }
                else
                {
                    _messages.Write(str);
                }
            }

        }

        public static void Log(string str, bool isError = false)
        {
            try
            {
                Instance.LogInner(str, isError);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void Log(Exception exception, bool isError = true)
        {
            try
            {
                Instance.LogInner(exception.ToString(), isError);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Dispose()
        {
            _instance = null;
        }
    }
}
