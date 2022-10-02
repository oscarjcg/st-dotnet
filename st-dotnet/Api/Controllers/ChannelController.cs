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
    public class ChannelController : Controller
    {
        private readonly IChannelRepository channelRepository;
        private readonly IAmazonS3 s3Client;

        private const string S3_BUTCKET_NAME = "oscar-catari-s3-dev";
        private const string S3_BUTCKET_FOLDER = "publicdev/channel";

        public ChannelController(IChannelRepository channelRepository, IAmazonS3 s3Client)
        {
            this.channelRepository = channelRepository;
            this.s3Client = s3Client;
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
        public async Task<IActionResult> Post(IFormCollection form)
        {
            var bucketExists = await s3Client.DoesS3BucketExistAsync(S3_BUTCKET_NAME);
            if (!bucketExists) return NotFound($"Bucket {S3_BUTCKET_NAME} does not exist.");

            var imageFile = form.Files.GetFile("image");
            var previewFile = form.Files.GetFile("preview");

            var imageKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";
            var previewKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";

            if (imageFile == null || previewFile == null) return BadRequest();
            await UploadFile(imageFile, imageKey);
            await UploadFile(previewFile, previewKey);

            channelRepository.Add(new Channel { Name = form["name"], Image = imageKey, Preview = previewKey, Type = 1, Content = "c" });

            return Ok();
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

        private async Task<PutObjectResponse> UploadFile(IFormFile imageFile, string imageKey)
        {
            var request = new PutObjectRequest()
            {
                BucketName = S3_BUTCKET_NAME,
                Key = imageKey,
                InputStream = imageFile.OpenReadStream(),
                CannedACL = S3CannedACL.PublicRead
            };
            request.Metadata.Add("Content-Type", imageFile.ContentType);
            return await s3Client.PutObjectAsync(request);
        }
    }
}

