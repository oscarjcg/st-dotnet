using System;
using System.ComponentModel.DataAnnotations;

namespace st_dotnet.Models
{
    public class ChannelType : BaseEntity
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
    }
}