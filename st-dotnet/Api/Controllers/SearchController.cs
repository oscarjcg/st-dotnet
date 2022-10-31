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
    public class SearchController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IChannelRepository channelRepository;

        public SearchController(ICategoryRepository categoryRepository, IChannelRepository channelRepository)
        {
            this.categoryRepository = categoryRepository;
            this.channelRepository = channelRepository;
        }

        [HttpGet("{search}")]
        public async Task<IActionResult> Get(string search)
        {
            var searchResult = new SearchResult
            {
                Channels = channelRepository.Search(search),
                Categories = categoryRepository.Search(search)
            };
            return Ok(searchResult);
        }
    }
}

