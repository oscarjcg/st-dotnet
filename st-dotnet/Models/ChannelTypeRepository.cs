using System;
using st_dotnet.Data;

namespace st_dotnet.Models
{
    public class ChannelTypeRepository : IChannelTypeRepository
    {
        private readonly IChannelTypeData channelTypeData;

        public ChannelTypeRepository(IChannelTypeData channelTypeData)
        {
            this.channelTypeData = channelTypeData;
        }

        public ChannelType Add(ChannelType channelType)
        {
            channelTypeData.Add(channelType);

            return channelType;
        }

        public ChannelType Delete(int id)
        {
            var  channelType = channelTypeData.Delete(id);
            return channelType;
        }

        public IEnumerable<ChannelType> GetAll()
        {
            return channelTypeData.GetAll();
        }

        public ChannelType GetbyId(int id)
        {
            return channelTypeData.GetbyId(id);
        }

        public ChannelType GetbyName(string name)
        {
            return channelTypeData.GetbyName(name);
        }

        public ChannelType Update(ChannelType channelType)
        {
            channelTypeData.Update(channelType);
            return channelType;
        }
    }
}

