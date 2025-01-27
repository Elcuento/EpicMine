using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AMTServerDLL;

namespace AMTServer
{
    public class ConfigClass
    {
        public int Port;
        public byte[] IpAddress;

        public int MaxConnectionsPerServer;

        public string DbName = "";
        public string DbIpString = "";

        public string ServerName = "";

        public bool IsLoginServer;

        public ConfigClass()
        {
        }

        public ConfigClass(string[] configFile)
        {
            Port = 34566;
            MaxConnectionsPerServer = 100;
            DbName = "EMS";
            IsLoginServer = false;
            IpAddress = new byte[] {127, 0, 0, 1};
            DbIpString = "mongodb://127.0.0.1/";
            ServerName = "ServerName";

            for (var i = 0; i < configFile.Length; i++)
            {
                var s = configFile[i].Split('=');

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (s[0].ToLower())
                {
                    case "port":
                        Port = Convert.ToInt32(s[1]);
                        break;

                    case "isloginserver":
                        IsLoginServer = Convert.ToBoolean(s[1]);
                        break;

                    case "maxconnections":
                        MaxConnectionsPerServer = Convert.ToInt32(s[1]);
                        break;

                    case "ipaddress":
                        var str = s[1].Split('.');
      
                        IpAddress = new byte[str.Length];
                        for (var index = 0; index < str.Length; index++)
                        {
                            IpAddress[index] = (byte)(int.Parse(str[index]));
                        }
                        break;

                    case "dbname":
                        DbName = s[1];
                        break;

                    case "servername":
                        ServerName = s[1];
                        break;
                }
            }
        }

        public static ConfigClass GetDefault()
        {
            return new ConfigClass
            {
                Port = 34566,
                MaxConnectionsPerServer = 100,
                DbName = "EMS",
                IsLoginServer = false,
                DbIpString = "mongodb://127.0.0.1/",
                IpAddress = new byte[]{ 127,0,0,1 },
                ServerName = "Default"
            };
        }
    }
}
