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
    public class CommentController : Controller
    {
        private readonly ICommentRepository commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public IActionResult Index()
        {
            return View(new CommentViewModel
            {
                Comments = commentRepository.All()
            });
        }
    }
}