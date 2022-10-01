using System;
namespace st_dotnet.Models
{
    public interface IChannelRepository
    {
        IEnumerable<Channel> All();
        Channel Add(Channel newChannel);
        Channel Update(Channel updatedChannel);
        Channel Delete(int id);
        Channel GetbyId(int id);
        Channel GetbyName(string name);
    }
}