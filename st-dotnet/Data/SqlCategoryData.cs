using System;
using st_dotnet.Models;

namespace st_dotnet.Data
{

    public partial class GalleryDbContext
    {
        public class SqlCategoryData : ICategoryData
        {
            private readonly GalleryDbContext db;

            public SqlCategoryData(GalleryDbContext db)
            {
                this.db = db;
            }

            public Category Add(Category newCategory)
            {
                db.categories.Add(newCategory);
                return newCategory;
            }
            public int Commit()
            {
                return db.SaveChanges();
            }

            public Category Delete(int id)
            {
                var category = GetbyId(id);
                if (category != null)
                {
                    db.categories.Remove(category);
                }
                return category;
            }

            public IEnumerable<Category> GetAll()
            {
                return db.categories;
            }

            public Category GetbyId(int id)
            {
                return db.categories.Find(id);
            }

            public Category GetbyName(string name)
            {
                var query = from c in db.categories
                            where c.Name.StartsWith(name) || string.IsNullOrEmpty(name)
                            orderby c.Name
                            select c;
                return (Category)query;
            }

            public Category Update(Category updatedCategory)
            {
                var entity = db.categories.Attach(updatedCategory);
                entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return updatedCategory;
            }
        }
    }
}

