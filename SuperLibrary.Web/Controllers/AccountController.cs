using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    private readonly IImageHelper _imageHelper;

    public AccountController(IUserHelper userHelper, IMailHelper mailHelper, IConfiguration configuration, IImageHelper imageHelper)
    {
        _userHelper = userHelper;
        _mailHelper = mailHelper;
        _configuration = configuration;
        _imageHelper = imageHelper;
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
            // Require admin confirmation
            if (user != null && !user.IsAdminConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Your account is pending admin approval.");
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
                    PhoneNumber = model.PhoneNumber,
                    IsAdminConfirmed = false // Require admin confirmation
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
        if (user != null && user.MustChangePassword)
        {
            // Force password change before allowing access
            return RedirectToAction("ChangePassword");
        }
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
        var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
        if (user != null && user.MustChangePassword)
        {
            // Force password change before allowing access
            return RedirectToAction("ChangePassword");
        }
        if (ModelState.IsValid)
        {
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
        // Explicitly specify the new path for the moved view
        return View("~/Views/Errors/AccessDenied.cshtml");
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

    /// <summary>
    /// Displays the profile view for the authenticated user.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userHelper.GetUserByNameAsync(User.Identity.Name);
        if (user == null) return RedirectToAction("Login");
        if (user.MustChangePassword)
        {
            // Force password change before allowing access
            return RedirectToAction("ChangePassword");
        }
        var model = new UserViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };
        ViewBag.ProfileImageUrl = string.IsNullOrEmpty(user.ImageUrl) ? "/images/noimage.png" : user.ImageUrl;
        ViewBag.StatusMessage = TempData["StatusMessage"];
        return View(model);
    }

    /// <summary>
    /// Handles the profile update process for the authenticated user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UserViewModel model)
    {
        var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
        if (user != null && user.MustChangePassword)
        {
            // Force password change before allowing access
            return RedirectToAction("ChangePassword");
        }
        if (!ModelState.IsValid)
        {
            TempData["StatusMessage"] = "Invalid data.";
            return RedirectToAction("Profile");
        }
        if (user == null) return RedirectToAction("Login");
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        var result = await _userHelper.UpdateUserAsync(user);
        TempData["StatusMessage"] = result.Succeeded ? "Profile updated successfully." : "Failed to update profile.";
        return RedirectToAction("Profile");
    }

    /// <summary>
    /// Handles the profile picture change process for the authenticated user.
    /// </summary>
    /// <param name="profilePicture"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangeProfilePicture(IFormFile profilePicture)
    {
        var user = await _userHelper.GetUserByNameAsync(User.Identity.Name);
        if (user != null && user.MustChangePassword)
        {
            // Force password change before allowing access
            return RedirectToAction("ChangePassword");
        }
        if (user == null) return RedirectToAction("Login");
        if (profilePicture != null && profilePicture.Length > 0)
        {
            var imageUrl = await _imageHelper.UploadImageAsync(profilePicture.OpenReadStream(), profilePicture.FileName, "users");
            user.ImageUrl = imageUrl;
            await _userHelper.UpdateUserAsync(user);
            TempData["StatusMessage"] = "Profile picture updated.";
        }
        else
        {
            TempData["StatusMessage"] = "No image selected.";
        }
        return RedirectToAction("Profile");
    }

    /// <summary>
    /// Displays a list of users in the specified role, accessible only to Admins, with management actions.
    /// </summary>
    /// <param name="role">The role to filter users by (e.g., Reader, Employee, Admin).</param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UsersByRole(string role)
    {
        if (string.IsNullOrEmpty(role))
        {
            return RedirectToAction("ManageUsers");
        }
        var allUsers = await _userHelper.GetAllUsersAsync();
        var usersInRole = new List<UserViewModel>();
        foreach (var user in allUsers)
        {
            if (await _userHelper.IsUserInRoleAsync(user, role))
            {
                usersInRole.Add(new UserViewModel
                {
                    Username = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    IsAdminConfirmed = user.IsAdminConfirmed // <-- Add this line
                });
            }
        }
        ViewBag.Role = role;
        return View(usersInRole);
    }

    /// <summary>
    /// Displays users filtered by admin confirmation status (Pending or Confirmed).
    /// </summary>
    /// <param name="status">"Pending" for !IsAdminConfirmed, "Confirmed" for IsAdminConfirmed</param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UsersByStatus(string status)
    {
        var allUsers = await _userHelper.GetAllUsersAsync();
        var users = new List<UserViewModel>();
        if (status == "Pending")
        {
            users = allUsers.Where(u => !u.IsAdminConfirmed).Select(user => new UserViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsAdminConfirmed = user.IsAdminConfirmed
            }).ToList();
        }
        else if (status == "Confirmed")
        {
            users = allUsers.Where(u => u.IsAdminConfirmed).Select(user => new UserViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsAdminConfirmed = user.IsAdminConfirmed
            }).ToList();
        }
        ViewBag.Status = status;
        return View("UsersByRole", users); // Reuse UsersByRole view
    }

    /// <summary>
    /// Displays all users regardless of role or status.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AllUsers()
    {
        var allUsers = await _userHelper.GetAllUsersAsync();
        var users = allUsers.Select(user => new UserViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            IsAdminConfirmed = user.IsAdminConfirmed
        }).ToList();
        ViewBag.Role = "All";
        return View("UsersByRole", users); // Reuse UsersByRole view
    }

    /// <summary>
    /// Displays the user management view, accessible only to Admins.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public IActionResult ManageUsers()
    {
        // This view acts as an index for managing users by role
        return View();
    }

    /// <summary>
    /// Displays the view to add a new user to a specified role, accessible only to Admins.
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    // GET: Add user to role
    [Authorize(Roles = "Admin")]
    public IActionResult AddUserToRole(string role)
    {
        ViewBag.Role = role;
        return View();
    }

    /// <summary>
    /// Handles the process of adding a new user to a specified role, accessible only to Admins.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    // POST: Add user to role
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddUserToRole(RegisterNewUserViewModel model, string role)
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
                    PhoneNumber = model.PhoneNumber,
                    IsAdminConfirmed = true,
                    EmailConfirmed = true,
                    MustChangePassword = true // Require password change on first login
                };
                var result = await _userHelper.AddUserAsync(user, model.Password);
                if (result == IdentityResult.Success)
                {
                    await _userHelper.AddUserToRoleAsync(user, role);
                    return RedirectToAction("UsersByRole", new { role });
                }
                ModelState.AddModelError(string.Empty, "The User couldn't be created");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User already exists");
            }
        }
        ViewBag.Role = role;
        return View(model);
    }

    /// <summary>
    /// Displays the view to edit a user's details in a specified role, accessible only to Admins.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    // GET: Edit user in role
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditUserInRole(string username, string role)
    {
        var user = await _userHelper.GetUserByNameAsync(username);
        if (user == null) return NotFound();
        var model = new UserViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };
        ViewBag.Role = role;
        return View(model);
    }

    /// <summary>
    /// Handles the process of editing a user's details in a specified role, accessible only to Admins.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    // POST: Edit user in role
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditUserInRole(UserViewModel model, string role)
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
                return RedirectToAction("UsersByRole", new { role });
            ModelState.AddModelError("", "Failed to update user");
        }
        ViewBag.Role = role;
        return View(model);
    }

    /// <summary>
    /// Handles the process of removing a user from a specified role, accessible only to Admins.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    // POST: Remove user from role
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveUserFromRole(string username, string role)
    {
        var user = await _userHelper.GetUserByNameAsync(username);
        if (user == null) return NotFound();
        var result = await _userHelper.RemoveUserAsync(user);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to remove user");
        }
        return RedirectToAction("UsersByRole", new { role });
    }

    /// <summary>
    /// Handles the process of confirming a user in a specified role, accessible only to Admins.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    // POST: Confirm user in role
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmUserInRole(string username, string role)
    {
        var user = await _userHelper.GetUserByNameAsync(username);
        if (user == null) return NotFound();
        user.IsAdminConfirmed = true;
        await _userHelper.UpdateUserAsync(user);
        return RedirectToAction("UsersByRole", new { role });
    }

    /// <summary>
    /// Displays the details of a user in a specified role, accessible only to Admins.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    // GET: View user details
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UserDetails(string username, string role)
    {
        var user = await _userHelper.GetUserByNameAsync(username);
        if (user == null) return NotFound();
        var model = new UserViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };
        ViewBag.Role = role;
        ViewBag.ProfileImageUrl = string.IsNullOrEmpty(user.ImageUrl) ? "/images/noimage.png" : user.ImageUrl;
        return View(model);
    }
}
