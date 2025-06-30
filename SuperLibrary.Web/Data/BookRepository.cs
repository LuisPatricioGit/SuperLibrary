using System.Linq;
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

    public IQueryable GetAllWithUsers()
    {
        return _context.Books.Include(p => p.User);
    }
}