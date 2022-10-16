using System;
namespace st_dotnet.Models
{
    public interface ICommentRepository
    {
        IEnumerable<Comment> All();
        Comment Add(Comment newComment);
        Comment Delete(int id);
        void DeleteChannelComments(int id);
        IEnumerable<Comment> GetChannelComments(int id);
    }
}