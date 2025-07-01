using System.IO;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Helper;

public class ConverterHelper : IConverterHelper
{
    /// <summary>
    /// Converts BookViewModel to Book format and checks if it's a new book
    /// </summary>
    /// <param name="model"></param>
    /// <param name="isNew"></param>
    /// <returns>Book</returns>
    public Book ToBook(BookViewModel model, string path, bool isNew)
    {
        return new Book
        {
            Id = isNew ? 0 : model.Id,
            ImageUrl = path,
            Title = model.Title,
            Author = model.Author,
            Publisher = model.Publisher,
            ReleaseYear = model.ReleaseYear,
            Copies = model.Copies,
            GenreId = model.GenreId,
            IsAvailable = model.IsAvailable,
            WasDeleted = model.WasDeleted,
            User = model.User,
        };
    }

    /// <summary>
    /// Converts Book to BookViewModel format
    /// </summary>
    /// <param name="book"></param>
    /// <returns>BookViewModel</returns>
    public BookViewModel ToBookViewModel(Book book)
    {
        return new BookViewModel
        {
            Id = book.Id,
            ImageUrl = book.ImageUrl,
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            ReleaseYear = book.ReleaseYear,
            Copies = book.Copies,
            GenreId = book.GenreId,
            IsAvailable = book.IsAvailable,
            WasDeleted = book.WasDeleted,
            User = book.User,
        };
    }
}
