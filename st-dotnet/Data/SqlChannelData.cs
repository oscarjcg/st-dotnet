using System;
using st_dotnet.Models;

namespace st_dotnet.Data
{
    public partial class GalleryDbContext
    {
        public class SqlChannelData : IChannelData
        {
            private readonly GalleryDbContext db;

            public SqlChannelData(GalleryDbContext db)
            {
                this.db = db;
            }

            public Channel Add(Channel newChannel)
            {
                db.channels.Add(newChannel);
                db.SaveChanges();
                return newChannel;
            }

            public Channel Delete(int id)
            {
                var channel = db.channels.Find(id);
                db.channels.Remove(channel);
                db.SaveChanges();
                return channel;
            }

            public IEnumerable<Channel> GetAll()
            {
                return db.channels;
            }

            public Channel GetbyId(int id)
            {
                return db.channels.Find(id);
            }

            public Channel GetbyName(string name)
            {
                var query = from r in db.channels
                            where r.Name == name
                            select r;
                return query.First();
            }

            public IEnumerable<Channel> Search(string name)
            {
                var query = from r in db.channels
                            where r.Name.Contains(name)
                            select r;
                return query;
            }

            public Channel Update(Channel updatedChannel)
            {
                var entity = db.channels.Attach(updatedChannel);
                entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();
                return updatedChannel;
            }
        }
    }
}

