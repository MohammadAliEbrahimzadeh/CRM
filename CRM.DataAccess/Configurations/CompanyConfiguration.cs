using Bogus;
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
            .Property(x => x.NationalCode)
            .HasMaxLength(11)
            .IsFixedLength();

        builder
            .HasData(GenerateFakeCompanies(100));

        builder
            .HasMany(x => x.Sales)
            .WithOne(x => x.Company)
            .HasForeignKey(fk => fk.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasQueryFilter(p => !p.IsDeleted);

    }


    private static List<Company> GenerateFakeCompanies(int count)
    {
        var productFaker = new Faker<Company>()
            .RuleFor(x => x.Address, fk => fk.Address.FullAddress())
            .RuleFor(x => x.Id, fk => fk.IndexFaker + 1)
            .RuleFor(x => x.Email, fk => fk.Person.Email)
            .RuleFor(x => x.Name, fk => fk.Company.CompanyName())
            .RuleFor(x => x.NationalCode, fk => fk.Random.String2(11, "0123456789"));

        return productFaker.Generate(count);
    }
}
