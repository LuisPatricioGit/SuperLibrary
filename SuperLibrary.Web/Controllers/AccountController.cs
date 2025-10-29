using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Helper;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Controllers;

public class AccountController : Controller
{
    private readonly IUserHelper _userHelper;
    private readonly IMailHelper _mailHelper;
    private readonly IConfiguration _configuration; 

    public AccountController(IUserHelper userHelper, IMailHelper mailHelper, IConfiguration configuration)
    {
        _userHelper = userHelper;
        _mailHelper = mailHelper;
        _configuration = configuration;
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
            var user = await _userHelper.GetUserByNameAsync(model.Username);
            if (user != null && !user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Your account has not been confirmed.");
                return View(model);
            }

            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                if (user.MustChangePassword)
                {
                    return RedirectToAction("ChangePassword");
                }

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

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(model.Username, model.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                    $"To allow the user, " +
                    $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                if (response.IsSuccess)
                {
                    await _userHelper.AddUserToRoleAsync(user, "Reader");
                    ViewBag.Message = "The instructions to allow you user has been sent to email";

                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "The user couldn't be logged.");
            }
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
                    user.MustChangePassword = false;
                    await _userHelper.UpdateUserAsync(user);
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

    public IActionResult RecoverPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                return View(model);
            }

            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

            var link = this.Url.Action(
                "ResetPassword",
                "Account",
                new { token = myToken }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendEmail(model.Email, model.Email, "SuperLibrary Password Reset", $"<h1>SuperLibrary Password Reset</h1>" +
            $"To reset the password click in this link:</br></br>" +
            $"<a href = \"{link}\">Reset Password</a>");

            if (response.IsSuccess)
            {
                this.ViewBag.Message = "The instructions to recover your password has been sent to email.";
            }

            return this.View();

        }

        return this.View(model);
    }

    public IActionResult ResetPassword(string token)
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        var user = await _userHelper.GetUserByEmailAsync(model.Username);
        if (user != null)
        {
            var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                this.ViewBag.Message = "Password reset successful.";
                return View();
            }

            this.ViewBag.Message = "Error while resetting the password.";
            return View(model);
        }

        this.ViewBag.Message = "User not found.";
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
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false
            };
            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userHelper.AddUserToRoleAsync(user, "Employee");

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                string subject = "Employee Email Confirmation";
                string body = $"<h1>Email Confirmation</h1>" +
                              $"Please click the link to confirm your account:</br></br><a href=\"{tokenLink}\">Confirm Email</a>";

                Response response = _mailHelper.SendEmail(model.Username, model.Email, subject, body);

                if (!response.IsSuccess)
                {
                    ModelState.AddModelError("", "Employee created, but email could not be sent.");
                }

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
        user.EmailConfirmed = true;
        await _userHelper.UpdateUserAsync(user);

        return RedirectToAction("ConfirmUsers");
    }

    /// <summary>
    /// Displays the user management view, accessible only to Admins.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ManageUsers()
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
    /// Deletes a user based on their username, accessible only to Admins.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var user = await _userHelper.GetUserByNameAsync(username);
        if (user == null) return NotFound();
        var result = await _userHelper.RemoveUserAsync(user);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to delete user");
        }
        return RedirectToAction("ManageUsers");
    }

    /// <summary>
    /// Confirms the user's email and activates the account.
    /// </summary>
    /// <param name="userid">The ID of the user.</param>
    /// <param name="token">The confirmation token.</param>
    /// <returns></returns>
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return NotFound();
        }

        var user = await _userHelper.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userHelper.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return NotFound();
        }

        return View();
    }

    /// <summary>
    /// Generates a JWT token for authenticated users.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Username);
            if (user != null)
            {
                var result = await _userHelper.ValidatePasswordAsync(
                    user,
                    model.Password);

                if (result.Succeeded)
                {
                    var claims = new[]
                    {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Tokens:Issuer"],
                        _configuration["Tokens:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(15),
                        signingCredentials: credentials);
                    var results = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    };

                    return this.Created(string.Empty, results);
                }
            }
        }
        return BadRequest();
    }
}
