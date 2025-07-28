using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    private readonly DataContext _context;

    public BookRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all Books with their associated Users.
    /// </summary>
    /// <returns></returns>
    public IQueryable GetAllWithUsers()
    {
        return _context.Books.Include(p => p.User);
    }

    /// <summary>
    /// Gets a list of books for a dropdown or select list.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SelectListItem> GetComboBooks()
    {
        var list = _context.Books.Select(b => new SelectListItem
        {
            Text = b.Title,
            Value = b.Id.ToString()
        }).ToList();

        list.Insert(0, new SelectListItem
        {
            Text = "(Select a book...)",
            Value = "0"
        });

        return list;
    }
}