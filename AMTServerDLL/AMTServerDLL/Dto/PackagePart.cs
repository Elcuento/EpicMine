namespace AMTServerDLL.Dto
{
    public class PackagePart
    {
        public long SendTime { get; private set; }

        public byte[] Data { get; private set; }

        public string Id { get; private set; }
        public string PackageId { get; private set; }

        public int Part { get; private set; }
        public int PartCount { get; private set; }

        public bool IsResponse { get; private set; }

        public PackagePart(string id, bool isResponse, long sendTime, byte[] data, string packageId, int part, int partCount)
        {
            Id = id;
            SendTime = sendTime;
            Data = data;
            PackageId = packageId;
            Part = part;
            PartCount = partCount;
            IsResponse = isResponse;
        }


        public void SetPartsCount(int max)
        {
            PartCount = max;
        }
    }
}