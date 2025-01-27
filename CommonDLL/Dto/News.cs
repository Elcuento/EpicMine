namespace CommonDLL.Dto
{
    public class News
    {
        public string Title;
        public string Date;
        public string Platform;
        public string Description;
        public string Language;

        public News(string title, string description, string date, string language, string platform)
        {
            Title = title;
            Description = description;
            Date = date;
            Language = language;
            Platform = platform;
        }
    }
}