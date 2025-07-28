using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Web.Data;
using SuperLibrary.Web.Helper;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Controllers;

public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;
    private readonly IUserHelper _userHelper;
    private readonly IBlobHelper _blobHelper;
    private readonly IConverterHelper _converterHelper;

    public BooksController(
        IBookRepository bookRepository,
        IUserHelper userHelper,
        IBlobHelper blobHelper,
        IConverterHelper converterHelper)
    {
        _bookRepository = bookRepository;
        _userHelper = userHelper;
        _blobHelper = blobHelper;
        _converterHelper = converterHelper;
    }

    /// <summary>
    /// Displays a list of all books in the library, ordered by title.
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return View(_bookRepository.GetAll().OrderBy(b => b.Title));
    }

    /// <summary>
    /// Displays the details of a specific book based on its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return new NotFoundViewResult("BookNotFound");
        }

        var book = await _bookRepository.GetByIdAsync(id.Value);
        if (book == null)
        {
            return new NotFoundViewResult("BookNotFound");
        }

        return View(book); 
    }

    /// <summary>
    /// Displays the view for creating a new book entry.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin,Employee")]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Handles the submission of a new book entry.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookViewModel model)
    {
        if (ModelState.IsValid)
        {
            Guid imageId = Guid.Empty;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "books");
            }

            var book = _converterHelper.ToBook(model, imageId, true);

            book.User = await _userHelper.GetUserByNameAsync(this.User.Identity.Name);
            await _bookRepository.CreateAsync(book);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    /// <summary>
    /// Displays the view for editing an existing book entry based on its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return new NotFoundViewResult("BookNotFound");
        }

        var book = await _bookRepository.GetByIdAsync(id.Value);
        if (book == null)
        {
            return new NotFoundViewResult("BookNotFound");
        }

        var model = _converterHelper.ToBookViewModel(book);
        return View(model);
    }

    /// <summary>
    /// Handles the submission of the edited book details.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                Guid imageId = model.ImageId;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "books");
                }

                var book = _converterHelper.ToBook(model, imageId, false);

                book.User = await _userHelper.GetUserByNameAsync(this.User.Identity.Name);
                await _bookRepository.UpdateAsync(book);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _bookRepository.ExistAsync(model.Id))
                {
                    return new NotFoundViewResult("BookNotFound");
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

    /// <summary>
    /// Displays the confirmation view for deleting a book based on its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return new NotFoundViewResult("BookNotFound");
        }

        var book = await _bookRepository.GetByIdAsync(id.Value);
        if (book == null)
        {
            return new NotFoundViewResult("BookNotFound");
        }

        return View(book);
    }

    /// <summary>
    /// Handles the deletion of a book entry based on its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        
        try
        {
            await _bookRepository.DeleteAsync(book);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
            {
                ViewBag.ErrorTitle = $"Book {book.Title} is beiong used...</br>";
                ViewBag.ErrorMessage = $"Book {book.Title} can't be deleted due to existing loans/reservations</br></br>" +
                    $"Please delete all associated Loans and Reservations to Delete the current book."; 
            }

            return View("Error");
        }
    }

    /// <summary>
    /// Displays a view when a book is not found, typically for 404 errors.
    /// </summary>
    /// <returns></returns>
    public IActionResult BookNotFound()
    {
        return View();
    }
}