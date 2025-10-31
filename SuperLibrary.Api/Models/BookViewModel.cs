using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SuperLibrary.Api.Data.Entities;

namespace SuperLibrary.Api.Models;

public class BookViewModel : Book
{
    [Display(Name = "Image")]
    public IFormFile ImageFile { get; set; }
}
