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
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IAmazonS3 s3Client;
        private readonly S3Settings s3Settings;

        private const string S3_BUTCKET_FOLDER = "public/category";

        public CategoryController(ICategoryRepository categoryRepository, IChannelRepository channelRepository, IAmazonS3 s3Client, IOptions<S3Settings> s3Settings)
        {
            _categoryRepository = categoryRepository;
            _channelRepository = channelRepository;
            this.s3Client = s3Client;
            this.s3Settings = s3Settings.Value;
        }

        [HttpGet]
        public IEnumerable<Category> Get()
        {
            return _categoryRepository.AllCategories;
        }

        [HttpGet("{id}")]
        public ActionResult<Category> Get(int id)
        {
            var category = _categoryRepository.GetbyId(id);
            if (category == null) return NotFound();
            return category;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormCollection form)
        {
            var bucketExists = await s3Client.DoesS3BucketExistAsync(s3Settings.BucketName);
            if (!bucketExists) return NotFound($"Bucket {s3Settings.BucketName} does not exist.");

            var imageFile = form.Files.GetFile("image");
            var imageKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";
            if (imageFile == null) return BadRequest();
            await UploadFile(imageFile, imageKey);

            var now = DateTime.Now;
            _categoryRepository.Add(new Category { Name = form["name"], Image = $"{s3Settings.PublicUrl}{imageKey}", CreatedAt = now, UpdatedAt = now });
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, IFormCollection form)
        {
            // TODO Review optional image
            var bucketExists = await s3Client.DoesS3BucketExistAsync(s3Settings.BucketName);
            if (!bucketExists) return NotFound($"Bucket {s3Settings.BucketName} does not exist.");

            var category = _categoryRepository.GetbyId(id);
            if (category == null) return NotFound();

            var imageFile = form.Files.GetFile("image");
            var imageKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";
            if (imageFile == null) return BadRequest();
            await UploadFile(imageFile, imageKey);


            category.Name = form["name"];
            category.Image = $"{s3Settings.PublicUrl}{imageKey}";
            category.UpdatedAt = DateTime.Now;
            _categoryRepository.Update(category);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Category> Delete(int id)
        {
            var category = _categoryRepository.Delete(id);
            if (category == null) return NotFound();
            return category;
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
        public ActionResult<Category> GetCategoryByName(string name)
        {
            var category = _categoryRepository.GetbyName(name);
            if (category == null) return NotFound();
            return category;
        }


        [HttpGet("byname/{name}/channels")]
        public ActionResult<IEnumerable<Channel>> GetChannelsByName(string name)
        {
            var category = _categoryRepository.GetbyName(name);
            if (category == null) return NotFound();
            var withChannels = _categoryRepository.GetChannels(category.Id);
            return Ok(withChannels?.Channels ?? new List<Channel>());
        }

    }
}
