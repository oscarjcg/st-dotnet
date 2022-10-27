using System;
namespace st_dotnet.Models
{
    public interface IChannelTypeRepository
    {
        ChannelType Add(ChannelType channelType);
        ChannelType Update(ChannelType channelType);
        ChannelType Delete(int id);
        IEnumerable<ChannelType> GetAll();
        ChannelType GetbyId(int id);
        ChannelType GetbyName(string name);
    }
}

