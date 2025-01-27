using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataGetLanguageDictionary : SendData
    {
        public LocalizationData Data;

        public ResponseDataGetLanguageDictionary(LocalizationData data)
        {
            Data = data;
        }
    }
}