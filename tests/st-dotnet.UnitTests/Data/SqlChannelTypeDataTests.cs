using st_dotnet.Data;
using st_dotnet.Models;
using st_dotnet.UnitTests.Helpers;

namespace st_dotnet.UnitTests.Data;

public class SqlChannelTypeDataTests : IDisposable
{
    private readonly SqliteDb db = new();
    private readonly GalleryDbContext.SqlChannelTypeData data;

    public SqlChannelTypeDataTests()
    {
        data = new GalleryDbContext.SqlChannelTypeData(db.Context);
    }

    public void Dispose() => db.Dispose();

    [Fact]
    public void GetbyId_returns_null_when_missing()
    {
        Assert.Null(data.GetbyId(5));
    }

    [Fact]
    public void GetbyId_returns_existing()
    {
        var type = data.Add(new ChannelType { Name = "Video" });

        Assert.Equal("Video", data.GetbyId(type.Id).Name);
    }

    [Fact]
    public void GetbyName_returns_first_prefix_match_or_null()
    {
        data.Add(new ChannelType { Name = "Video" });
        data.Add(new ChannelType { Name = "Voice" });

        Assert.Equal("Video", data.GetbyName("V").Name);
        Assert.Equal("Voice", data.GetbyName("Vo").Name);
        Assert.Null(data.GetbyName("Audio"));
    }

    [Fact]
    public void Delete_returns_null_when_missing()
    {
        Assert.Null(data.Delete(5));
    }

    [Fact]
    public void Delete_removes_existing()
    {
        var type = data.Add(new ChannelType { Name = "Video" });

        data.Delete(type.Id);

        using var fresh = db.NewContext();
        Assert.Empty(fresh.channelTypes);
    }

    [Fact]
    public void Update_saves_immediately()
    {
        var type = data.Add(new ChannelType { Name = "Video" });
        type.Name = "Streaming";

        data.Update(type);

        using var fresh = db.NewContext();
        Assert.Equal("Streaming", fresh.channelTypes.Single().Name);
    }
}
