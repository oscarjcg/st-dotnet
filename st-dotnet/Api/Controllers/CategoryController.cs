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
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IAmazonS3 s3Client;

        private const string S3_BUTCKET_NAME = "oscar-catari-s3-dev";
        private const string S3_BUTCKET_FOLDER = "publicdev/category";
        private const string S3_URL = "https://oscar-catari-s3-dev.s3.eu-central-1.amazonaws.com/";

        public CategoryController(ICategoryRepository categoryRepository, IChannelRepository channelRepository, IAmazonS3 s3Client)
        {
            _categoryRepository = categoryRepository;
            _channelRepository = channelRepository;
            this.s3Client = s3Client;
        }

        [HttpGet]
        public IEnumerable<Category> Get()
        {
            return _categoryRepository.AllCategories;
        }

        [HttpGet("{id}")]
        public Category Get(int id)
        {
            return _categoryRepository.GetbyId(id);
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormCollection form)
        {
            var bucketExists = await s3Client.DoesS3BucketExistAsync(S3_BUTCKET_NAME);
            if (!bucketExists) return NotFound($"Bucket {S3_BUTCKET_NAME} does not exist.");

            var imageFile = form.Files.GetFile("image");
            var imageKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";
            if (imageFile == null) return BadRequest();
            await UploadFile(imageFile, imageKey);

            var now = DateTime.Now;
            _categoryRepository.Add(new Category { Name = form["name"], Image = $"{S3_URL}{imageKey}", CreatedAt = now, UpdatedAt = now });
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, IFormCollection form)
        {
            // TODO Review optional image
            var bucketExists = await s3Client.DoesS3BucketExistAsync(S3_BUTCKET_NAME);
            if (!bucketExists) return NotFound($"Bucket {S3_BUTCKET_NAME} does not exist.");

            var category = _categoryRepository.GetbyId(id);
            if (category == null) return BadRequest();

            var imageFile = form.Files.GetFile("image");
            var imageKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";
            if (imageFile == null) return BadRequest();
            await UploadFile(imageFile, imageKey);

            
            category.Name = form["name"];
            category.Image = $"{S3_URL}{imageKey}";
            category.UpdatedAt = DateTime.Now;
            _categoryRepository.Update(category);
            return Ok();
        }

        [HttpDelete("{id}")]
        public Category Delete(int id)
        {
            return _categoryRepository.Delete(id);
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

        [HttpGet("byname/{name}")]
        public Category GetCategoryByName(string name)
        {
            return _categoryRepository.GetbyName(name);
        }

        
        [HttpGet("byname/{name}/channels")]
        public IEnumerable<Channel> GetChannelsByName(string name)
        {
            var category = _categoryRepository.GetbyName(name);
            return _categoryRepository.GetChannels(category.Id).Channels;
        }
        
    }
}

