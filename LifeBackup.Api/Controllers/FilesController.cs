using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


    }
}