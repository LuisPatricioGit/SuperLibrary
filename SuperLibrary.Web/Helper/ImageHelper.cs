using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SuperLibrary.Web.Helper;

public class ImageHelper : IImageHelper
{
    /// <summary>
    /// Uploads Image to specified path without replacing same name ones.
    /// </summary>
    /// <param name="imageFile"></param>
    /// <param name="folder"></param>
    /// <returns>Image Path</returns>
    public async Task<string> UploadImageAsync(IFormFile imageFile, string folder)
    {
        string guid = Guid.NewGuid().ToString();
        string file = $"{guid}.jpg";

        string path = Path.Combine(Directory.GetCurrentDirectory(),
                            $"wwwroot\\images\\{folder}",
                            file);

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return $"~/images/{folder}/{file}";
    }
}
