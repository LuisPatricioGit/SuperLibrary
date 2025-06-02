using Microsoft.EntityFrameworkCore;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}
