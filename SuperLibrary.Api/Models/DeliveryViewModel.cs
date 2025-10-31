using System.ComponentModel.DataAnnotations;
using System;

namespace SuperLibrary.Api.Models;

public class DeliveryViewModel
{
    public int Id { get; set; }

    [Display(Name = "Delivery Date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime DeliveryDate { get; set; }
}
