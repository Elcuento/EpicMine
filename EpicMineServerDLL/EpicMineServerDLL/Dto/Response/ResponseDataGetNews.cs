using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataGetNews : SendData
    {
        public List<News> Data;

        public ResponseDataGetNews(List<News> news)
        {
            Data = news;
        }
    }
}