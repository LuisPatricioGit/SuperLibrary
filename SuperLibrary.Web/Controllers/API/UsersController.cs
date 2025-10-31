using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Web.Helper;
using SuperLibrary.Web.Models.API;
using System.Linq;
using System.Threading.Tasks;

namespace SuperLibrary.Web.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly IUserHelper _userHelper;

    public UsersController(IUserHelper userHelper)
    {
        _userHelper = userHelper;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = (await _userHelper.GetAllUsersAsync())
            .Select(u => new UserApiViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ImageUrl = u.ImageUrl,
                IsAdminConfirmed = u.IsAdminConfirmed
            }).ToList();
        return Ok(users);
    }
}
