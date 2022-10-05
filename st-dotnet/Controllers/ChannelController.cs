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
    public class ChannelController : Controller
    {
        private readonly IChannelRepository channelRepository;

        public ChannelController(IChannelRepository channelRepository)
        {
            this.channelRepository = channelRepository;
        }

        public IActionResult Index()
        {
            return View(new ChannelViewModel
            {
                Channels = channelRepository.All()
            });
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var channel = channelRepository.GetbyId(id);
            return View(channel);
        }
    }
}

