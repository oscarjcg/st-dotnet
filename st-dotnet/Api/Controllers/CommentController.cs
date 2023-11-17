using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            var now = DateTime.Now;
            commentRepository.Add(
                new Comment {
                    ChannelId = Int32.Parse(form["channel_id"]),
                    Author = form["author"],
                    Text= form["comment"],
                    CreatedAt = now,
                    UpdatedAt = now
                });

            var nodeRequest = new NewMessageRequest();
            nodeRequest.channel_id = form["channel_id"];
            var json = JsonConvert.SerializeObject(nodeRequest);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://st-node.codename-project.com/new-message";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

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

