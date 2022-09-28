using System;
namespace st_dotnet.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
    }
}

