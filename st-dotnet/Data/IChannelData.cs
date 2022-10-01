using System;
using st_dotnet.Models;

namespace st_dotnet.Data
{
    public interface IChannelData
    {
        Channel Add(Channel newChannel);
        Channel Update(Channel updatedChannel);
        Channel Delete(int id);
        IEnumerable<Channel> GetAll();
        Channel GetbyId(int id);
        Channel GetbyName(string name);
    }
}

