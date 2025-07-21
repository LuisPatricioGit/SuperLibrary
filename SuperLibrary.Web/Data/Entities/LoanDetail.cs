using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace SuperLibrary.Web.Data.Entities;

public class LoanDetail : IEntity
{
    public int Id { get; set; }
    public bool WasDeleted { get; set; }

    [Required]
    public Book Book { get; set; }

    [DisplayFormat(DataFormatString = "{0:N2}")]
    public int Quantity { get; set; }

    [Display(Name = "Penalty")]
    [DisplayFormat(DataFormatString = "0:C2")]
    public int PenaltyPrice => 1 * DaysOverdue;

    [Display(Name = "Days Overdue")]
    [DisplayFormat(DataFormatString = "{0:N2}")]
    public int DaysOverdue { get; set; }
}
