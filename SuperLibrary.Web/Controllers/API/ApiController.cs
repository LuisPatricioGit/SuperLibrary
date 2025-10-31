using Microsoft.AspNetCore.Mvc;

namespace SuperLibrary.Web.Controllers.Api
{
    public class ApiController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult BooksApi() => View();
        public IActionResult UsersApi() => View();
        public IActionResult LoansApi() => View();
    }
}
