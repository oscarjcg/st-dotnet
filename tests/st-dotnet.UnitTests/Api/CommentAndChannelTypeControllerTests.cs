using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using st_dotnet.Api.Controllers;
using st_dotnet.Models;
using st_dotnet.Services;
using st_dotnet.UnitTests.Helpers;

namespace st_dotnet.UnitTests.Api;

public class CommentControllerTests
{
    private readonly ICommentRepository comments = Substitute.For<ICommentRepository>();
    private readonly IChannelRepository channels = Substitute.For<IChannelRepository>();
    private readonly INewMessageNotifier notifier = Substitute.For<INewMessageNotifier>();
    private readonly CommentController controller;

    public CommentControllerTests()
    {
        controller = new CommentController(comments, channels, notifier);
    }

    [Fact]
    public async Task Post_saves_comment_then_notifies_node()
    {
        var form = Forms.Create(new Dictionary<string, string>
        {
            ["channel_id"] = "7", ["author"] = "ana", ["comment"] = "hello"
        });

        var result = await controller.Post(form);

        Assert.IsType<OkResult>(result);
        Received.InOrder(() =>
        {
            comments.Add(Arg.Is<Comment>(c => c.ChannelId == 7 && c.Author == "ana" && c.Text == "hello"));
            notifier.NotifyAsync("7", Arg.Any<CancellationToken>());
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("seven")]
    public async Task Post_returns_400_for_invalid_channel_id(string? channelId)
    {
        var fields = new Dictionary<string, string> { ["author"] = "ana", ["comment"] = "hello" };
        if (channelId != null) fields["channel_id"] = channelId;

        Assert.IsType<BadRequestResult>(await controller.Post(Forms.Create(fields)));
        comments.DidNotReceiveWithAnyArgs().Add(default!);
        await notifier.DidNotReceiveWithAnyArgs().NotifyAsync(default!, default);
    }

    [Fact]
    public void Delete_returns_404_when_missing()
    {
        comments.Delete(3).Returns((Comment)null);

        Assert.IsType<NotFoundResult>(controller.Delete(3).Result);
    }

    [Fact]
    public void GetChannelCommentsByName_returns_404_for_unknown_channel()
    {
        channels.GetbyName("ghost").Returns((Channel)null);

        Assert.IsType<NotFoundResult>(controller.GetChannelCommentsByName("ghost").Result);
    }

    [Fact]
    public void GetChannelCommentsByName_returns_comments_of_channel()
    {
        var list = new List<Comment> { new() { Text = "hi" } };
        channels.GetbyName("speedruns").Returns(new Channel { Id = 7 });
        comments.GetChannelComments(7).Returns(list);

        var ok = Assert.IsType<OkObjectResult>(controller.GetChannelCommentsByName("speedruns").Result);
        Assert.Same(list, ok.Value);
    }
}

public class ChannelTypeControllerTests
{
    private readonly IChannelTypeRepository types = Substitute.For<IChannelTypeRepository>();
    private readonly ChannelTypeController controller;

    public ChannelTypeControllerTests()
    {
        controller = new ChannelTypeController(types);
    }

    [Fact]
    public void Post_adds_channel_type_with_name()
    {
        controller.Post(Forms.Create(new Dictionary<string, string> { ["name"] = "Video" }));

        types.Received(1).Add(Arg.Is<ChannelType>(t => t.Name == "Video"));
    }

    [Fact]
    public void Put_returns_404_for_unknown_and_updates_existing()
    {
        var existing = new ChannelType { Id = 1, Name = "Old" };
        types.GetbyId(1).Returns(existing);
        types.GetbyId(2).Returns((ChannelType)null);
        var form = Forms.Create(new Dictionary<string, string> { ["name"] = "New" });

        Assert.IsType<NotFoundResult>(controller.Put(2, form));
        Assert.IsType<OkObjectResult>(controller.Put(1, form));
        Assert.Equal("New", existing.Name);
        types.Received(1).Update(existing);
    }

    [Fact]
    public void Get_and_Delete_return_404_when_missing()
    {
        types.GetbyId(Arg.Any<int>()).Returns((ChannelType)null);
        types.Delete(Arg.Any<int>()).Returns((ChannelType)null);

        Assert.IsType<NotFoundResult>(controller.Get(1).Result);
        Assert.IsType<NotFoundResult>(controller.Delete(1).Result);
    }
}
