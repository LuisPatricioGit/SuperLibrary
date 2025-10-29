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

    [Display(Name = "Year")]
    [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = false)]
    public DateTime? ReleaseYear { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
    public int Copies { get; set; }

    [Display(Name = "Genre")]
    public int GenreId { get; set; }

    [Display(Name = "Image")]
    public Guid ImageId { get; set; }

    [Display(Name = "Available")]
    public bool IsAvailable { get; set; }

    public bool WasDeleted { get; set; }

    public User User { get; set; }

    public string ImageFullPath => ImageId == Guid.Empty
           ? $"https://superlibrary-d7cmb3geg9d8dab7.westeurope-01.azurewebsites.net/images/noimage.png"
           : $"https://superlibrary.blob.core.windows.net/books/{ImageId}";
           // $"https://localhost:44353/images/noimage.png" // Local version
}