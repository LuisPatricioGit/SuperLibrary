using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SuperLibrary.Api.Models;

public class UploadImageViewModel
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public IFormFile ImageFile { get; set; }
}
