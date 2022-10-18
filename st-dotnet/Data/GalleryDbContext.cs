using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using st_dotnet.Models;

namespace st_dotnet.Data
{
    public partial class GalleryDbContext: IdentityDbContext<IdentityUser>
    {
        public DbSet<Category> categories { get; set; }
        public DbSet<Channel> channels { get; set; }
        public DbSet<Comment> comments { get; set; }


        public GalleryDbContext(DbContextOptions<GalleryDbContext> options): base(options)
        {
        }
    }
}

