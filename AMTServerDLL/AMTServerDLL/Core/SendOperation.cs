using System.Collections.Generic;
using AMTServerDLL.Dto;

namespace AMTServerDLL.Core
{
    public class SendOperation 
    {
        public string Id { get; private set; }
        public SendData Data { get; private set; }

        public int LocalCommand { get; private set; }

        public bool IsComplete { get; private set; }
        public bool IsStopped { get; private set; }
        public bool IsSend { get; private set; }
        public bool IsError { get; private set; }
        public string ErrorMessage { get; private set; }

        internal SystemCommandType SystemCommand { get; private set; }

        public SendData ResponseData;
        public List<PackagePart> Parts;
        public List<int> ReceivedParts = new List<int>();

        internal void ReceivePart(int dataPart)
        {
            if(!ReceivedParts.Contains(dataPart))
           ReceivedParts.Add(dataPart);
        }

        internal void ClearReceivedParts()
        {
            ReceivedParts.Clear();
        }

        internal bool IsPartReceived(int part)
        {
            return ReceivedParts.Contains(part);
        }

        internal void Stop()
        {
            IsStopped = true;
            IsComplete = true;
        }

        internal virtual void Complete(SendData data, bool error, string errorMessage)
        {
            IsError = error;
            ErrorMessage = errorMessage;
            ResponseData = data;
            IsComplete = true;
        }

        internal SendOperation(string id, SendData data, SystemCommandType command = SystemCommandType.None, int localCommand = 0)
        {
            Id = id;
            Data = data;
            SystemCommand = command;
            LocalCommand = localCommand;
            Parts = new List<PackagePart>();
        }

        internal void SetSend()
        {
            IsSend = true;
        }

        internal bool IsPartsReceived()
        {
            return Parts.Count == ReceivedParts.Count;
        }

        internal void SetParts(List<PackagePart> messages)
        {
            Parts = messages;
        }
    }
}