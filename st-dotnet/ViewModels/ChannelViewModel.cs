using System;
namespace st_dotnet.ViewModels
{
    public class ChannelViewModel
    {
        public IEnumerable<Models.Channel> Channels { get; set; }
        public IEnumerable<Models.ChannelType> ChannelTypes { get; set; }
        public Models.Channel Channel;
    }
}

