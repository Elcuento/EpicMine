namespace AMTServerDLL.Dto
{

    internal class SendDataPackagePart : SendData
    {
        public int Part { get; private set; }
        public string PackageId { get; private set; }
        public string PackagePartId { get; private set; }

        public SendDataPackagePart(int part, string packageId, string packagePartId)
        {
            Part = part;
            PackageId = packageId;
            PackagePartId = packagePartId;
        }
    }
}