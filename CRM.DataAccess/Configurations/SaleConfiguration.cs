using Bogus;
using CRM.Common.Extentions;
using CRM.Models.Enums;
using CRM.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace CRM.DataAccess.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .HasOne(x => x.Company)
            .WithMany(x => x.Sales)
            .HasForeignKey(fk => fk.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        //builder
        //    .HasData(GenerateFakeSales(100));

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Sales)
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.Sales)
            .HasForeignKey(fk => fk.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasQueryFilter(p => !p.IsDeleted);

    }

    private static List<Sale> GenerateFakeSales(int count)
    {
        var userIds = new[] { 1, 2, 3, 4, 8, 9, 16, 17, 18, 19, 20 };

        var saleFaker = new Faker<Sale>()
          .RuleFor(s => s.PricePerProduct, f => f.Finance.Amount(10, 1000))
          .RuleFor(s => s.DiscountPerProduct, f => f.Finance.Amount(0, 100))
          .RuleFor(s => s.Count, f => f.Random.Long(1, 100))
          .RuleFor(s => s.UserId, f => f.PickRandom(userIds))
          .RuleFor(s => s.CompanyId, f => f.Random.Int(1, 100))
          .RuleFor(s => s.ProductId, f => f.Random.Int(101, 200))
          .RuleFor(s => s.SalesStatus, f => f.PickRandom<SalesStatus>());

        return saleFaker.Generate(count);
    }
}
