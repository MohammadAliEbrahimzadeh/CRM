using CRM.Common.Extentions;
using CRM.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace CRM.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.Sales)
            .WithOne(x => x.User)
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasQueryFilter(p => !p.IsDeleted);

        builder
            .Property(x => x.Username)
            .HasMaxLength(256);

        builder
            .Property(x => x.Email)
            .HasMaxLength(256);

        builder
         .Property(e => e.PasswordSalt)
         .HasMaxLength(5);

        builder
           .HasIndex(x => x.Username)
           .IsUnique();

        builder
          .HasIndex(x => x.Email)
          .IsUnique();

        builder.HasData(
               new User
               {
                   Id = 1,
                   Username = "john.doe",
                   Email = "john.doe@example.com",
                   PasswordHash = Generators.HashPassword("hashedPassword1", "abcde"),
                   PasswordSalt = "abcde",
                   IsDeleted = false,
                   CreatedAt = DateTime.Now,
                   UpdatedAt = DateTime.Now
               },
               new User
               {
                   Id = 2,
                   Username = "jane.smith",
                   Email = "jane.smith@example.com",
                   PasswordHash = Generators.HashPassword("hashedPassword2", "fghij"),
                   PasswordSalt = "fghij",
                   IsDeleted = false,
                   CreatedAt = DateTime.Now,
                   UpdatedAt = DateTime.Now
               },
               new User
               {
                   Id = 3,
                   Username = "admin",
                   Email = "admin@example.com",
                   PasswordHash = Generators.HashPassword("hashedPassword3", "klmno"),
                   PasswordSalt = "klmno",
                   IsDeleted = false,
                   CreatedAt = DateTime.Now,
                   UpdatedAt = DateTime.Now
               }
           );

    }
}
