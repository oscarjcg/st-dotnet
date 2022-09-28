using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using st_dotnet.Models;
using st_dotnet.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace st_dotnet.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            return View(new CategoryViewModel
            {
                Categories = _categoryRepository.AllCategories
            });
        }

        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetbyId(id);
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}

