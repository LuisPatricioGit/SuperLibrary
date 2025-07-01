using System;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Models;

namespace SuperLibrary.Web.Helper;

public interface IConverterHelper
{
    Book ToBook(BookViewModel model, Guid imageId, bool isNew);

    BookViewModel ToBookViewModel(Book book);
}
