using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data;

public class DataContext : IdentityDbContext<User>
{
    public DbSet<Book> Books { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}