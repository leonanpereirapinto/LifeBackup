﻿using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using LifeBackup.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LifeBackup.Core.Communication.Files;
using Newtonsoft.Json;
using Xunit;

namespace LifeBackup.Integration.Tests.Scenarios
{
    [Collection("api")]
    public class FilesControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public FilesControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAWSService<IAmazonS3>(new AWSOptions
                    {
                        DefaultClientConfig =
                        {
                            ServiceURL = "http://localhost:9003"
                        },
                        Credentials = new BasicAWSCredentials("FAKE", "FAKE")
                    });
                });
            }).CreateClient();

            Task.Run(CreateBucket).Wait();
        }

        private async Task CreateBucket()
        {
            await _client.PostAsJsonAsync("api/bucket/create/testS3Bucket","testS3Bucket");
        }

        [Fact]
        public async Task When_AddFiles_endpoint_is_hit_we_are_returned_ok_status()
        {
            var response = await UploadFileToS3Bucket();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> UploadFileToS3Bucket()
        {
            const string path = @"c:\temp\IntegrationTest.jpg";

            var file = File.Create(path);

            HttpContent fileStreamContent = new StreamContent(file);

            var formData = new MultipartFormDataContent
            {
                { fileStreamContent, "formFiles", "IntegrationTest.jpg" }
            };

            var response = await _client.PostAsync("api/files/testS3Bucket/add", formData);

            fileStreamContent.Dispose();
            formData.Dispose();

            return response;
        }
        
        [Fact]
        public async Task When_ListFiles_endpoint_is_hit_our_result_is_not_null()
        {
            await UploadFileToS3Bucket();

            var response = await _client.GetAsync("api/files/testS3Bucket/list");

            ListFilesResponse[] result;

            using (var content = response.Content.ReadAsStringAsync())
            {
                result = JsonConvert.DeserializeObject<ListFilesResponse[]>(await content);
            }
            
            Assert.NotNull(result);
        }
    }
}
