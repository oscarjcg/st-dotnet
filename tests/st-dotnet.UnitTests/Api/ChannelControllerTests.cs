using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;
using st_dotnet.Api.Controllers;
using st_dotnet.Models;
using st_dotnet.Services;
using st_dotnet.UnitTests.Helpers;

namespace st_dotnet.UnitTests.Api;

public class ChannelControllerTests
{
    private const string Bucket = "test-bucket";
    private const string PublicUrl = "http://minio:9000/test-bucket/";

    private readonly IChannelRepository channels = Substitute.For<IChannelRepository>();
    private readonly ICategoryRepository categories = Substitute.For<ICategoryRepository>();
    private readonly IChannelTypeRepository channelTypes = Substitute.For<IChannelTypeRepository>();
    private readonly IAmazonS3 s3 = Substitute.For<IAmazonS3>();
    private readonly ChannelController controller;

    public ChannelControllerTests()
    {
        s3.DoesS3BucketExistAsync(Bucket).Returns(true);
        s3.PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>()).Returns(new PutObjectResponse());
        channelTypes.GetbyId(1).Returns(new ChannelType { Id = 1, Name = "Video" });
        controller = new ChannelController(channels, categories, channelTypes, s3,
            Options.Create(new S3Settings { BucketName = Bucket, PublicUrl = PublicUrl }));
    }

    private static IFormCollection Form(string? channelTypeId = "1", bool image = true, bool preview = true)
    {
        var fields = new Dictionary<string, string> { ["name"] = "speedruns", ["content"] = "rtmp://x" };
        if (channelTypeId != null) fields["channelTypeId"] = channelTypeId;
        var files = new List<IFormFile>();
        if (image) files.Add(Forms.File("image"));
        if (preview) files.Add(Forms.File("preview"));
        return Forms.Create(fields, files.ToArray());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    public async Task Post_returns_400_for_missing_or_invalid_channelTypeId(string? channelTypeId)
    {
        var result = await controller.Post(Form(channelTypeId));

        Assert.IsType<BadRequestResult>(result);
        channels.DidNotReceiveWithAnyArgs().Add(default!);
    }

    [Fact]
    public async Task Post_returns_400_for_unknown_channel_type()
    {
        channelTypes.GetbyId(99).Returns((ChannelType)null);

        Assert.IsType<BadRequestResult>(await controller.Post(Form("99")));
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Post_returns_400_when_an_image_is_missing(bool image, bool preview)
    {
        Assert.IsType<BadRequestResult>(await controller.Post(Form(image: image, preview: preview)));
        await s3.DidNotReceive().PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Post_returns_404_when_bucket_is_missing()
    {
        s3.DoesS3BucketExistAsync(Bucket).Returns(false);

        Assert.IsType<NotFoundObjectResult>(await controller.Post(Form()));
    }

    [Fact]
    public async Task Post_uploads_both_files_and_adds_channel()
    {
        var result = await controller.Post(Form());

        Assert.IsType<OkResult>(result);
        await s3.Received(2).PutObjectAsync(Arg.Is<PutObjectRequest>(r => r.BucketName == Bucket), Arg.Any<CancellationToken>());
        channels.Received(1).Add(Arg.Is<Channel>(c =>
            c.Name == "speedruns" &&
            c.ChannelTypeId == 1 &&
            c.Content == "rtmp://x" &&
            c.Image.StartsWith(PublicUrl + "public/channel/") &&
            c.Preview.StartsWith(PublicUrl + "public/channel/") &&
            c.Image != c.Preview));
    }

    [Fact]
    public async Task Put_returns_404_for_unknown_channel_without_touching_s3()
    {
        channels.GetbyId(5).Returns((Channel)null);

        Assert.IsType<NotFoundResult>(await controller.Put(5, Form()));
        await s3.DidNotReceive().DoesS3BucketExistAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task Put_returns_400_for_invalid_channelTypeId()
    {
        channels.GetbyId(5).Returns(new Channel { Id = 5 });

        Assert.IsType<BadRequestResult>(await controller.Put(5, Form("x")));
    }

    [Fact]
    public async Task Put_only_replaces_images_that_were_sent()
    {
        var existing = new Channel { Id = 5, Name = "old", Image = "old-image", Preview = "old-preview", ChannelTypeId = 2 };
        channels.GetbyId(5).Returns(existing);

        var result = await controller.Put(5, Form(image: false, preview: true));

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal("old-image", existing.Image);
        Assert.StartsWith(PublicUrl, existing.Preview);
        Assert.Equal("speedruns", existing.Name);
        Assert.Equal(1, existing.ChannelTypeId);
        channels.Received(1).Update(existing);
    }

    [Fact]
    public void Lookups_return_404_when_missing()
    {
        channels.GetbyId(Arg.Any<int>()).Returns((Channel)null);
        channels.GetbyName(Arg.Any<string>()).Returns((Channel)null);
        channels.Delete(Arg.Any<int>()).Returns((Channel)null);

        Assert.IsType<NotFoundResult>(controller.Get(1).Result);
        Assert.IsType<NotFoundResult>(controller.GetChannelByName("x").Result);
        Assert.IsType<NotFoundResult>(controller.Delete(1).Result);
    }

    [Fact]
    public void AddCategory_returns_404_when_channel_or_category_missing()
    {
        channels.GetbyId(1).Returns(new Channel { Id = 1 });
        channels.GetbyId(2).Returns((Channel)null);
        categories.GetbyId(10).Returns((Category)null);

        Assert.IsType<NotFoundResult>(controller.AddCategory(2, 10));
        Assert.IsType<NotFoundResult>(controller.AddCategory(1, 10));
        channels.DidNotReceiveWithAnyArgs().Update(default!);
    }

    [Fact]
    public void AddCategory_links_category_to_channel()
    {
        var channel = new Channel { Id = 1 };
        var category = new Category { Id = 10 };
        channels.GetbyId(1).Returns(channel);
        categories.GetbyId(10).Returns(category);

        Assert.IsType<OkObjectResult>(controller.AddCategory(1, 10));
        Assert.Same(category, Assert.Single(channel.Categories));
        channels.Received(1).Update(channel);
    }
}
