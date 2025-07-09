using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Helper;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Controllers;

public class AccountController : Controller
{
    private readonly IUserHelper _userHelper;

    public AccountController(IUserHelper userHelper)
    {
        _userHelper = userHelper;
    }

    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                if (this.Request.Query.Keys.Contains("ReturnUrl"))
                {
                    return Redirect(this.Request.Query["ReturnUrl"].First());
                }
                return this.RedirectToAction("Index", "Home");
            }
        }
        this.ModelState.AddModelError(string.Empty, "Failed to login");
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _userHelper.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterNewUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                user = new User
                {
                    Email = model.Email, 
                    UserName = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userHelper.AddUserAsync(user, model.Password);
                if (result != IdentityResult.Success)
                {
                    ModelState.AddModelError(string.Empty, "The User couldn't be created");
                    return View(model);
                }

                var loginViewModel = new LoginViewModel
                {
                    Username = model.Username,
                    Password = model.Password,
                    RememberMe = false
                };

                var resultLogin = await _userHelper.LoginAsync(loginViewModel);
                if (resultLogin.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Failed to login after registration");
            }
            ModelState.AddModelError(string.Empty, "Failed to register, Email already in Use!");
        }
        return View(model);
    }
}
