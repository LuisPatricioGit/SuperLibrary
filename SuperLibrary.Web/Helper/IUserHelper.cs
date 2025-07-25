﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Helper;

public interface IUserHelper
{
    Task<User> GetUserByEmailAsync(string email);

    Task<User> GetUserByNameAsync(string userName);

    Task<IdentityResult> AddUserAsync(User user, string password);

    Task<SignInResult> LoginAsync(LoginViewModel model);

    Task LogoutAsync();

    Task<IdentityResult> UpdateUserAsync(User user);

    Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

    Task CheckRoleAsync(string roleName);

    Task AddUserToRoleAsync(User user, string roleName);

    Task<bool> IsUserInRoleAsync(User user, string roleName);
}
