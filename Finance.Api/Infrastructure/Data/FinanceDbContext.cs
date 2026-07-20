using Finance.Api.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Infrastructure.Data;

public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ArgumentNullException.ThrowIfNull(modelBuilder);
        //indexes for unique constraints and performance optimization
        _ = modelBuilder.Entity<AppUser>().HasIndex(u => u.Email).IsUnique();
        _ = modelBuilder.Entity<Category>().HasIndex(c => new { c.UserId, c.Name }).IsUnique();
        _ = modelBuilder.Entity<Expense>().HasIndex(e => e.ExpenseDate);
        _ = modelBuilder.Entity<Expense>().HasOne(e => e.User).WithMany(u => u.Expenses).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        _ = modelBuilder.Entity<Expense>().HasOne(e => e.Category).WithMany(c => c.Expenses).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}
