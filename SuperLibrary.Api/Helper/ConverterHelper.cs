using System;
using SuperLibrary.Api.Data.Entities;
using SuperLibrary.Api.Models;

namespace SuperLibrary.Api.Helper;

public class ConverterHelper : IConverterHelper
{
    public Book ToBook(BookViewModel model, Guid imageId, bool isNew)
    {
        return new Book
        {
            Id = isNew ? 0 : model.Id,
            ImageId = imageId,
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

    public BookViewModel ToBookViewModel(Book book)
    {
        return new BookViewModel
        {
            Id = book.Id,
            ImageId = book.ImageId,
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
