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
    public class ChannelTypeController : Controller
    {

        private readonly IChannelTypeRepository channelTypeRepository;

        public ChannelTypeController(IChannelTypeRepository channelTypeRepository)
        {
            this.channelTypeRepository = channelTypeRepository;
        }

        [HttpGet]
        public IEnumerable<ChannelType> Get()
        {
            return channelTypeRepository.GetAll();
        }

        [HttpGet("{id}")]
        public ChannelType Get(int id)
        {
            return channelTypeRepository.GetbyId(id);
        }

        [HttpPost]
        public void Post(IFormCollection form)
        {
            var channelType = new ChannelType { Name = form["name"] };
            channelTypeRepository.Add(channelType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, IFormCollection form)
        {
            var channelType = channelTypeRepository.GetbyId(id);
            if (channelType == null) return BadRequest();
            channelType.Name = form["name"];

            channelTypeRepository.Update(channelType);
            return Ok(channelType);
        }

        [HttpDelete("{id}")]
        public ChannelType Delete(int id)
        {
            return channelTypeRepository.Delete(id);
        }
    }
}

