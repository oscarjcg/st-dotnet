using System;
using st_dotnet.Data;

namespace st_dotnet.Models
{
    public class ChannelRepository: IChannelRepository
    {
        private readonly IChannelData channelData;

        public ChannelRepository(IChannelData channelData)
        {
            this.channelData = channelData;
        }

        public Channel Add(Channel newChannel)
        {
            return channelData.Add(newChannel);
        }

        public IEnumerable<Channel> All()
        {
            return channelData.GetAll();
        }

        public Channel Delete(int id)
        {
            return channelData.Delete(id);
        }

        public Channel GetbyId(int id)
        {
            return channelData.GetbyId(id);
        }

        public Channel GetbyName(string name)
        {
            return channelData.GetbyName(name);
        }

        public Channel Update(Channel updatedChannel)
        {
            return channelData.Update(updatedChannel);
        }
    }
}

