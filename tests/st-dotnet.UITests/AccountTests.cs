using System.Text.RegularExpressions;
using st_dotnet.UITests.Infrastructure;

namespace st_dotnet.UITests;

public class AccountTests : UiTest
{
    public AccountTests(AppFixture app) : base(app) { }

    [Fact]
    public Task Home_page_offers_login_and_register_in_development() => Run(async page =>
    {
        await page.GotoAsync("/");

        await Expect(page.Locator("h1")).ToContainTextAsync("Welcome");
        await Expect(page.Locator(".text-center #login")).ToBeVisibleAsync();
        await Expect(page.Locator(".text-center #register")).ToBeVisibleAsync();
    });

    [Fact]
    public Task Registering_a_new_user_signs_them_in() => Run(async page =>
    {
        var user = TestUser.New();

        await Pages.RegisterAsync(page, user);

        await Expect(page.Locator("#manage")).ToContainTextAsync(user.Email);
    });

    [Fact]
    public Task Login_with_correct_password_signs_in() => Run(async page =>
    {
        await Pages.LoginAsync(page, App.User.Email, App.User.Password);

        await Expect(page.Locator("#logout")).ToBeVisibleAsync();
    });

    [Fact]
    public Task Login_with_wrong_password_shows_error() => Run(async page =>
    {
        await Pages.LoginAsync(page, App.User.Email, "Wrong!Password1");

        await Expect(page.Locator("#account .validation-summary-errors")).ToContainTextAsync("Invalid login attempt");
        await Expect(page.Locator("#logout")).ToHaveCountAsync(0);
    });

    [Fact]
    public Task Logout_signs_out() => Run(async page =>
    {
        await page.GotoAsync("/");
        await page.ClickAsync("#logout");

        await Expect(page.Locator("#login").First).ToBeVisibleAsync();
        await Expect(page.Locator("#logout")).ToHaveCountAsync(0);
    }, signedIn: true);

    [Theory]
    [InlineData("/Category")]
    [InlineData("/Channel")]
    [InlineData("/ChannelType")]
    [InlineData("/Comment")]
    public Task Admin_pages_redirect_anonymous_users_to_login(string path) => Run(async page =>
    {
        await page.GotoAsync(path);

        await Expect(page).ToHaveURLAsync(new Regex("/Identity/Account/Login\\?ReturnUrl=", RegexOptions.IgnoreCase));
    }, name: $"Admin_pages_redirect_anonymous_users_to_login{path.Replace('/', '_')}");
}

public class ProductionAccountTests : UiTest
{
    public ProductionAccountTests(AppFixture app) : base(app) { }

    [Fact]
    public Task Register_is_disabled_in_production() => Run(async page =>
    {
        Assert.False(string.IsNullOrEmpty(App.ProdUrl), "APP_PROD_URL is not set");

        await page.GotoAsync("/Identity/Account/Register");

        await Expect(page.GetByText("Registration is disabled.")).ToBeVisibleAsync();
        await Expect(page.Locator("#registerSubmit")).ToBeDisabledAsync();
    }, baseUrl: App.ProdUrl);

    [Fact]
    public Task Home_page_hides_register_link_in_production() => Run(async page =>
    {
        await page.GotoAsync("/");

        await Expect(page.GetByText("Registering is disabled")).ToBeVisibleAsync();
        await Expect(page.Locator("#register")).ToHaveCountAsync(0);
    }, baseUrl: App.ProdUrl);
}
