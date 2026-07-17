using Finance.Api.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Infrastructure.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //indexes for unique constraints and performance optimization
        modelBuilder.Entity<AppUser>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(c => new { c.UserId, c.Name }).IsUnique();
        modelBuilder.Entity<Expense>().HasIndex(e => e.ExpenseDate);
        modelBuilder.Entity<Expense>().HasOne(e => e.User).WithMany(u => u.Expenses).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Expense>().HasOne(e => e.Category).WithMany(c => c.Expenses).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}
