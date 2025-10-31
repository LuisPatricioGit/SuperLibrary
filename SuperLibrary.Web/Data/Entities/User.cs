using System;
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
    public Guid ImageId { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => $"{FirstName} {LastName}";

    [Display(Name = "Username")]
    public override string UserName { get; set; }

    [Display(Name = "Image")]
    public string ImageUrl { get; set; }

    public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://superlibrary-d7cmb3geg9d8dab7.westeurope-01.azurewebsites.net/images/noimage.png"
            : $"https://superlibrary.blob.core.windows.net/users/{ImageId}";
    // $"https://localhost:44353/images/noimage.png" // Local version

    public bool MustChangePassword { get; set; }

    // Indicates if the user has been confirmed by an admin
    public bool IsAdminConfirmed { get; set; } = false;
}
