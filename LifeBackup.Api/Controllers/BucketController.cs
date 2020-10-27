using System.Threading.Tasks;
using LifeBackup.Core.Communication.Bucket;
using LifeBackup.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LifeBackup.Api.Controllers
{
    [Route("api/bucket")]
    [ApiController]
    public class BucketController : ControllerBase
    {
        private readonly IBucketRepository _bucketRepository;

        public BucketController(IBucketRepository bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }

        [HttpPost]
        [Route("create/{bucketName}")]
        public async Task<ActionResult<CreateBucketResponse>> CreateS3Bucket([FromRoute] string bucketName)
        {
            var bucketExists = await _bucketRepository.DoesS3BucketExist(bucketName);

            if (bucketExists)
            {
                return BadRequest("S3 bucket already exists");
            }

            var result = await _bucketRepository.CreateBucket(bucketName);


            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}