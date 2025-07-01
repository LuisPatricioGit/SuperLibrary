using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SuperLibrary.Web.Helper;

public interface IBlobHelper
{
    Task<Guid> UploadBlobAsync(IFormFile formFile, string containerName);

    Task<Guid> UploadBlobAsync(byte[] file, string containerName);

    Task<Guid> UploadBlobAsync(string image, string containerName);
}
