using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using st_dotnet.Models;
using st_dotnet.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace st_dotnet.Controllers
{
    [Authorize]
    public class ChannelTypeController : Controller
    {
        private readonly IChannelTypeRepository channelTypeRepository;

        public ChannelTypeController(IChannelTypeRepository channelTypeRepository)
        {
            this.channelTypeRepository = channelTypeRepository;
        }

        public IActionResult Index()
        {
            return View(new ChannelTypeViewModel
            {
                ChannelTypes = channelTypeRepository.GetAll()
            });
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var channel = channelTypeRepository.GetbyId(id);
            return View(channel);
        }
    }
}

