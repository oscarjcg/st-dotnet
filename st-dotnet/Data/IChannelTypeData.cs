using System;
using st_dotnet.Models;

namespace st_dotnet.Data
{
    public interface IChannelTypeData
    {
        ChannelType Add(ChannelType channelType);
        ChannelType Update(ChannelType channelType);
        ChannelType Delete(int id);
        IEnumerable<ChannelType> GetAll();
        ChannelType GetbyId(int id);
        ChannelType GetbyName(string name);
    }
}
 
