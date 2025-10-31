using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Api.Data.Entities;

namespace SuperLibrary.Api.Data;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    private readonly DataContext _context;

    public BookRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public IQueryable GetAllWithUsers()
    {
        return _context.Books.Include(p => p.User);
    }

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
