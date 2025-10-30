using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SuperLibrary.Web.Helper;

public interface IImageHelper
{
    Task<string> UploadImageAsync(System.IO.Stream imageStream, string fileName, string folder);
}
