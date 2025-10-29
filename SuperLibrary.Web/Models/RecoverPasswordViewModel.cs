using System.ComponentModel.DataAnnotations;

namespace SuperLibrary.Web.Models;

public class RecoverPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
