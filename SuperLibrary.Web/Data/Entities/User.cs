 using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SuperLibrary.Web.Data.Entities;

public class User : IdentityUser
{
    [MaxLength(25)]
    public string FirstName { get; set; }

    [MaxLength(25)]
    public string LastName { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => $"{FirstName} {LastName}";

    [Display(Name = "Username")]
    public string Username
    {
        get => base.UserName;
        set => base.UserName = value;
    }

    [Display(Name = "Image")]
    public string ImageUrl { get; set; }

    public string ImageFullPath
    {
        get
        {
            if (string.IsNullOrEmpty(ImageUrl))
            {
                return null;
            }

            return $"https://localhost:44353{ImageUrl.Substring(1)}";
        }
    }
}