using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class LocalizationData
    {
        public int Version;

        public string Code;

        public List<LocalizationString> Values;


        public LocalizationData(int version, string code, List<LocalizationString> values)
        {
            Version = version;
            Code = code;
            Values = values;
        }
    }
}