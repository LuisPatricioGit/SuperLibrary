using System;
using SuperLibrary.Api.Data.Entities;
using SuperLibrary.Api.Models;

namespace SuperLibrary.Api.Helper;

public interface IConverterHelper
{
    Book ToBook(BookViewModel model, Guid imageId, bool isNew);

    BookViewModel ToBookViewModel(Book book);
}
