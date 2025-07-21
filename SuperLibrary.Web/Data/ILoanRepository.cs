using System.Linq;
using System.Threading.Tasks;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data;

public interface ILoanRepository : IGenericRepository<Loan>
{
    Task<IQueryable<Loan>> GetLoanAsync(string userName);
}
