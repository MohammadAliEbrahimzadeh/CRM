using Bogus;
using CRM.DataAccess.Configurations;
using CRM.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM.DataAccess.Context;

public class CRMContext : DbContext
{
    public CRMContext(DbContextOptions<CRMContext> options) : base(options)
    {
        
    }

    public DbSet<User>? Users { get; set; }
    public DbSet<Role>? Roles { get; set; }
    public DbSet<UserRole>? UserRoles { get; set; }
    public DbSet<Sale>? Sales { get; set; }
    public DbSet<Product>? Products { get; set; }
    public DbSet<Company>? Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new SaleConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}
