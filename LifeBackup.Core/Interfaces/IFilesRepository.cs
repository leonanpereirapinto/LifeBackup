using System.Collections.Generic;
using System.Threading.Tasks;
using LifeBackup.Core.Communication.Files;
using Microsoft.AspNetCore.Http;

namespace LifeBackup.Core.Interfaces
{
    public interface IFilesRepository
    {
        Task<AddFileResponse> UploadFiles(string bucketName, IList<IFormFile> formFiles);
    }
}