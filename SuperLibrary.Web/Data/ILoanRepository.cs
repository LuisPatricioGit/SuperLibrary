using System.Linq;
using System.Threading.Tasks;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Data;

public interface ILoanRepository : IGenericRepository<Loan>
{
    Task<IQueryable<Loan>> GetLoanAsync(string userName);

    Task<IQueryable<LoanDetailTemp>> GetDetailTempsAsync(string userName);

    Task AddItemToLoanAsync(AddItemViewModel model, string userName);

    Task ModifyLoanDetailTempQuantityAsync(int id, int quantity);

    Task DeleteLoanDetailTempAsync(int id);
}
