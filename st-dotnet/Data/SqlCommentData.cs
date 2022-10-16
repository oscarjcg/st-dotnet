using System;
using st_dotnet.Models;

namespace st_dotnet.Data
{
    public partial class GalleryDbContext
    {
        public class SqlCommentData : ICommentData
        {
            private readonly GalleryDbContext db;

            public SqlCommentData(GalleryDbContext db)
            {
                this.db = db;
            }

            public Comment Add(Comment newComment)
            {
                db.comments.Add(newComment);
                db.SaveChanges();
                return newComment;
            }

            public void DeleteChannelComments(int id)
            {
                // TODO Review execution time
                var query = from r in db.comments
                            where r.ChannelId == id
                            select r;
                foreach(var comment in query)
                {
                    db.comments.Remove(comment);
                }
                db.SaveChanges();
            }

            Comment ICommentData.Delete(int id)
            {
                var comment = db.comments.Find(id);
                db.comments.Remove(comment);
                db.SaveChanges();
                return comment;
            }

            IEnumerable<Comment> ICommentData.GetAll()
            {
                return db.comments;
            }

            public IEnumerable<Comment> GetChannelComments(int id)
            {
                var query = from r in db.comments
                            where r.ChannelId == id
                            select r;
                return query;
            }
        }
    }
}

