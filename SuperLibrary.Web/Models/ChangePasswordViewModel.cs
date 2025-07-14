using System.ComponentModel.DataAnnotations;

namespace SuperLibrary.Web.Models;

public class ChangePasswordViewModel
{
    [Required]
    [Display(Name = "Current Password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required]
    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword")]
    public string Confirm { get; set; }
}
