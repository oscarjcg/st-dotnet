using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using st_dotnet.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace st_dotnet.Api.Controllers
{
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentRepository commentRepository;
        private readonly IChannelRepository channelRepository;

        public CommentController(ICommentRepository commentRepository, IChannelRepository channelRepository)
        {
            this.commentRepository = commentRepository;
            this.channelRepository = channelRepository;
        }

        [HttpGet]
        public IEnumerable<Comment> Get()
        {
            return commentRepository.All();
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormCollection form)
        {
            commentRepository.Add(new Comment { ChannelId = Int32.Parse(form["channel_id"]), Author = form["author"], Text= form["comment"] });
            return Ok();
        }


        [HttpDelete("{id}")]
        public Comment Delete(int id)
        {
            return commentRepository.Delete(id);
        }

        [HttpDelete("ChannelComments/{id}")]
        public void DeleteChannelComments(int id)
        {
            commentRepository.DeleteChannelComments(id);
        }

        [HttpGet("ChannelComments/{id}")]
        public IEnumerable<Comment> GetChannelComments(int id)
        {
            return commentRepository.GetChannelComments(id);
        }

        [HttpGet("ByChannelName/{name}")]
        public IEnumerable<Comment> GetChannelCommentsByName(string name)
        {
            var channel = channelRepository.GetbyName(name);
            return commentRepository.GetChannelComments(channel.Id);
        }

    }
}

