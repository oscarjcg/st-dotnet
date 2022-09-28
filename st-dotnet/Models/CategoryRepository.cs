using System;
using st_dotnet.Data;

namespace st_dotnet.Models
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ICategoryData categoryData;

        public CategoryRepository(ICategoryData categoryData)
        {
            this.categoryData = categoryData;
        }

        public IEnumerable<Category> AllCategories => categoryData.GetAll();

        public Category Add(Category newCategory)
        {
            categoryData.Add(newCategory);
            categoryData.Commit();

            return newCategory;
        }

        public Category Delete(int id)
        {
            var category = categoryData.Delete(id);
            categoryData.Commit();
            return category;
        }

        public IEnumerable<Category> GetAll()
        {
            throw new NotImplementedException();
        }

        public Category GetbyId(int id)
        {
            return categoryData.GetbyId(id);
        }

        public Category GetbyName(string name)
        {
            return categoryData.GetbyName(name);
        }

        public Category Update(Category updatedCategory)
        {
            categoryData.Update(updatedCategory);
            categoryData.Commit();
            return updatedCategory;
        }
    }
}

