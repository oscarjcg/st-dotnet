using st_dotnet.Data;
using st_dotnet.Models;
using st_dotnet.UnitTests.Helpers;

namespace st_dotnet.UnitTests.Data;

public class SqlChannelDataTests : IDisposable
{
    private readonly SqliteDb db = new();
    private readonly GalleryDbContext.SqlChannelData data;

    public SqlChannelDataTests()
    {
        data = new GalleryDbContext.SqlChannelData(db.Context);
    }

    public void Dispose() => db.Dispose();

    private Channel Seed(string name) =>
        data.Add(new Channel { Name = name, Image = "i", Preview = "p", Content = "c", ChannelTypeId = 1 });

    [Fact]
    public void Add_saves_immediately()
    {
        Seed("speedruns");

        using var fresh = db.NewContext();
        Assert.Equal("speedruns", Assert.Single(fresh.channels).Name);
    }

    [Fact]
    public void GetbyId_returns_null_when_missing()
    {
        Assert.Null(data.GetbyId(123));
    }

    [Fact]
    public void GetbyName_matches_exact_name_only()
    {
        Seed("speedruns");
        Seed("speedruns-2");

        Assert.Equal("speedruns", data.GetbyName("speedruns").Name);
        Assert.Null(data.GetbyName("speed"));
    }

    [Fact]
    public void Search_returns_channels_containing_text()
    {
        Seed("retro-speedruns");
        Seed("cooking");

        Assert.Equal("retro-speedruns", Assert.Single(data.Search("speed")).Name);
    }

    [Fact]
    public void Delete_removes_existing_channel()
    {
        var channel = Seed("speedruns");

        Assert.Same(channel, data.Delete(channel.Id));

        using var fresh = db.NewContext();
        Assert.Empty(fresh.channels);
    }

    [Fact]
    public void Delete_returns_null_when_missing()
    {
        Assert.Null(data.Delete(77));
    }
}
