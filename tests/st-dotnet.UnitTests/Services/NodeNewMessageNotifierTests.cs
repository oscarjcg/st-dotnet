using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using st_dotnet.Services;

namespace st_dotnet.UnitTests.Services;

public class NodeNewMessageNotifierTests
{
    private sealed class FakeHandler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, HttpResponseMessage> Respond { get; set; } = _ => new HttpResponseMessage(HttpStatusCode.OK);
        public List<(HttpRequestMessage Request, string Body)> Requests { get; } = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = request.Content == null ? "" : await request.Content.ReadAsStringAsync(cancellationToken);
            Requests.Add((request, body));
            return Respond(request);
        }
    }

    private readonly FakeHandler handler = new();

    private NodeNewMessageNotifier Create(string? url)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { [NodeNewMessageNotifier.UrlConfigKey] = url })
            .Build();
        return new NodeNewMessageNotifier(new HttpClient(handler), config, NullLogger<NodeNewMessageNotifier>.Instance);
    }

    [Fact]
    public async Task Posts_channel_id_as_json_to_configured_url()
    {
        await Create("http://node.test/new-message").NotifyAsync("42");

        var (request, body) = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("http://node.test/new-message", request.RequestUri!.ToString());
        Assert.Equal("application/json", request.Content!.Headers.ContentType!.MediaType);
        Assert.Equal("{\"channel_id\":\"42\"}", body);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Does_nothing_when_url_is_not_configured(string? url)
    {
        await Create(url).NotifyAsync("42");

        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task Swallows_network_errors()
    {
        handler.Respond = _ => throw new HttpRequestException("connection refused");

        var ex = await Record.ExceptionAsync(() => Create("http://node.test/new-message").NotifyAsync("42"));

        Assert.Null(ex);
    }

    [Fact]
    public async Task Does_not_throw_on_error_status()
    {
        handler.Respond = _ => new HttpResponseMessage(HttpStatusCode.InternalServerError);

        var ex = await Record.ExceptionAsync(() => Create("http://node.test/new-message").NotifyAsync("42"));

        Assert.Null(ex);
    }
}
