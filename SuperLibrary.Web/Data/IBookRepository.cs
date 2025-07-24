using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data;

public interface IBookRepository : IGenericRepository<Book>
{
    public IQueryable GetAllWithUsers();

    IEnumerable<SelectListItem> GetComboBooks();
}