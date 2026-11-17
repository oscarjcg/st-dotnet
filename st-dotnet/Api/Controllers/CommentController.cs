using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using st_dotnet.Models;
using st_dotnet.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace st_dotnet.Api.Controllers
{
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentRepository commentRepository;
        private readonly IChannelRepository channelRepository;
        private readonly INewMessageNotifier newMessageNotifier;

        public CommentController(ICommentRepository commentRepository, IChannelRepository channelRepository, INewMessageNotifier newMessageNotifier)
        {
            this.commentRepository = commentRepository;
            this.channelRepository = channelRepository;
            this.newMessageNotifier = newMessageNotifier;
        }

        [HttpGet]
        public IEnumerable<Comment> Get()
        {
            return commentRepository.All();
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormCollection form)
        {
            if (!int.TryParse(form["channel_id"], out var channelId)) return BadRequest();

            var now = DateTime.Now;
            commentRepository.Add(
                new Comment {
                    ChannelId = channelId,
                    Author = form["author"],
                    Text= form["comment"],
                    CreatedAt = now,
                    UpdatedAt = now
                });

            await newMessageNotifier.NotifyAsync(channelId.ToString(), HttpContext?.RequestAborted ?? default);

            return Ok();
        }


        [HttpDelete("{id}")]
        public ActionResult<Comment> Delete(int id)
        {
            var comment = commentRepository.Delete(id);
            if (comment == null) return NotFound();
            return comment;
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
        public ActionResult<IEnumerable<Comment>> GetChannelCommentsByName(string name)
        {
            var channel = channelRepository.GetbyName(name);
            if (channel == null) return NotFound();
            return Ok(commentRepository.GetChannelComments(channel.Id));
        }

    }
}
