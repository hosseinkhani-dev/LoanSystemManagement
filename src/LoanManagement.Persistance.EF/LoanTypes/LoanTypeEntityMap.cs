using LoanManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Persistance.EF.LoanTypes
{
    public class LoanTypeEntityMap : IEntityTypeConfiguration<LoanType>
    {
        public void Configure(EntityTypeBuilder<LoanType> builder)
        {
            builder.ToTable("LoanTypes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.Name).HasMaxLength(30).IsRequired();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.InterestRate).IsRequired();
            builder.Property(x => x.RepaymentPeriod).IsRequired();
            builder.Property(x => x.MonthlyRepayment).IsRequired();
        }
    }
}
