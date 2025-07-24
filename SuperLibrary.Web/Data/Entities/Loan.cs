using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SuperLibrary.Web.Data.Entities;

public class Loan : IEntity
{
    public int Id { get; set; }
    public bool WasDeleted { get; set; }

    [Required]
    [Display(Name = "Loan Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
    public DateTime LoanDate { get; set; }

    [Required]
    [Display(Name = "Due Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
    public DateTime DueDate { get; set; }

    [Required]
    public User User { get; set; }

    public IEnumerable<LoanDetail> LoanItems { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}")]
    public int Quantity => LoanItems == null ? 0 : LoanItems.Sum(li => li.Quantity);

    [Display(Name = "Penalty")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal PenaltyPrice => LoanItems == null ? 0 : LoanItems.Sum(li => li.PenaltyPrice);

    [Display(Name = "Days Overdue")]
    [DisplayFormat(DataFormatString = "{0:N0}")]
    public int DaysOverdue => (DateTime.Now - DueDate).Days > 0 ? (DateTime.Now - DueDate).Days : 0;
}
