using System;
using System.Linq;
using System.Threading.Tasks;
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
            Guid imageId = Guid.Empty;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "books");
            }

            var book = _converterHelper.ToBook(model, imageId, true);

            // TODO: Change User to Logged In
            book.User = await _userHelper.GetUserByEmailAsync("SuperLibrary.Admin@gmail.com");
            await _bookRepository.CreateAsync(book);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
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

        var model = _converterHelper.ToBookViewModel(book);
        return View(model);
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
                Guid imageId = model.ImageId;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "books");
                }

                var book = _converterHelper.ToBook(model, imageId, false);

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