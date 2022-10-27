using System;
using Microsoft.EntityFrameworkCore;
using st_dotnet.Models;

namespace st_dotnet.Data
{

    public partial class GalleryDbContext
    {
        public class SqlChannelTypeData : IChannelTypeData
        {
            private readonly GalleryDbContext db;

            public SqlChannelTypeData(GalleryDbContext db)
            {
                this.db = db;
            }

            public ChannelType Add(ChannelType channelType)
            {
                db.channelTypes.Add(channelType);
                db.SaveChanges();
                return channelType;
            }

            public ChannelType Delete(int id)
            {
                var channelType = GetbyId(id);
                if (channelType != null)
                {
                    db.channelTypes.Remove(channelType);
                    db.SaveChanges();
                }
                return channelType;
            }

            public IEnumerable<ChannelType> GetAll()
            {
                return db.channelTypes;
            }

            public ChannelType GetbyId(int id)
            {
                return db.channelTypes.Where(c => c.Id == id)
                        .First();
            }

            public ChannelType GetbyName(string name)
            {
                var query = from c in db.channelTypes
                            where c.Name.StartsWith(name) || string.IsNullOrEmpty(name)
                            orderby c.Name
                            select c;
                return query.First();
            }

            public ChannelType Update(ChannelType channelType)
            {
                var entity = db.channelTypes.Attach(channelType);
                entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();
                return channelType;
            }
        }
    }
}

