using Bogus;
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
            .HasData(GenerateFakeProducts(100));

        builder
            .HasMany(x => x.Sales)
            .WithOne(x => x.Product)
            .HasForeignKey(fk => fk.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasQueryFilter(p => !p.IsDeleted);

    }


    private static List<Product> GenerateFakeProducts(int count)
    {
        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Id, f => f.IndexFaker + 1)
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => f.Finance.Amount())
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(2))
            .RuleFor(p => p.UpdatedAt, f => f.Date.Recent())
            .RuleFor(p => p.IsDeleted, f => false);

        return productFaker.Generate(count);
    }
}
