using LoanManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Persistance.EF.FinancialInformations
{
    public class FinancialInformationEntityMap :
        IEntityTypeConfiguration<FinancialInformation>
    {
        public void Configure(EntityTypeBuilder<FinancialInformation> builder)
        {
            builder.ToTable("FinancialInformations");
            builder.HasKey(x => x.CustomerId);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.MonthlyIncome).IsRequired();
            builder.Property(x => x.Job).IsRequired();
            builder.Property(x => x.FinancialAssets);

            builder.HasOne(x => x.Customer)
                .WithOne(x => x.FinancialInformation)
                .HasForeignKey<FinancialInformation>(x => x.CustomerId);
        }
    }
}
