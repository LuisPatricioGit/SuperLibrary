using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperLibrary.Web.Models;
using SuperLibrary.Web.Data;
using System.Linq;

namespace SuperLibrary.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBookRepository _bookRepository;

    public HomeController(ILogger<HomeController> logger, IBookRepository bookRepository)
    {
        _logger = logger;
        _bookRepository = bookRepository;
    }

    /// <summary>
    /// Displays the home page of the application.
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        var books = _bookRepository.GetAll().ToList();
        return View(books);
    }

    /// <summary>
    /// Displays the About page of the application.
    /// </summary>
    /// <returns></returns>
    public IActionResult Privacy()
    {
        return View();
    }
}