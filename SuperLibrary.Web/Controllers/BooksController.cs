using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Web.Data;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Helper;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Controllers;

public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;
    private readonly IUserHelper _userHelper;

    public BooksController(IBookRepository bookRepository, IUserHelper userHelper)
    {
        _bookRepository = bookRepository;
        _userHelper = userHelper;
    }

    // GET: Books
    public IActionResult Index()
    {
        return View(_bookRepository.GetAll().OrderBy(b => b.Title));
    }

    // GET: Books/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _bookRepository.GetByIdAsync(id.Value);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: Books/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Books/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookViewModel model)
    {
        if (ModelState.IsValid)
        {
            var path = string.Empty;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";

                path = Path.Combine(Directory.GetCurrentDirectory(), 
                                    "wwwroot\\images\\books", 
                                    file);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                path = $"~/images/books/{file}";
            }

            var book = this.ToBook(model, path);

            // TODO: Change User to Logged In
            book.User = await _userHelper.GetUserByEmailAsync("SuperLibrary.Admin@gmail.com");
            await _bookRepository.CreateAsync(book);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    private Book ToBook(BookViewModel model, string path)
    {
        return new Book
        {
            Id = model.Id,
            ImageUrl = path,
            Title = model.Title,
            Author = model.Author,
            Publisher = model.Publisher,
            ReleaseYear = model.ReleaseYear,
            Copies = model.Copies,
            GenreId = model.GenreId,
            IsAvailable = model.IsAvailable,
            WasDeleted = model.WasDeleted,
            User = model.User,
        };
    }

    // GET: Books/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _bookRepository.GetByIdAsync(id.Value);
        if (book == null)
        {
            return NotFound();
        }

        var model = this.ToBookViewModel(book);
        return View(model);
    }

    private BookViewModel ToBookViewModel(Book book)
    {
        return new BookViewModel
        {
            Id = book.Id,
            ImageUrl = book.ImageUrl,
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            ReleaseYear = book.ReleaseYear,
            Copies = book.Copies,
            GenreId = book.GenreId,
            IsAvailable = book.IsAvailable,
            WasDeleted = book.WasDeleted,
            User = book.User,
        };
    }

    // POST: Books/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var path = model.ImageUrl;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(Directory.GetCurrentDirectory(),
                                        "wwwroot\\images\\books",
                                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/books/{file}";
                }

                var book = this.ToBook(model, path);

                // TODO: Change User to Logged In
                book.User = await _userHelper.GetUserByEmailAsync("SuperLibrary.Admin@gmail.com");
                await _bookRepository.UpdateAsync(book);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _bookRepository.ExistAsync(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Books/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _bookRepository.GetByIdAsync(id.Value);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        await _bookRepository.DeleteAsync(book);
        return RedirectToAction(nameof(Index));
    }
}