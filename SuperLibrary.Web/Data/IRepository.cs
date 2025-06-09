using System.Collections.Generic;
using System.Threading.Tasks;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data
{
    public interface IRepository
    {
        void AddBook(Book book);

        bool BookExists(int id);

        Book GetBook(int id);

        IEnumerable<Book> GetBooks();

        void RemoveBook(Book book);

        Task<bool> SaveAllAsync();

        void UpdateBook(Book book);
    }
}