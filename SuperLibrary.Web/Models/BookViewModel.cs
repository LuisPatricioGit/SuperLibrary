using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Models;

public class BookViewModel : Book
{
    [Display(Name = "Image")]
    public IFormFile ImageFile { get; set; }
}
