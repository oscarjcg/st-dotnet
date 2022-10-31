using System;
using st_dotnet.Models;

namespace st_dotnet.Data
{
    public interface ICategoryData
    {
        int Commit();
        Category Add(Category newCategory);
        Category Update(Category updatedCategory);
        Category Delete(int id);
        IEnumerable<Category> GetAll();
        Category GetbyId(int id);
        Category GetbyName(string name);
        IEnumerable<Category> Search(string name);
    }
}
 
