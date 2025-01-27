using AMTServerDLL.Dto;

namespace AMTServerDLL.Core.Events
{
    public class GetDataEvent : EventBase
    {
        public Package Package;

        public GetDataEvent(Package pack)
        {
            Package = pack;
        }
    }
}
