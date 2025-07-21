using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Helper;

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
}
