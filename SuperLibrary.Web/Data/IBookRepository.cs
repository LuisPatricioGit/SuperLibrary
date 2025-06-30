using System.Linq;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data;

public interface IBookRepository : IGenericRepository<Book>
{
   public IQueryable GetAllWithUsers();
}