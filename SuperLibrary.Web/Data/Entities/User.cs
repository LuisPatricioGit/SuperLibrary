using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SuperLibrary.Web.Data.Entities;

public class User : IdentityUser
{
    [MaxLength(25)]
    public string FirstName { get; set; }

    [MaxLength(25)]
    public string LastName { get; set; }

    [Display(Name = "Image")]
    public string ImageUrl { get; set; }
}