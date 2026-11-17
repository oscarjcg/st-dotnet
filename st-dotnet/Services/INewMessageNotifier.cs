namespace st_dotnet.Services
{
    public interface INewMessageNotifier
    {
        Task NotifyAsync(string channelId, CancellationToken cancellationToken = default);
    }
}
