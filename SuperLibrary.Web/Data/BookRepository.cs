using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(DataContext context) : base(context)
    {
    }
}