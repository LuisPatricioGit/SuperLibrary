using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuperLibrary.Api.Models;

public class AddItemViewModel
{
    [Display(Name = "Book")]
    [Range(1, int.MaxValue, ErrorMessage = "You must select a Book.")]
    public int BookId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "The Quantity must be a positive number.")]
    public int Quantity { get; set; }

    public IEnumerable<SelectListItem> Books { get; set; }
}
