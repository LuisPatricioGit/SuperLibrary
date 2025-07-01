using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
namespace SuperLibrary.Web.Helper;

public class BlobHelper : IBlobHelper
{
    private readonly CloudBlobClient _blobClient;

    public BlobHelper(IConfiguration configuration)
    {
        string keys = configuration["Blob:ConnectionString"];
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
        _blobClient = storageAccount.CreateCloudBlobClient();
    }

    /// <summary>
    /// Uploads a blob via formFile (Most Commonly used)
    /// </summary>
    /// <param name="formFile"></param>
    /// <param name="containerName"></param>
    /// <returns></returns>
    public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
    {
        Stream stream = file.OpenReadStream();
        return await UploadStreamAsync(stream, containerName);
    }

    /// <summary>
    /// Uploads a Blob via byte array (Commonly used for Mobile)
    /// </summary>
    /// <param name="file"></param>
    /// <param name="containerName"></param>
    /// <returns></returns>
    public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
    {
        MemoryStream stream = new MemoryStream(file);
        return await UploadStreamAsync(stream, containerName);
    }

    /// <summary>
    /// Uploads a blob via string (Commonly used has an URL)
    /// </summary>
    /// <param name="image"></param>
    /// <param name="containerName"></param>
    /// <returns></returns>
    public async Task<Guid> UploadBlobAsync(string image, string containerName)
    {
        Stream stream = File.OpenRead(image);
        return await UploadStreamAsync(stream, containerName);
    }

    /// <summary>
    /// Uploads the Stream to container
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="containerName"></param>
    /// <returns>Guid name</returns>
    private async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
    {
        Guid name = Guid.NewGuid();
        CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
        CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
        await blockBlob.UploadFromStreamAsync(stream);
        return name;
    }
}
