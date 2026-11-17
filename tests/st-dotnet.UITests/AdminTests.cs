using System.Text.RegularExpressions;
using st_dotnet.UITests.Infrastructure;

namespace st_dotnet.UITests;

public class AdminTests : UiTest
{
    public AdminTests(AppFixture app) : base(app) { }

    [Fact]
    public Task ChannelType_can_be_created_edited_and_deleted() => Run(async page =>
    {
        var name = Pages.Unique("type");
        var renamed = name + "-renamed";

        await page.GotoAsync("/ChannelType/Create");
        await page.FillAsync("#create-form #name", name);
        await page.ClickAsync("#create-form button");
        await Pages.ExpectSuccessAlert(page);

        await page.GotoAsync("/ChannelType");
        var row = Pages.Row(page, "channel-types-table", name);
        await Expect(row).ToHaveCountAsync(1);

        await row.Locator(".btn-edit").ClickAsync();
        await Expect(page).ToHaveURLAsync(new Regex("/channeltype/edit/\\d+", RegexOptions.IgnoreCase));
        await page.FillAsync("#edit-form #name", renamed);
        await page.ClickAsync("#edit-form button");
        await Pages.ExpectSuccessAlert(page);

        await page.GotoAsync("/ChannelType");
        row = Pages.Row(page, "channel-types-table", renamed);
        await Expect(row).ToHaveCountAsync(1);

        await row.Locator(".btn-delete").ClickAsync();
        await Pages.ExpectSuccessAlert(page);
        await page.ReloadAsync();
        await Expect(Pages.Row(page, "channel-types-table", renamed)).ToHaveCountAsync(0);
    }, signedIn: true);

    [Fact]
    public Task Category_can_be_created_with_an_image() => Run(async page =>
    {
        var name = Pages.Unique("category");

        await page.GotoAsync("/Category/Create");
        await page.FillAsync("#create-form #name", name);
        await page.SetInputFilesAsync("#create-form #image", Pages.PngFile("cover.png"));
        await page.ClickAsync("#create-form button");
        await Pages.ExpectSuccessAlert(page);

        await page.GotoAsync("/Category");
        var row = Pages.Row(page, "categories-table", name);
        await Expect(row).ToHaveCountAsync(1);

        var imageUrl = (await row.Locator("td").Nth(1).InnerTextAsync()).Trim();
        Assert.Matches("^https?://.+/public/category/", imageUrl);
        var image = await page.APIRequest.GetAsync(imageUrl);
        Assert.Equal(200, image.Status);
        Assert.Equal(Pages.Png, await image.BodyAsync());
    }, signedIn: true);

    [Fact]
    public Task Category_can_be_renamed_with_a_new_image() => Run(async page =>
    {
        var name = Pages.Unique("category");
        await CreateCategoryViaApi(page, name);

        await page.GotoAsync("/Category");
        await Pages.Row(page, "categories-table", name).Locator(".btn-edit").ClickAsync();
        await page.FillAsync("#edit-form #name", name + "-v2");
        await page.SetInputFilesAsync("#edit-form #image", Pages.PngFile("cover2.png"));
        await page.ClickAsync("#edit-form button");
        await Pages.ExpectSuccessAlert(page);

        await page.GotoAsync("/Category");
        await Expect(Pages.Row(page, "categories-table", name + "-v2")).ToHaveCountAsync(1);
    }, signedIn: true);

    [Fact]
    public Task Channel_can_be_created_with_images_and_type() => Run(async page =>
    {
        var typeName = Pages.Unique("type");
        var channelName = Pages.Unique("channel");
        var created = await page.APIRequest.PostAsync("/api/channeltype", new()
        {
            Multipart = page.APIRequest.CreateFormData().Set("name", typeName)
        });
        Assert.True(created.Ok, $"creating channel type returned {created.Status}");

        await page.GotoAsync("/Channel/Create");
        await page.FillAsync("#create-form #name", channelName);
        await page.FillAsync("#create-form #password", "unused");
        await page.SetInputFilesAsync("#create-form #image", Pages.PngFile("image.png"));
        await page.SetInputFilesAsync("#create-form #preview", Pages.PngFile("preview.png"));
        await page.FillAsync("#create-form #content", "rtmp://stream/" + channelName);
        await page.SelectOptionAsync("#create-form #channelTypeId", new SelectOptionValue { Label = typeName });
        await page.ClickAsync("#create-form button");
        await Pages.ExpectSuccessAlert(page);

        await page.GotoAsync("/Channel");
        var row = Pages.Row(page, "channels-table", channelName);
        await Expect(row).ToHaveCountAsync(1);
        await Expect(row).ToContainTextAsync("rtmp://stream/" + channelName);
    }, signedIn: true);

    [Fact]
    public Task Comment_posted_through_the_api_is_listed_and_can_be_deleted() => Run(async page =>
    {
        var text = Pages.Unique("hello");
        var posted = await page.APIRequest.PostAsync("/api/comment", new()
        {
            Multipart = page.APIRequest.CreateFormData()
                .Set("channel_id", "1")
                .Set("author", "ui-test")
                .Set("comment", text)
        });
        Assert.True(posted.Ok, $"POST /api/comment returned {posted.Status}");

        await page.GotoAsync("/Comment");
        var row = Pages.Row(page, "comments-table", text);
        await Expect(row).ToHaveCountAsync(1);

        await row.Locator(".btn-delete").ClickAsync();
        await Pages.ExpectSuccessAlert(page);
        await page.ReloadAsync();
        await Expect(Pages.Row(page, "comments-table", text)).ToHaveCountAsync(0);
    }, signedIn: true);

    [Fact]
    public Task Api_returns_404_for_missing_items() => Run(async page =>
    {
        foreach (var path in new[] { "/api/category/999999", "/api/channel/999999", "/api/channeltype/999999", "/api/category/byname/no-such-thing" })
        {
            var response = await page.APIRequest.GetAsync(path);
            Assert.True(response.Status == 404, $"{path} returned {response.Status}");
        }
    });

    private static async Task CreateCategoryViaApi(IPage page, string name)
    {
        var response = await page.APIRequest.PostAsync("/api/category", new()
        {
            Multipart = page.APIRequest.CreateFormData()
                .Set("name", name)
                .Set("image", Pages.PngFile("seed.png"))
        });
        Assert.True(response.Ok, $"POST /api/category returned {response.Status}");
    }
}
