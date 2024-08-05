using CRM.Common.Extentions;
using CRM.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace CRM.DataAccess.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .HasMany(x => x.Sales)
            .WithOne(x => x.Product)
            .HasForeignKey(fk => fk.ProductId)
            .OnDelete(DeleteBehavior.Restrict);


        builder
            .HasQueryFilter(p => !p.IsDeleted);

    }
}
