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
    public class ChannelController : Controller
    {
        private readonly IChannelRepository channelRepository;

        public ChannelController(IChannelRepository channelRepository)
        {
            this.channelRepository = channelRepository;
        }

        [HttpGet]
        public IEnumerable<Channel> Get()
        {
            return channelRepository.All();
        }

        [HttpGet("{id}")]
        public Channel Get(int id)
        {
            return channelRepository.GetbyId(id);
        }

        [HttpPost]
        public void Post(IFormCollection form)
        {
            channelRepository.Add(new Channel { Name = form["name"], Image = "i", Preview ="p", Type=1, Content="c" });
        }

        [HttpPut("{id}")]
        public void Put(int id, IFormCollection form)
        {
            var channel = new Channel { Id = id, Name = form["name"], Image = "s", Preview = "p", Type = 1, Content = "c" };
            channelRepository.Update(channel);
            Ok();
        }

        [HttpDelete("{id}")]
        public Channel Delete(int id)
        {
            return channelRepository.Delete(id);
        }
    }
}

