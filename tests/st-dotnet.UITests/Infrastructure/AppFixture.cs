namespace st_dotnet.UITests.Infrastructure;

public sealed class AppFixture : IAsyncLifetime
{
    public string AppUrl { get; } = (Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:8086").TrimEnd('/');
    public string ProdUrl { get; } = (Environment.GetEnvironmentVariable("APP_PROD_URL") ?? "").TrimEnd('/');
    public string ResultsDir { get; } = Environment.GetEnvironmentVariable("RESULTS_DIR") ?? Path.Combine(AppContext.BaseDirectory, "test-results");

    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;

    public TestUser User { get; } = TestUser.New();

    public string AuthState { get; private set; } = "";

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(ResultsDir);
        await WaitUntilUp(AppUrl);
        if (ProdUrl != "") await WaitUntilUp(ProdUrl);

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new() { Headless = true });

        await using var context = await Browser.NewContextAsync(new() { BaseURL = AppUrl });
        var page = await context.NewPageAsync();
        await Pages.RegisterAsync(page, User);
        AuthState = await context.StorageStateAsync();
    }

    public async Task DisposeAsync()
    {
        if (Browser != null) await Browser.DisposeAsync();
        Playwright?.Dispose();
    }

    private static async Task WaitUntilUp(string url)
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var deadline = DateTime.UtcNow.AddSeconds(120);
        Exception? last = null;
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                using var response = await http.GetAsync(url);
                if (response.IsSuccessStatusCode) return;
                last = new Exception($"{url} returned {(int)response.StatusCode}");
            }
            catch (Exception ex) { last = ex; }
            await Task.Delay(1000);
        }
        throw new TimeoutException($"{url} did not become ready in 120 s", last);
    }
}

[CollectionDefinition(Name)]
public sealed class AppCollection : ICollectionFixture<AppFixture>
{
    public const string Name = "app";
}

public sealed record TestUser(string Email, string Password)
{
    public static TestUser New()
    {
        var id = Guid.NewGuid().ToString("N")[..12];
        return new TestUser($"ui-{id}@example.test", $"Ui!{id}Aa1");
    }
}
