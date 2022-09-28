using System;
using System.ComponentModel.DataAnnotations;

namespace st_dotnet.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, StringLength(300)]
        public string Image { get; set; }
    }
}

