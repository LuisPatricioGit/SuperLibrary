using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperLibrary.Api.Data.Entities;
using SuperLibrary.Api.Models;

namespace SuperLibrary.Api.Helper;

public interface IUserHelper
{
    Task<IdentityResult> AddUserAsync(User user, string password);
    Task AddUserToRoleAsync(User user, string roleName);
    Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

    Task CheckRoleAsync(string roleName);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    Task<string> GenerateEmailConfirmationTokenAsync(User user);

    Task<string> GeneratePasswordResetTokenAsync(User user);
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByEmailAsync(string email);

    Task<User> GetUserByIdAsync(string userId);
    Task<User> GetUserByNameAsync(string userName);
    Task<bool> IsUserInRoleAsync(User user, string roleName);

    Task<SignInResult> LoginAsync(LoginViewModel model);
    Task LogoutAsync();
    Task<IdentityResult> RemoveUserAsync(User user);

    Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
    Task<IdentityResult> UpdateUserAsync(User user);
    Task<SignInResult> ValidatePasswordAsync(User user, string password);
}
