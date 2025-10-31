using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Api.Data;
using SuperLibrary.Api.Models.API;
using System.Linq;
using System.Threading.Tasks;

namespace SuperLibrary.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
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
            .Cast<SuperLibrary.Api.Data.Entities.Book>()
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

    /// <summary>
    /// Gets a specific book by Id.
    /// </summary>
    /// <param name="id">Book Id</param>
    /// <returns>The book with the specified Id</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookApiViewModel), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetBook(int id)
    {
        var book = _bookRepository.GetAllWithUsers()
            .Cast<SuperLibrary.Api.Data.Entities.Book>()
            .FirstOrDefault(b => b.Id == id);
        if (book == null)
            return NotFound();
        var model = new BookApiViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            ReleaseYear = book.ReleaseYear,
            Copies = book.Copies,
            GenreId = book.GenreId,
            ImageFullPath = book.ImageFullPath,
            IsAvailable = book.IsAvailable,
            UserName = book.User != null ? book.User.UserName : null
        };
        return Ok(model);
    }

    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="model">Book data</param>
    /// <returns>The created book</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Books
    ///     {
    ///         "title": "Sample Book",
    ///         "author": "John Doe",
    ///         "publisher": "Sample Publisher",
    ///         "releaseYear": "2024-01-01T00:00:00Z",
    ///         "copies": 5,
    ///         "genreId": 1,
    ///         "isAvailable": true
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(BookApiViewModel), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostBook([FromBody] BookApiViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var book = new Data.Entities.Book
        {
            Title = model.Title,
            Author = model.Author,
            Publisher = model.Publisher,
            ReleaseYear = model.ReleaseYear,
            Copies = model.Copies,
            GenreId = model.GenreId,
            IsAvailable = model.IsAvailable,
            ImageId = Guid.Empty // or handle image upload logic
        };
        await _bookRepository.CreateAsync(book);
        model.Id = book.Id;
        model.ImageFullPath = book.ImageFullPath;
        return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, model);
    }

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="id">Book Id</param>
    /// <param name="model">Book data</param>
    /// <returns>No content</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/Books/1
    ///     {
    ///         "id": 1,
    ///         "title": "Updated Book",
    ///         "author": "Jane Doe",
    ///         "publisher": "Updated Publisher",
    ///         "releaseYear": "2023-01-01T00:00:00Z",
    ///         "copies": 10,
    ///         "genreId": 2,
    ///         "isAvailable": false
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PutBook(int id, [FromBody] BookApiViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        book.Title = model.Title;
        book.Author = model.Author;
        book.Publisher = model.Publisher;
        book.ReleaseYear = model.ReleaseYear;
        book.Copies = model.Copies;
        book.GenreId = model.GenreId;
        book.IsAvailable = model.IsAvailable;
        // Optionally update ImageId if needed

        await _bookRepository.UpdateAsync(book);
        return NoContent();
    }
}
