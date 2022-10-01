using System;
using Microsoft.EntityFrameworkCore;
using st_dotnet.Models;

namespace st_dotnet.Data
{
    public partial class GalleryDbContext: DbContext
    {
        public DbSet<Category> categories { get; set; }
        public DbSet<Channel> channels { get; set; }

        public GalleryDbContext(DbContextOptions<GalleryDbContext> options): base(options)
        {
        }
    }
}

