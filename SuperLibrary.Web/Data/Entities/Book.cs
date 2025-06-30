using System;
using System.ComponentModel.DataAnnotations;

namespace SuperLibrary.Web.Data.Entities;

public class Book : IEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required]
    [MaxLength(50)]
    public string Author { get; set; }

    [Required]
    [MaxLength(50)]
    public string Publisher { get; set; }

    //[DisplayFormat(DataFormatString = "{YYYY}", ApplyFormatInEditMode = false)]
    [Display(Name = "Published Year")]
    public DateTime? ReleaseYear { get; set; }

    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
    public double Copies { get; set; }

    [Display(Name = "Genre")]
    public int GenreId { get; set; }

    [Display(Name = "Image")]
    public string ImageUrl { get; set; }

    [Display(Name = "Is Available")]
    public bool IsAvailable { get; set; }

    public bool WasDeleted { get; set; }

    public User User { get; set; }

    public string ImageFullPath
    {
        get
        {
            if (string.IsNullOrEmpty(ImageUrl))
            {
                return null;
            }

            return $"https://localhost:44353{ImageUrl.Substring(1)}";
        }
    }
}