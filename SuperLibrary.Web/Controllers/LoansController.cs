using System;
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

    /// <summary>
    /// Displays the index view for loans, showing the current user's loan details.
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Index()
    {
        var model = await _loanRepository.GetLoanAsync(this.User.Identity.Name);
        return View(model);
    }

    /// <summary>
    /// Displays the view for creating a new loan, showing temporary loan details for the current user.
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Create()
    {
        var model = await _loanRepository.GetDetailTempsAsync(this.User.Identity.Name);
        return View(model);
    }

    /// <summary>
    /// Displays the view for adding a new book to the loan.
    /// </summary>
    /// <returns></returns>
    public IActionResult AddBook()
    {
        var model = new AddItemViewModel
        {
            Quantity = 1,
            Books = _bookRepository.GetComboBooks()
        };

        return View(model);
    }

    /// <summary>
    /// Handles the addition of a new book to the loan based on the provided model.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Deletes a specific item from the temporary loan details based on the provided ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> DeleteItem(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        await _loanRepository.DeleteLoanDetailTempAsync(id.Value);
        return RedirectToAction("Create");
    }

    /// <summary>
    /// Increases the quantity of a specific item in the temporary loan details by 1 based on the provided ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> Increase(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        await _loanRepository.ModifyLoanDetailTempQuantityAsync(id.Value, 1);
        return RedirectToAction("Create");
    }

    /// <summary>
    /// Decreases the quantity of a specific item in the temporary loan details by 1 based on the provided ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> Decrease(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        await _loanRepository.ModifyLoanDetailTempQuantityAsync(id.Value, -1);
        return RedirectToAction("Create");
    }

    /// <summary>
    /// Confirms the loan for the current user, finalizing the loan process and redirecting to the index view.
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> ConfirmLoan()
    {
        var response = await _loanRepository.ConfirmLoanAsync(this.User.Identity.Name);
        if (response)
        {
            return RedirectToAction("Index");
        }

        return RedirectToAction("Create");
    }

    /// <summary>
    /// Displays the view for delivering a loan based on the provided ID, initializing the delivery date to today.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> Deliver(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var loan = await _loanRepository.GetLoanAsync(id.Value);
        if (loan == null || loan.WasDeleted)
        {
            return NotFound();
        }

        var model = new DeliveryViewModel
        {
            Id = loan.Id,
            DeliveryDate = DateTime.Today,
        };

        return View(model); 
    }

    /// <summary>
    /// Handles the delivery of a loan based on the provided model, updating the delivery date and redirecting to the index view.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Deliver(DeliveryViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _loanRepository.DeliverLoan(model);
            return RedirectToAction("Index");
        }

       return View();
    }
}