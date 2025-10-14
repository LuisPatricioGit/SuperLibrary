using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// Displays the login view if the user is not authenticated, otherwise redirects to the home page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    /// <summary>
    /// Handles the login process for users.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Logs out the current user and redirects to the home page.
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Logout()
    {
        await _userHelper.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Displays the registration view for new users.
    /// </summary>
    /// <returns></returns>
    public IActionResult Register()
    {
        return View();
    }

    /// <summary>
    /// Handles the registration process for new users.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
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

                var isInRole = await _userHelper.IsUserInRoleAsync(user, "Reader");
                if (!isInRole)
                {
                    await _userHelper.AddUserToRoleAsync(user, "Reader");
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
            ModelState.AddModelError(string.Empty, "Failed to register!");
        }
        return View(model);
    }

    /// <summary>
    /// Displays the view for changing user details such as name and phone number.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin,Employee,Reader")]
    public async Task<IActionResult> ChangeUser()
    {
        var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
        var model = new ChangeUserViewModel();
        if (user != null)
        {
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.PhoneNumber = user.PhoneNumber;
        }
        return View(model);
    }

    /// <summary>
    /// Handles the process of changing user details such as name and phone number.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                var response = await _userHelper.UpdateUserAsync(user);
                if (response.Succeeded)
                {
                    ViewBag.UserMessage = "User updated successfully!";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                }
            }
        }
        return View(model);
    }

    /// <summary>
    /// Displays the view for changing the user's password.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin,Employee,Reader")]
    public IActionResult ChangePassword()
    {
        return View();
    }

    /// <summary>
    /// Handles the process of changing the user's password.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user != null)
            {
                var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return this.RedirectToAction("ChangeUser");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                }
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "User not found");
            }
        }
        return View(model);
    }

    /// <summary>
    /// Displays the access denied view when a user tries to access a resource they are not authorized for.
    /// </summary>
    /// <returns></returns>
    public IActionResult AccessDenied()
    {
        return View();
    }

    /// <summary>
    /// Displays a list of employees, accessible only to Admins.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EmployeeList()
    {
        var allUsers = await _userHelper.GetAllUsersAsync();
        var employees = new List<UserViewModel>();

        foreach (var user in allUsers)
        {
            if (await _userHelper.IsUserInRoleAsync(user, "Employee"))
            {
                employees.Add(new UserViewModel
                {
                    Username = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                });
            }
        }

        return View(employees);
    }

    /// <summary>
    /// Displays the view for creating a new employee.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public IActionResult CreateEmployee()
    {
        return View();
    }

    /// <summary>
    /// Handles the process of creating a new employee.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateEmployee(RegisterNewUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userHelper.AddUserToRoleAsync(user, "Employee");
                return RedirectToAction("EmployeeList");
            }
            ModelState.AddModelError("", "Failed to create employee");
        }
        return View(model);
    }

    /// <summary>
    /// Displays the view for editing an existing employee's details.
    /// </summary>
    /// <param name="id">The ID of the employee to edit.</param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditEmployee(string id)
    {
        var user = await _userHelper.GetUserByNameAsync(id);
        if (user == null) return NotFound();
        var model = new UserViewModel
        {
            Email = user.Email,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };
        return View(model);
    }

    /// <summary>
    /// Handles the process of editing an existing employee's details.
    /// </summary>
    /// <param name="model">The model containing the updated employee details.</param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditEmployee(UserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userHelper.GetUserByNameAsync(model.Username);
            if (user == null) return NotFound();
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            var result = await _userHelper.UpdateUserAsync(user);
            if (result.Succeeded)
                return RedirectToAction("EmployeeList");
            ModelState.AddModelError("", "Failed to update employee");
        }
        return View(model);
    }

    /// <summary>
    /// Displays a list of users who are not yet confirmed, accessible only to Admins.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmUsers()
    {
        var users = await _userHelper.GetAllUsersAsync();
        var viewModels = users.Select(u => new UserViewModel
        {
            Username = u.UserName,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhoneNumber = u.PhoneNumber
        }).ToList();

        return View(viewModels);
    }

    /// <summary>
    /// Confirms a user based on their ID, accessible only to Admins.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmUser(string id)
    {
        var user = await _userHelper.GetUserByNameAsync(id);
        if (user == null) return NotFound();
        user.IsConfirmed = true;
        await _userHelper.UpdateUserAsync(user);
        return RedirectToAction("ConfirmUsers");
    }

}
