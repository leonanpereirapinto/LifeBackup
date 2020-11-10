using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeBackup.Core.Communication.Files;
using LifeBackup.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LifeBackup.Api.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFilesRepository _filesRepository;

        public FilesController(IFilesRepository filesRepository)
        {
            _filesRepository = filesRepository;
        }

        [HttpPost]
        [Route("{bucketName}/add")]
        public async Task<ActionResult<AddFileResponse>> AddFiles(string bucketName, IList<IFormFile> formFiles)
        {
            if (formFiles == null)
            {
                return BadRequest("The request doesn't contain any files to be uploaded");
            }

            var response = await _filesRepository.UploadFiles(bucketName, formFiles);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("{bucketName}/list")]
        public async Task<ActionResult<IEnumerable<ListFilesResponse>>> ListFiles(string bucketName)
        {
            var response = await _filesRepository.ListFiles(bucketName);

            return Ok(response);
        }
    }
}