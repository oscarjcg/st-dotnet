using st_dotnet.Data;
using st_dotnet.Models;
using st_dotnet.UnitTests.Helpers;

namespace st_dotnet.UnitTests.Data;

public class SqlCommentDataTests : IDisposable
{
    private readonly SqliteDb db = new();
    private readonly ICommentData data;

    public SqlCommentDataTests()
    {
        data = new GalleryDbContext.SqlCommentData(db.Context);
    }

    public void Dispose() => db.Dispose();

    private Comment Seed(int channelId, string text) =>
        data.Add(new Comment { ChannelId = channelId, Author = "ana", Text = text });

    [Fact]
    public void GetChannelComments_only_returns_that_channels_comments()
    {
        Seed(1, "hello");
        Seed(1, "again");
        Seed(2, "other channel");

        var texts = data.GetChannelComments(1).Select(c => c.Text).OrderBy(t => t).ToList();

        Assert.Equal(new[] { "again", "hello" }, texts);
    }

    [Fact]
    public void DeleteChannelComments_removes_only_that_channel()
    {
        Seed(1, "hello");
        Seed(2, "keep me");

        data.DeleteChannelComments(1);

        using var fresh = db.NewContext();
        Assert.Equal("keep me", Assert.Single(fresh.comments).Text);
    }

    [Fact]
    public void Delete_removes_existing_comment()
    {
        var comment = Seed(1, "bye");

        Assert.Same(comment, data.Delete(comment.Id));
        Assert.Empty(data.GetAll());
    }

    [Fact]
    public void Delete_returns_null_when_missing()
    {
        Assert.Null(data.Delete(404));
    }
}
