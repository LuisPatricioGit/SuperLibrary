using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Web.Data;
using SuperLibrary.Web.Models.API;
using System.Linq;

namespace SuperLibrary.Web.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public IActionResult GetBooks()
    {
        var books = _bookRepository.GetAllWithUsers()
            .Cast<SuperLibrary.Web.Data.Entities.Book>()
            .Select(b => new BookApiViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Publisher = b.Publisher,
                ReleaseYear = b.ReleaseYear,
                Copies = b.Copies,
                GenreId = b.GenreId,
                ImageFullPath = b.ImageFullPath,
                IsAvailable = b.IsAvailable,
                UserName = b.User != null ? b.User.UserName : null
            }).ToList();
        return Ok(books);
    }
}
