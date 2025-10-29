using System.ComponentModel.DataAnnotations;

namespace SuperLibrary.Web.Models;

public class ChangeUserViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [MaxLength(20, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
    public string PhoneNumber { get; set; }

}
