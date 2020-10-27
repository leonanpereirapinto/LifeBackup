using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using LifeBackup.Core.Communication.Bucket;
using LifeBackup.Core.Interfaces;

namespace LifeBackup.Infrastructure.Repositories
{
    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _s3Client;

        public BucketRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<bool> DoesS3BucketExist(string bucketName)
        {
            return await _s3Client.DoesS3BucketExistAsync(bucketName);
        }

        public async Task<CreateBucketResponse> CreateBucket(string bucketName)
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            };

            var response = await _s3Client.PutBucketAsync(putBucketRequest);

            return new CreateBucketResponse
            {
                BucketName = bucketName,
                RequestId = response.ResponseMetadata.RequestId
            };
        }
    }
}
