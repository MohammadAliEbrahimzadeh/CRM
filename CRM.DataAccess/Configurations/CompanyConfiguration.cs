using CRM.Common.Extentions;
using CRM.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace CRM.DataAccess.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .HasMany(x => x.Sales)
            .WithOne(x => x.Company)
            .HasForeignKey(fk => fk.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasQueryFilter(p => !p.IsDeleted);

    }
}
