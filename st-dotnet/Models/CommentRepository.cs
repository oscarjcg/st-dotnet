using System;
using st_dotnet.Data;

namespace st_dotnet.Models
{
    public class CommentRepository: ICommentRepository
    {
        private readonly ICommentData commentData;

        public CommentRepository(ICommentData commentData)
        {
            this.commentData = commentData;
        }

        public Comment Add(Comment newComment)
        {
            return commentData.Add(newComment);
        }

        public IEnumerable<Comment> All()
        {
            return commentData.GetAll();
        }

        public Comment Delete(int id)
        {
            return commentData.Delete(id);
        }

        public void DeleteChannelComments(int id)
        {
            commentData.DeleteChannelComments(id);
        }
    }
}