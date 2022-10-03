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

        public CommentController(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
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

        // TODO Not working
        [HttpDelete("{id}")]
        public void DeleteChannelComments(int id)
        {
            commentRepository.DeleteChannelComments(id);
        }

    }
}

