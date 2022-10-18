using System;
namespace st_dotnet.Models
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> AllCategories { get; }
        Category Add(Category newCategory);
        Category Update(Category updatedCategory);
        Category Delete(int id);
        IEnumerable<Category> GetAll();
        Category GetbyId(int id);
        Category GetbyName(string name);
        Category GetChannels(int id);
    }
}

