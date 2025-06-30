using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Web.Data;

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
        return Ok(_bookRepository.GetAllWithUsers());
    }
}
