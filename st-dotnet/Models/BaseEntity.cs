using System;
using System.ComponentModel.DataAnnotations;

namespace st_dotnet.Models
{
    public class BaseEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

