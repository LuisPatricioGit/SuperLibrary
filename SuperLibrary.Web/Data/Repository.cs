using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data
{
    public class Repository : IRepository
    {
        private readonly DataContext _context;

        public Repository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Book> GetBooks()
        {
            return _context.Books.OrderBy(b => b.Title);
        }

        public Book GetBook(int id)
        {
            return _context.Books.Find(id);
        }

        public void AddBook(Book book)
        {
            _context.Books.Add(book);
        }

        public void UpdateBook(Book book)
        {
            _context.Books.Update(book);
        }

        public void RemoveBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool BookExists(int id)
        {
            return _context.Books.Any(b => b.Id == id);
        }
    }
}
