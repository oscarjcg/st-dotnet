using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using st_dotnet.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace st_dotnet.Api.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public IEnumerable<Category> Get()
        {
            return _categoryRepository.AllCategories;
        }

        [HttpGet("{id}")]
        public Category Get(int id)
        {
            return _categoryRepository.GetbyId(id);
        }

        [HttpPost]
        public void Post(IFormCollection form)
        {
            _categoryRepository.Add(new Category { Name = form["name"], Image = "sss" });
        }

        [HttpPut("{id}")]
        public void Put(int id, IFormCollection form)
        {
            var category = new Category { Id = id, Name = form["name"], Image = "s" };
            _categoryRepository.Update(category);
            Ok();
        }

        [HttpDelete("{id}")]
        public Category Delete(int id)
        {
            return _categoryRepository.Delete(id);
        }
    }
}

