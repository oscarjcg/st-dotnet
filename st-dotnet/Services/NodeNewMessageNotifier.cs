using System.Text;
using Newtonsoft.Json;
using st_dotnet.Models;

namespace st_dotnet.Services
{
    public class NodeNewMessageNotifier : INewMessageNotifier
    {
        public const string UrlConfigKey = "NodeService:NewMessageUrl";

        private readonly HttpClient httpClient;
        private readonly ILogger<NodeNewMessageNotifier> logger;
        private readonly string? url;

        public NodeNewMessageNotifier(HttpClient httpClient, IConfiguration configuration, ILogger<NodeNewMessageNotifier> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            url = configuration[UrlConfigKey];
        }

        public async Task NotifyAsync(string channelId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            var json = JsonConvert.SerializeObject(new NewMessageRequest { channel_id = channelId });
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var response = await httpClient.PostAsync(url, content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Node new-message notification returned {StatusCode}", (int)response.StatusCode);
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                logger.LogWarning(ex, "Node new-message notification failed for channel {ChannelId}", channelId);
            }
        }
    }
}
