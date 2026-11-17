using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using st_dotnet.Models;
using st_dotnet.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace st_dotnet.Api.Controllers
{
    [Route("api/[controller]")]
    public class ChannelController : Controller
    {
        private readonly IChannelRepository channelRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IChannelTypeRepository channelTypeRepository;
        private readonly IAmazonS3 s3Client;
        private readonly S3Settings s3Settings;

        private const string S3_BUTCKET_FOLDER = "public/channel";

        public ChannelController(
            IChannelRepository channelRepository,
            ICategoryRepository categoryRepository,
            IChannelTypeRepository channelTypeRepository,
            IAmazonS3 s3Client,
            IOptions<S3Settings> s3Settings)
        {
            this.channelRepository = channelRepository;
            this.categoryRepository = categoryRepository;
            this.channelTypeRepository = channelTypeRepository;
            this.s3Client = s3Client;
            this.s3Settings = s3Settings.Value;
        }

        [HttpGet]
        public IEnumerable<Channel> Get()
        {
            return channelRepository.All();
        }

        [HttpGet("{id}")]
        public ActionResult<Channel> Get(int id)
        {
            var channel = channelRepository.GetbyId(id);
            if (channel == null) return NotFound();
            return channel;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormCollection form)
        {
            var bucketExists = await s3Client.DoesS3BucketExistAsync(s3Settings.BucketName);
            if (!bucketExists) return NotFound($"Bucket {s3Settings.BucketName} does not exist.");

            if (!int.TryParse(form["channelTypeId"], out var channelTypeId)) return BadRequest();
            var channelType = channelTypeRepository.GetbyId(channelTypeId);
            if (channelType == null) return BadRequest();

            var imageFile = form.Files.GetFile("image");
            var previewFile = form.Files.GetFile("preview");

            var imageKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";
            var previewKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";

            if (imageFile == null || previewFile == null) return BadRequest();
            await UploadFile(imageFile, imageKey);
            await UploadFile(previewFile, previewKey);

            // TODO Channel Content Type
            var now = DateTime.Now;
            channelRepository.Add(new Channel {
                Name = form["name"],
                Image = $"{s3Settings.PublicUrl}{imageKey}",
                Preview = $"{s3Settings.PublicUrl}{previewKey}",
                ChannelTypeId = channelTypeId,
                Content = form["content"],
                CreatedAt = now,
                UpdatedAt = now
            });

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, IFormCollection form)
        {
            var channel = channelRepository.GetbyId(id);
            if (channel == null) return NotFound();
            if (!int.TryParse(form["channelTypeId"], out var channelTypeId)) return BadRequest();

            var bucketExists = await s3Client.DoesS3BucketExistAsync(s3Settings.BucketName);
            if (!bucketExists) return NotFound($"Bucket {s3Settings.BucketName} does not exist.");

            var imageFile = form.Files.GetFile("image");
            var previewFile = form.Files.GetFile("preview");

            var imageKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";
            var previewKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";

            if (imageFile != null) {
                await UploadFile(imageFile, imageKey);
                channel.Image = $"{s3Settings.PublicUrl}{imageKey}";
            }

            if (previewFile != null)
            {
                await UploadFile(previewFile, previewKey);
                channel.Preview = $"{s3Settings.PublicUrl}{previewKey}";
            }

            channel.Name = form["name"];
            channel.ChannelTypeId = channelTypeId;
            channel.Content = form["content"];
            channel.UpdatedAt = DateTime.Now;

            channelRepository.Update(channel);
            return Ok(channel);
        }

        [HttpDelete("{id}")]
        public ActionResult<Channel> Delete(int id)
        {
            var channel = channelRepository.Delete(id);
            if (channel == null) return NotFound();
            return channel;
        }

        private async Task<PutObjectResponse> UploadFile(IFormFile imageFile, string imageKey)
        {
            var request = new PutObjectRequest()
            {
                BucketName = s3Settings.BucketName,
                Key = imageKey,
                InputStream = imageFile.OpenReadStream(),
                CannedACL = S3CannedACL.PublicRead
            };
            request.Metadata.Add("Content-Type", imageFile.ContentType);
            return await s3Client.PutObjectAsync(request);
        }

        [HttpGet("byname/{name}")]
        public ActionResult<Channel> GetChannelByName(string name)
        {
            var channel = channelRepository.GetbyName(name);
            if (channel == null) return NotFound();
            return channel;
        }

        [HttpPut("{id}/addcategory/{category_id}")]
        public IActionResult AddCategory(int id, int category_id)
        {
            var channel = channelRepository.GetbyId(id);
            if (channel == null) return NotFound();
            var category = categoryRepository.GetbyId(category_id);
            if (category == null) return NotFound();
            if (channel.Categories == null)
                channel.Categories = new List<Category>();
            channel.Categories.Add(category);
            channelRepository.Update(channel);
            return Ok(channel);
        }
    }
}
