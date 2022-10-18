using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace st_dotnet.Models
{
    public class Channel
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, StringLength(300)]
        public string Image { get; set; }
        [Required, StringLength(300)]
        public string Preview { get; set; }
        [Required, StringLength(300)]
        public int Type { get; set; }
        [Required, StringLength(300)]
        public string Content { get; set; }

        [JsonIgnore]
        public List<Category> Categories { get; set; }
    }
}