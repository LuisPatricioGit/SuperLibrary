using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Web.Data;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Controllers;

[Authorize]
public class LoansController : Controller
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;

    public LoansController(ILoanRepository loanRepository, IBookRepository bookRepository)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _loanRepository.GetLoanAsync(this.User.Identity.Name);
        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        var model = await _loanRepository.GetDetailTempsAsync(this.User.Identity.Name);
        return View(model);
    }

    public IActionResult AddBook()
    {
        var model = new AddItemViewModel
        {
            Quantity = 1,
            Books = _bookRepository.GetComboBooks()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(AddItemViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _loanRepository.AddItemToLoanAsync(model, this.User.Identity.Name);
            return RedirectToAction("Create");
        }

        return View(model);
    }

    public async Task<IActionResult> DeleteItem(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        await _loanRepository.DeleteLoanDetailTempAsync(id.Value);
        return RedirectToAction("Create");
    }

    public async Task<IActionResult> Increase(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        await _loanRepository.ModifyLoanDetailTempQuantityAsync(id.Value, 1);
        return RedirectToAction("Create");
    }

    public async Task<IActionResult> Decrease(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        await _loanRepository.ModifyLoanDetailTempQuantityAsync(id.Value, -1);
        return RedirectToAction("Create");
    }

    public async Task<IActionResult> ConfirmLoan()
    {
        var response = await _loanRepository.ConfirmLoanAsync(this.User.Identity.Name);
        if (response)
        {
            return RedirectToAction("Index");
        }

        return RedirectToAction("Create");
    }
}