using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Web.Data;
using SuperLibrary.Web.Models.API;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SuperLibrary.Web.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class LoansController : Controller
{
    private readonly DbContext _dbContext;

    public LoansController(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetLoans()
    {
        var loans = await _dbContext.Set<SuperLibrary.Web.Data.Entities.Loan>()
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
}
