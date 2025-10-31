using System;

namespace SuperLibrary.Api.Models.API
{
    public class BookApiViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public DateTime? ReleaseYear { get; set; }
        public int Copies { get; set; }
        public int GenreId { get; set; }
        public string ImageFullPath { get; set; }
        public bool IsAvailable { get; set; }
        public string UserName { get; set; }
    }
}
