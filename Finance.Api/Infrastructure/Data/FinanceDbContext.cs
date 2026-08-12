using Finance.Api.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Infrastructure.Data;

public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<AppUser>().ToContainer("Users").HasPartitionKey(u => u.Id);

        _ = modelBuilder.Entity<Category>().ToContainer("Category").HasPartitionKey(c => c.UserId);

        _ = modelBuilder.Entity<Expense>().ToContainer("Expense").HasPartitionKey(e => e.UserId);

        _ = modelBuilder.Entity<RefreshToken>().ToContainer("RefreshToken").HasPartitionKey(x => x.UserId);

        base.OnModelCreating(modelBuilder);
    }
}
