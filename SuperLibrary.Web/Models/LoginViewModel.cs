using System.ComponentModel.DataAnnotations;

namespace SuperLibrary.Web.Models;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
