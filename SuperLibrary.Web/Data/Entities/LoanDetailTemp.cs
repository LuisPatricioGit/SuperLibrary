﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace SuperLibrary.Web.Data.Entities;

public class LoanDetailTemp : IEntity
{
    public int Id { get; set; }

    [Required]
    public User User { get; set; }

    [Required]
    public Book Book { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}")]
    public int Quantity { get; set; }

    public bool WasDeleted { get; set; }
}
