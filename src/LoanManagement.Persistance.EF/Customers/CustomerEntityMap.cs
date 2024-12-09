using LoanManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Persistance.EF.Customers
{
    public class CustomerEntityMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(x => x.NationalCode).HasMaxLength(10)
                .IsRequired();
            builder.Property(x => x.Score).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            builder.HasOne(x => x.FinancialInformation)
               .WithOne(x => x.Customer)
               .HasForeignKey<FinancialInformation>(x => x.CustomerId);
        }
    }
}
