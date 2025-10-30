using System.IO;
using System;
using System.Threading.Tasks;

namespace SuperLibrary.Web.Helper;

public class ImageHelper : IImageHelper
{
    public async Task<string> UploadImageAsync(System.IO.Stream imageStream, string fileName, string folder)
    {
        string guid = Guid.NewGuid().ToString();
        string ext = Path.GetExtension(fileName);
        string file = $"{guid}{ext}";
        string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folder);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        string path = Path.Combine(directory, file);
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await imageStream.CopyToAsync(stream);
        }
        return $"/images/{folder}/{file}";
    }
}
