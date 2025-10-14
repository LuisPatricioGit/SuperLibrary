using System.Linq;
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

    /// <summary>
    /// This method is used to configure the model and its relationships.
    /// </summary>
    /// <param name="modelbuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        var cascadeFKs = modelbuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Configure LoanDetail -> Book relationship
        modelbuilder.Entity<LoanDetail>()
            .HasOne(ld => ld.Book)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        // Configure LoanDetailTemp -> Book relationship
        modelbuilder.Entity<LoanDetailTemp>()
            .HasOne(ldt => ldt.Book)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelbuilder);
    }
}