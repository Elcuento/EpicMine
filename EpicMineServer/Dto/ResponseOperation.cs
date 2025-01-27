using System.Text;
using AMTServerDLL.Dto;
using Utils = AMTServerDLL.Utils;

namespace AMTServer.Dto
{
    public class ResponseOperation
    {
        public byte[] OperationDataArchive { get; private set; }

        public string OperationId { get; private set; }

        public int OperationLocalCommand { get; private set; }

        public string UniqueName => OperationId + OperationLocalCommand;

        public ResponseOperation(string id, int command, SendData data)
        {
            OperationId = id;
            OperationLocalCommand = command;
            OperationDataArchive = Utils.Compress(Encoding.UTF8.GetBytes(Utils.ToJson(data)));
        }

        public bool Compare(Package pack)
        {
            var name = pack.Id + pack.Command;

            return UniqueName == name;
        }
    }
}
