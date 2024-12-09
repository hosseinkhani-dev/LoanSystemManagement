using LoanManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Persistance.EF.Customers
{
    public class CustomerEntityMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.FirstName).HasMaxLength(30).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(30).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(10).IsRequired();
            builder.Property(x => x.NationalCode).HasMaxLength(10)
                .IsRequired();
            builder.Property(x => x.Email).HasMaxLength(100);
            builder.Property(x => x.Score).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            builder.HasOne(x => x.FinancialInformation)
               .WithOne(x => x.Customer)
               .HasForeignKey<FinancialInformation>(x => x.CustomerId);
        }
    }
}
