using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data;

public class DataContext : IdentityDbContext<User>
{
    public DbSet<Book> Books { get; set; }

    public DbSet<Loan> Loans { get; set; }

    public DbSet<LoanDetail> LoanDetails { get; set; }

    public DbSet<LoanDetailTemp> LoanDetailsTemp { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}