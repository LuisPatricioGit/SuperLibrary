using System.Linq;
using System.Threading.Tasks;
using SuperLibrary.Api.Data.Entities;
using SuperLibrary.Api.Models;

namespace SuperLibrary.Api.Data;

public interface ILoanRepository : IGenericRepository<Loan>
{
    Task<IQueryable<Loan>> GetLoanAsync(string userName);

    Task<IQueryable<LoanDetailTemp>> GetDetailTempsAsync(string userName);

    Task AddItemToLoanAsync(AddItemViewModel model, string userName);

    Task ModifyLoanDetailTempQuantityAsync(int id, int quantity);

    Task DeleteLoanDetailTempAsync(int id);

    Task<bool> ConfirmLoanAsync(string userName);

    Task DeliverLoan(DeliveryViewModel model);

    Task<Loan> GetLoanAsync(int id);
}
