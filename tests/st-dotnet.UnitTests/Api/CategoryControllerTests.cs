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

public class CategoryControllerTests
{
    private const string Bucket = "test-bucket";
    private const string PublicUrl = "http://minio:9000/test-bucket/";

    private readonly ICategoryRepository categories = Substitute.For<ICategoryRepository>();
    private readonly IChannelRepository channels = Substitute.For<IChannelRepository>();
    private readonly IAmazonS3 s3 = Substitute.For<IAmazonS3>();
    private readonly CategoryController controller;

    public CategoryControllerTests()
    {
        s3.DoesS3BucketExistAsync(Bucket).Returns(true);
        s3.PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>()).Returns(new PutObjectResponse());
        controller = new CategoryController(categories, channels, s3,
            Options.Create(new S3Settings { BucketName = Bucket, PublicUrl = PublicUrl }));
    }

    private static IFormCollection NamedForm(string name, bool withImage = true) =>
        Forms.Create(new Dictionary<string, string> { ["name"] = name },
            withImage ? new[] { Forms.File("image") } : Array.Empty<IFormFile>());

    [Fact]
    public async Task Post_returns_404_when_bucket_is_missing()
    {
        s3.DoesS3BucketExistAsync(Bucket).Returns(false);

        var result = await controller.Post(NamedForm("Music"));

        Assert.IsType<NotFoundObjectResult>(result);
        categories.DidNotReceiveWithAnyArgs().Add(default!);
    }

    [Fact]
    public async Task Post_returns_400_without_image()
    {
        var result = await controller.Post(NamedForm("Music", withImage: false));

        Assert.IsType<BadRequestResult>(result);
        await s3.DidNotReceive().PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Post_uploads_to_configured_bucket_and_stores_public_url()
    {
        PutObjectRequest? uploaded = null;
        s3.PutObjectAsync(Arg.Do<PutObjectRequest>(r => uploaded = r), Arg.Any<CancellationToken>())
            .Returns(new PutObjectResponse());

        var result = await controller.Post(NamedForm("Music"));

        Assert.IsType<OkResult>(result);
        Assert.NotNull(uploaded);
        Assert.Equal(Bucket, uploaded.BucketName);
        Assert.StartsWith("public/category/", uploaded.Key);
        categories.Received(1).Add(Arg.Is<Category>(c =>
            c.Name == "Music" && c.Image == PublicUrl + uploaded.Key));
    }

    [Fact]
    public async Task Put_returns_404_for_unknown_category()
    {
        categories.GetbyId(9).Returns((Category)null);

        var result = await controller.Put(9, NamedForm("x"));

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Put_updates_name_and_image()
    {
        var existing = new Category { Id = 2, Name = "Old", Image = "old" };
        categories.GetbyId(2).Returns(existing);

        var result = await controller.Put(2, NamedForm("New"));

        Assert.IsType<OkResult>(result);
        Assert.Equal("New", existing.Name);
        Assert.StartsWith(PublicUrl + "public/category/", existing.Image);
        categories.Received(1).Update(existing);
    }

    [Fact]
    public void Get_by_id_returns_404_when_missing_and_value_when_found()
    {
        var music = new Category { Id = 1, Name = "Music" };
        categories.GetbyId(1).Returns(music);
        categories.GetbyId(2).Returns((Category)null);

        Assert.Same(music, controller.Get(1).Value);
        Assert.IsType<NotFoundResult>(controller.Get(2).Result);
    }

    [Fact]
    public void Get_by_name_returns_404_when_missing()
    {
        categories.GetbyName("nope").Returns((Category)null);

        Assert.IsType<NotFoundResult>(controller.GetCategoryByName("nope").Result);
        Assert.IsType<NotFoundResult>(controller.GetChannelsByName("nope").Result);
    }

    [Fact]
    public void GetChannelsByName_returns_channels_of_category()
    {
        var list = new List<Channel> { new() { Name = "speedruns" } };
        categories.GetbyName("Games").Returns(new Category { Id = 4 });
        categories.GetChannels(4).Returns(new Category { Id = 4, Channels = list });

        var ok = Assert.IsType<OkObjectResult>(controller.GetChannelsByName("Games").Result);
        Assert.Same(list, ok.Value);
    }

    [Fact]
    public void Delete_returns_404_when_missing()
    {
        categories.Delete(5).Returns((Category)null);

        Assert.IsType<NotFoundResult>(controller.Delete(5).Result);
    }
}
