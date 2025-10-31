using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperLibrary.Api.Data.Entities;

namespace SuperLibrary.Api.Data;

public interface IBookRepository : IGenericRepository<Book>
{
    public IQueryable GetAllWithUsers();

    IEnumerable<SelectListItem> GetComboBooks();
}
