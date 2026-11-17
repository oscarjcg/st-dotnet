namespace st_dotnet.UITests.Infrastructure;

public static class Pages
{
    public static readonly byte[] Png = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

    public static FilePayload PngFile(string name) => new() { Name = name, MimeType = "image/png", Buffer = Png };

    public static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid().ToString("N")[..8]}";

    public static async Task RegisterAsync(IPage page, TestUser user)
    {
        await page.GotoAsync("/Identity/Account/Register");
        await page.FillAsync("#Input_Email", user.Email);
        await page.FillAsync("#Input_Password", user.Password);
        await page.FillAsync("#Input_ConfirmPassword", user.Password);
        await page.ClickAsync("#registerSubmit");
        await Expect(page.Locator("#logout")).ToBeVisibleAsync();
    }

    public static async Task LoginAsync(IPage page, string email, string password)
    {
        await page.GotoAsync("/Identity/Account/Login");
        await page.FillAsync("#Input_Email", email);
        await page.FillAsync("#Input_Password", password);
        await page.ClickAsync("#login-submit");
    }

    public static Task ExpectSuccessAlert(IPage page) =>
        Expect(page.Locator("#status .alert-success")).ToBeVisibleAsync();

    public static ILocator Row(IPage page, string tableId, string text) =>
        page.Locator($"#{tableId} tbody tr", new() { HasText = text });
}
