using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SuperLibrary.Web.Helper;

public interface IImageHelper
{
    Task<string> UploadImageAsync(IFormFile ImageFile, string folder);
}
