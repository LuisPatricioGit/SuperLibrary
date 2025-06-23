using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Helper;

public interface IUserHelper
{
    Task<User> GetUserByEmailAsync(string email);
    Task<IdentityResult> AddUserAsync(User user, string password);
}
