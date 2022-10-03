using System;
using st_dotnet.Models;

namespace st_dotnet.Data
{
    public interface ICommentData
    {
        Comment Add(Comment newComment);
        Comment Delete(int id);
        IEnumerable<Comment> GetAll();
        void DeleteChannelComments(int id);
    }
}
 
