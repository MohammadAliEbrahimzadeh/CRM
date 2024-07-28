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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
    }
}
