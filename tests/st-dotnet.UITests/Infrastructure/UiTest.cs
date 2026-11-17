using System.Runtime.CompilerServices;

namespace st_dotnet.UITests.Infrastructure;

[Collection(AppCollection.Name)]
public abstract class UiTest
{
    protected UiTest(AppFixture app)
    {
        App = app;
    }

    protected AppFixture App { get; }

    protected Task Run(Func<IPage, Task> test, bool signedIn = false, string? baseUrl = null,
        [CallerMemberName] string name = "")
    {
        return RunCore(test, signedIn, baseUrl ?? App.AppUrl, $"{GetType().Name}.{name}");
    }

    private async Task RunCore(Func<IPage, Task> test, bool signedIn, string baseUrl, string name)
    {
        var options = new BrowserNewContextOptions { BaseURL = baseUrl };
        if (signedIn) options.StorageState = App.AuthState;

        await using var context = await App.Browser.NewContextAsync(options);
        context.SetDefaultTimeout(15_000);
        await context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true, Sources = false });
        var page = await context.NewPageAsync();

        try
        {
            await test(page);
            await context.Tracing.StopAsync();
        }
        catch
        {
            var basePath = Path.Combine(App.ResultsDir, name);
            try { await page.ScreenshotAsync(new() { Path = basePath + ".png", FullPage = true }); } catch { }
            await context.Tracing.StopAsync(new() { Path = basePath + ".zip" });
            throw;
        }
    }
}
