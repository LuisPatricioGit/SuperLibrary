using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Web.Data;
using SuperLibrary.Web.Helper;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Controllers
{
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
    }
}
