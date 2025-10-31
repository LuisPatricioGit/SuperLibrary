using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperLibrary.Api.Helper;
using SuperLibrary.Api.Models.API;
using SuperLibrary.Api.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SuperLibrary.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserHelper _userHelper;
    private readonly IConfiguration _configuration;

    public UsersController(IUserHelper userHelper, IConfiguration configuration)
    {
        _userHelper = userHelper;
        _configuration = configuration;
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

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterNewUserViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var user = new Data.Entities.User
        {
            UserName = model.Username,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            IsAdminConfirmed = false
        };
        var result = await _userHelper.AddUserAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        await _userHelper.AddUserToRoleAsync(user, "Reader");
        return Ok();
    }

    /// <summary>
    /// Authenticates a user and returns a token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var user = await _userHelper.GetUserByNameAsync(model.Username);
        if (user == null || !user.IsAdminConfirmed)
            return Unauthorized();
        var result = await _userHelper.ValidatePasswordAsync(user, model.Password);
        if (!result.Succeeded)
            return Unauthorized();

        // Generate JWT token
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Email),
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, System.Guid.NewGuid().ToString())
        };
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            _configuration["Tokens:Issuer"],
            _configuration["Tokens:Audience"],
            claims,
            expires: System.DateTime.UtcNow.AddDays(15),
            signingCredentials: credentials);
        var results = new
        {
            token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            userId = user.Id
        };
        return Ok(results);
    }

    /// <summary>
    /// Uploads a profile image for a user.
    /// </summary>
    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageViewModel model)
    {
        var user = await _userHelper.GetUserByIdAsync(model.UserId);
        if (user == null)
            return NotFound();
        // Save image to wwwroot/images/users/{guid}.jpg (demo only)
        var fileName = $"{user.Id}_{Path.GetFileName(model.ImageFile.FileName)}";
        var filePath = Path.Combine("wwwroot/images/users", fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.ImageFile.CopyToAsync(stream);
        }
        user.ImageUrl = $"/images/users/{fileName}";
        await _userHelper.UpdateUserAsync(user);
        return Ok(new { imageUrl = user.ImageUrl });
    }

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
    {
        var user = await _userHelper.GetUserByNameAsync(User.Identity.Name);
        if (user == null)
            return Unauthorized();
        var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        return Ok();
    }

    /// <summary>
    /// Gets the user's profile image by userId.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("image/{userId}")]
    public async Task<IActionResult> GetUserImage(string userId)
    {
        var user = await _userHelper.GetUserByIdAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.ImageUrl))
            return NotFound();
        var imagePath = Path.Combine("wwwroot", user.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (!System.IO.File.Exists(imagePath))
            return NotFound();
        var image = await System.IO.File.ReadAllBytesAsync(imagePath);
        return File(image, "image/jpeg");
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="model">User data</param>
    /// <returns>The created user</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Users
    ///     {
    ///         "userName": "johndoe",
    ///         "email": "john@example.com",
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "imageUrl": "https://example.com/image.jpg",
    ///         "isAdminConfirmed": false
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(UserApiViewModel), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostUser([FromBody] UserApiViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // This is a simplified example. In a real app, handle password, roles, etc.
        var user = new Data.Entities.User
        {
            UserName = model.UserName,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            ImageUrl = model.ImageUrl,
            IsAdminConfirmed = model.IsAdminConfirmed
        };
        // You would normally use UserManager here
        // For demo, just return the model
        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, model);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">User Id</param>
    /// <param name="model">User data</param>
    /// <returns>No content</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/Users/1
    ///     {
    ///         "id": "1",
    ///         "userName": "johndoe",
    ///         "email": "john@example.com",
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "imageUrl": "https://example.com/image.jpg",
    ///         "isAdminConfirmed": true
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PutUser(string id, [FromBody] UserApiViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        // In a real app, fetch and update the user entity
        // For demo, just return NoContent
        return NoContent();
    }
}
