using System;
using EpicMineServerDLL.Static.Enums;

namespace EpicMineServerDLL.Dto
{
    public class ServerAddress
    {
        public string Ip;
        public int Port;
        public ServerAddressType Type;


        public ServerAddress() { }
        public ServerAddress( byte[] address, int port, ServerAddressType type)
        {
            Ip = GetByteString(address);
            Port = port;
            Type = type;
        }

        public bool IsEqual(ServerAddress ad)
        {
            return Ip == ad.Ip && Port == ad.Port;
        }
        public ServerAddress(string address, int port, ServerAddressType type)
        {
            Ip = address;
            Port = port;
            Type = type;
        }

        public byte[] GetByteIp()
        {
            var str = Ip.Split('.');

            var ad = new byte[str.Length];
            for (var index = 0; index < str.Length; index++)
            {
                ad[index] = (byte)(int.Parse(str[index]));
            }

            return ad;
        }

        public string GetByteString(byte[] ip)
        {
            var str = "";

            for (var index = 0; index < ip.Length; index++)
            {
                var byt = ip[index];
                str += byt;

                if (index != ip.Length - 1)
                    str += ".";

            }

            return str;
        }
    }
}
