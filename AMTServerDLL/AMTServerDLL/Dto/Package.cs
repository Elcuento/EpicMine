using System;
using Newtonsoft.Json;

namespace AMTServerDLL.Dto
{
    public class Package
    {
        public object Data { get; private set; }

        public int Command { get; private set; }

        public string Id { get; private set; }
        public string ErrorMessage { get; private set; }

        public long SendTime;

        [JsonProperty]
        internal SystemCommandType Type { get; private set; }

        [JsonConstructor]
        internal Package(long sendTime, object data, string id, string errorMessage, SystemCommandType type, int command = 0)
        {
            SendTime = sendTime;
            Type = type;
            Data = data;
            Id = id;
            ErrorMessage = errorMessage;
            Command = command;
        }
    }
}