using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Web.Data;
using SuperLibrary.Web.Helper;

namespace SuperLibrary.Web.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ILoanRepository _loanRepository;

        public LoansController(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _loanRepository.GetLoanAsync(this.User.Identity.Name);
            return View(model);
        }
    }
}
