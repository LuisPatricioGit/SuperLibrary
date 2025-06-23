using Microsoft.AspNetCore.Identity;

namespace SuperLibrary.Web.Data.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}