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
        public ActionResult<ChannelType> Get(int id)
        {
            var channelType = channelTypeRepository.GetbyId(id);
            if (channelType == null) return NotFound();
            return channelType;
        }

        [HttpPost]
        public void Post(IFormCollection form)
        {
            var now = DateTime.Now;
            var channelType = new ChannelType { Name = form["name"], CreatedAt = now, UpdatedAt = now };
            channelTypeRepository.Add(channelType);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, IFormCollection form)
        {
            var channelType = channelTypeRepository.GetbyId(id);
            if (channelType == null) return NotFound();
            channelType.Name = form["name"];
            channelType.UpdatedAt = DateTime.Now;

            channelTypeRepository.Update(channelType);
            return Ok(channelType);
        }

        [HttpDelete("{id}")]
        public ActionResult<ChannelType> Delete(int id)
        {
            var channelType = channelTypeRepository.Delete(id);
            if (channelType == null) return NotFound();
            return channelType;
        }
    }
}
