using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using st_dotnet.Data;

namespace st_dotnet.UnitTests.Helpers;

public sealed class SqliteDb : IDisposable
{
    private readonly SqliteConnection connection;

    public SqliteDb()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        Context = NewContext();
        Context.Database.EnsureCreated();
    }

    public GalleryDbContext Context { get; }

    public GalleryDbContext NewContext() =>
        new(new DbContextOptionsBuilder<GalleryDbContext>().UseSqlite(connection).Options);

    public void Dispose()
    {
        Context.Dispose();
        connection.Dispose();
    }
}
