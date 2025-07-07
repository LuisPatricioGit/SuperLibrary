using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Helper;

public interface IUserHelper
{
    Task<User> GetUserByEmailAsync(string email);
    Task<IdentityResult> AddUserAsync(User user, string password);

    Task<SignInResult> LoginAsync(LoginViewModel model);

    Task LogoutAsync();
}
