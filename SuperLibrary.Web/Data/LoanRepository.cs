using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Resources;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Helper;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Data;

public class LoanRepository : GenericRepository<Loan>, ILoanRepository
{
    private readonly DataContext _context;
    private readonly IUserHelper _userHelper;

    public LoanRepository(DataContext context, IUserHelper userHelper) : base(context)
    {
        _context = context;
        _userHelper = userHelper;
    }

    /// <summary>
    /// Adds an Item to the Loan (LoanDetailTemp) for a User.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    public async Task AddItemToLoanAsync(AddItemViewModel model, string userName)
    {
        var user = await _userHelper.GetUserByNameAsync(userName);
        if (user == null)
        {
            return;
        }

        var book = await _context.Books.FindAsync(model.BookId);
        if (book == null || book.WasDeleted)
        {
            return;
        }

        var loanDetailTemp = await _context.LoanDetailsTemp
            .Where(ldt => ldt.User == user && ldt.Book == book)
            .FirstOrDefaultAsync();

        if (loanDetailTemp == null)
        {
            loanDetailTemp = new LoanDetailTemp
            {
                User = user,
                Book = book,
                Quantity = model.Quantity
            };

            _context.LoanDetailsTemp.Add(loanDetailTemp);
        }
        else
        {
            loanDetailTemp.Quantity += model.Quantity;
            _context.LoanDetailsTemp.Update(loanDetailTemp);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ConfirmLoanAsync(string userName)
    {
        var user = await _userHelper.GetUserByNameAsync(userName);
        if (user == null)
        {
            return false;
        }

        var loanTemp = await _context.LoanDetailsTemp
            .Include(l => l.Book)
            .Where(l => l.User == user && !l.WasDeleted)
            .ToListAsync();

        if (loanTemp == null || loanTemp.Count == 0)
        {
            return false;
        }

        var details = loanTemp.Select(l => new LoanDetail
        {
            Book = l.Book,
            Quantity = l.Quantity,
            User = user
        }).ToList();

        var loan = new Loan
        {
            User = user,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(15), // TODO: Adjust due date per role
            LoanItems = details
        };

        await CreateAsync(loan);
        _context.LoanDetailsTemp.RemoveRange(loanTemp);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Removes a LoanDetailTemp by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteLoanDetailTempAsync(int id)
    {
        var loanDetailTemp = await _context.LoanDetailsTemp.FindAsync(id);
        if (loanDetailTemp == null)
        {
            return;
        }

        _context.LoanDetailsTemp.Remove(loanDetailTemp);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Returns all LoanDetailTemps for a User.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns>LoanDetailsTemp</returns>
    public async Task<IQueryable<LoanDetailTemp>> GetDetailTempsAsync(string userName)
    {
        var user = await _userHelper.GetUserByNameAsync(userName);
        if (user == null)
        {
            return null;
        }

        return _context.LoanDetailsTemp
            .Include(ldt => ldt.Book)
            .Where(ldt => ldt.User == user && !ldt.WasDeleted)
            .OrderByDescending(ldt => ldt.Book.Title);
    }

    /// <summary>
    /// Returns all Loans for a User or all Loans if the User is an Employee.
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Loans</returns>
    public async Task<IQueryable<Loan>> GetLoanAsync(string userName)
    {
        var user = await _userHelper.GetUserByNameAsync(userName);
        if (user == null)
        {
            return null;
        }

        if (await _userHelper.IsUserInRoleAsync(user, "Employee"))
        {
            return _context.Loans
                .Include(l => l.User)
                .Include(l => l.LoanItems)
                .ThenInclude(li => li.Book)
                .OrderByDescending(l => l.LoanDate);
        }

        return _context.Loans
            .Include(l => l.LoanItems)
            .ThenInclude(li => li.Book)
            .Where(l => l.User == user && !l.WasDeleted)
            .OrderByDescending(l => l.LoanDate);
    }

    /// <summary>
    /// Modifies the quantity of a LoanDetailTemp by adding the specified quantity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public async Task ModifyLoanDetailTempQuantityAsync(int id, int quantity)
    {
        var loanDetailTemp = await _context.LoanDetailsTemp.FindAsync(id);
        if (loanDetailTemp == null)
        {
            return;
        }

        loanDetailTemp.Quantity += quantity;
        if (loanDetailTemp.Quantity > 0)
        {
            _context.LoanDetailsTemp.Update(loanDetailTemp);
            await _context.SaveChangesAsync();
        }
    }
}
