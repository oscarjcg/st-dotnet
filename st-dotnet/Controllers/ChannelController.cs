using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
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
        private readonly IChannelTypeRepository channelTypeRepository;

        public ChannelController(IChannelRepository channelRepository, IChannelTypeRepository channelTypeRepository)
        {
            this.channelRepository = channelRepository;
            this.channelTypeRepository = channelTypeRepository;
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
            var channelTypes = channelTypeRepository.GetAll();
            return View(new ChannelViewModel
            {
                ChannelTypes = channelTypes
            });
        }

        public IActionResult Edit(int id)
        {
            var channelTypes = channelTypeRepository.GetAll();
            var channel = channelRepository.GetbyId(id);
            return View(new ChannelViewModel
            {
                Channel = channel,
                ChannelTypes = channelTypes
            });
        }
    }
}

