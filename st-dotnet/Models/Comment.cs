using System;
using System.ComponentModel.DataAnnotations;

namespace st_dotnet.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public int ChannelId { get; set; } // TODO review relationship
        [Required, StringLength(100)]
        public string Author { get; set; }
        [Required, StringLength(300)]
        public string Text { get; set; }
    }
}

