using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Controllers;

public class ErrorsController : Controller
{
    /// <summary>
    /// Handles errors and displays the error view with the request ID.
    /// </summary>
    /// <returns></returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Handles 404 errors and displays a custom 404 view.
    /// </summary>
    /// <returns></returns>
    [Route("Error/404")]
    public IActionResult Error404()
    {
        return View();
    }
}
