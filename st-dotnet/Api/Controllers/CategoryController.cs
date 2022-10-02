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
        private readonly IAmazonS3 s3Client;

        private const string S3_BUTCKET_NAME = "oscar-catari-s3-dev";
        private const string S3_BUTCKET_FOLDER = "publicdev/category";

        public CategoryController(ICategoryRepository categoryRepository, IAmazonS3 s3Client)
        {
            _categoryRepository = categoryRepository;
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

            _categoryRepository.Add(new Category { Name = form["name"], Image = imageKey });
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, IFormCollection form)
        {
            var bucketExists = await s3Client.DoesS3BucketExistAsync(S3_BUTCKET_NAME);
            if (!bucketExists) return NotFound($"Bucket {S3_BUTCKET_NAME} does not exist.");

            var imageFile = form.Files.GetFile("image");
            var imageKey = $"{S3_BUTCKET_FOLDER}/{Guid.NewGuid().ToString()}";
            if (imageFile == null) return BadRequest();
            await UploadFile(imageFile, imageKey);

            var category = new Category { Id = id, Name = form["name"], Image = imageKey };
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
    }
}

