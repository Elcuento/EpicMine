namespace AMTServerDLL.Dto
{
    public class RequestDataGetLanguageDictionary : SendData
    {
        public string LanguageCode;

        public RequestDataGetLanguageDictionary(string code)
        {
            LanguageCode = code;
        }
    }
}