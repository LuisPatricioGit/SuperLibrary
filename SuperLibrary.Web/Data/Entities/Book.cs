using System;
using System.ComponentModel.DataAnnotations;

namespace SuperLibrary.Web.Data.Entities
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        //[DisplayFormat(DataFormatString = "{YYYY}", ApplyFormatInEditMode = false)]
        [Display(Name = "Published Year")]
        public DateTime ReleaseYear { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double Copies { get; set; }

        public int GenreId { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }
    }
}
