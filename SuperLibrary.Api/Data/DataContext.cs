using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Api.Data.Entities;

namespace SuperLibrary.Api.Data;

public class DataContext : IdentityDbContext<User>
{
    public DbSet<Book> Books { get; set; }

    public DbSet<Loan> Loans { get; set; }

    public DbSet<LoanDetail> LoanDetails { get; set; }

    public DbSet<LoanDetailTemp> LoanDetailsTemp { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

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

        modelbuilder.Entity<LoanDetail>()
            .HasOne(ld => ld.Book)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelbuilder.Entity<LoanDetailTemp>()
            .HasOne(ldt => ldt.Book)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelbuilder);
    }
}
