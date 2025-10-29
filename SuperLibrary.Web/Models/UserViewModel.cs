using System.ComponentModel.DataAnnotations;

namespace SuperLibrary.Web.Models;

public class UserViewModel
{
    [Required]
    [MaxLength(25)]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
}