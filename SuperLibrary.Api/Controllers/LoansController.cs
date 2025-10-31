using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Api.Data;
using SuperLibrary.Api.Models.API;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace SuperLibrary.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LoansController : ControllerBase
{
    private readonly DataContext _dbContext;

    public LoansController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetLoans()
    {
        var loans = await _dbContext.Set<SuperLibrary.Api.Data.Entities.Loan>()
            .Include(l => l.User)
            .Include(l => l.LoanItems)
            .ThenInclude(li => li.Book)
            .ToListAsync();

        var result = loans.Select(l => new LoanApiViewModel
        {
            Id = l.Id,
            UserName = l.User != null ? l.User.UserName : null,
            LoanDate = l.LoanDate,
            DueDate = l.DueDate,
            DeliveryDate = l.DeliveryDate,
            LoanItems = l.LoanItems != null ? l.LoanItems.Select(li => new LoanItemApiViewModel
            {
                BookId = li.Book.Id,
                BookTitle = li.Book.Title,
                Quantity = li.Quantity
            }).ToList() : new List<LoanItemApiViewModel>()
        }).ToList();
        return Ok(result);
    }

    /// <summary>
    /// Gets all loans for a specific user by UserId.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns>List of loans for the user</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<LoanApiViewModel>), 200)]
    public async Task<IActionResult> GetLoansByUser(string userId)
    {
        var loans = await _dbContext.Loans
            .Include(l => l.User)
            .Include(l => l.LoanItems)
            .ThenInclude(li => li.Book)
            .Where(l => l.User.Id == userId)
            .ToListAsync();

        var result = loans.Select(l => new LoanApiViewModel
        {
            Id = l.Id,
            UserName = l.User != null ? l.User.UserName : null,
            LoanDate = l.LoanDate,
            DueDate = l.DueDate,
            DeliveryDate = l.DeliveryDate,
            LoanItems = l.LoanItems != null ? l.LoanItems.Select(li => new LoanItemApiViewModel
            {
                BookId = li.Book.Id,
                BookTitle = li.Book.Title,
                Quantity = li.Quantity
            }).ToList() : new List<LoanItemApiViewModel>()
        }).ToList();
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific loan by LoanId.
    /// </summary>
    /// <param name="id">Loan Id</param>
    /// <returns>The loan with the specified Id</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LoanApiViewModel), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetLoan(int id)
    {
        var loan = await _dbContext.Loans
            .Include(l => l.User)
            .Include(l => l.LoanItems)
            .ThenInclude(li => li.Book)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (loan == null)
            return NotFound();
        var model = new LoanApiViewModel
        {
            Id = loan.Id,
            UserName = loan.User != null ? loan.User.UserName : null,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            DeliveryDate = loan.DeliveryDate,
            LoanItems = loan.LoanItems != null ? loan.LoanItems.Select(li => new LoanItemApiViewModel
            {
                BookId = li.Book.Id,
                BookTitle = li.Book.Title,
                Quantity = li.Quantity
            }).ToList() : new List<LoanItemApiViewModel>()
        };
        return Ok(model);
    }

    /// <summary>
    /// Creates a new loan.
    /// </summary>
    /// <param name="model">Loan data</param>
    /// <returns>The created loan</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Loans
    ///     {
    ///         "userName": "johndoe",
    ///         "loanDate": "2024-07-21T00:00:00Z",
    ///         "dueDate": "2024-08-05T00:00:00Z",
    ///         "deliveryDate": "2024-07-22T00:00:00Z",
    ///         "loanItems": [
    ///             { "bookId": 1, "bookTitle": "Book Title", "quantity": 1 }
    ///         ]
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(LoanApiViewModel), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostLoan([FromBody] LoanApiViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
        if (user == null)
            return BadRequest("User not found");

        var loan = new Data.Entities.Loan
        {
            User = user,
            LoanDate = model.LoanDate,
            DueDate = model.DueDate,
            DeliveryDate = model.DeliveryDate,
            LoanItems = model.LoanItems?.Select(li => new Data.Entities.LoanDetail
            {
                Book = _dbContext.Books.Find(li.BookId),
                Quantity = li.Quantity,
                User = user
            }).ToList()
        };
        _dbContext.Loans.Add(loan);
        await _dbContext.SaveChangesAsync();
        model.Id = loan.Id;
        return CreatedAtAction(nameof(GetLoans), new { id = loan.Id }, model);
    }

    /// <summary>
    /// Updates an existing loan.
    /// </summary>
    /// <param name="id">Loan Id</param>
    /// <param name="model">Loan data</param>
    /// <returns>No content</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/Loans/1
    ///     {
    ///         "id": 1,
    ///         "userName": "johndoe",
    ///         "loanDate": "2024-07-21T00:00:00Z",
    ///         "dueDate": "2024-08-05T00:00:00Z",
    ///         "deliveryDate": "2024-07-22T00:00:00Z",
    ///         "loanItems": [
    ///             { "bookId": 1, "bookTitle": "Book Title", "quantity": 2 }
    ///         ]
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PutLoan(int id, [FromBody] LoanApiViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        var loan = await _dbContext.Loans.Include(l => l.LoanItems).FirstOrDefaultAsync(l => l.Id == id);
        if (loan == null)
            return NotFound();

        loan.LoanDate = model.LoanDate;
        loan.DueDate = model.DueDate;
        loan.DeliveryDate = model.DeliveryDate;
        // Optionally update LoanItems here
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
}
