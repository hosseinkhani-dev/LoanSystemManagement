using LoanManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Persistance.EF.Users
{
    public class UserEntityMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.FirstName).HasMaxLength(30).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(30).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(10).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.Role).IsRequired();

            builder.HasDiscriminator<Role>("Role")
                .HasValue<User>(Role.Admin)
                .HasValue<User>(Role.Management)
                .HasValue<Customer>(Role.Customer);
               
        }
    }
}
