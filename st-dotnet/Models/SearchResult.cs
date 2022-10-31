using System;
namespace st_dotnet.Models
{
    public class SearchResult
    {
        public IEnumerable<Channel> Channels { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}

