using LoanManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Persistance.EF.Repayments
{
    public class RepaymentEntityMap : IEntityTypeConfiguration<Repayment>
    {
        public void Configure(EntityTypeBuilder<Repayment> builder)
        {
            builder.ToTable("Repayments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.DueDate).IsRequired();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.TotalRepaid).IsRequired();
            builder.Property(x => x.RepaymentCount).IsRequired();
            builder.Property(x => x.TotalLatePenalty).IsRequired();
            builder.Property(x => x.LatePenaltyCount).IsRequired();
            builder.Property(x => x.IsFullyRepaid).IsRequired();
            builder.Property(x => x.IsRepaid).IsRequired();
            builder.Property(x => x.PaymentDate);

            builder.HasOne(x => x.Loan)
                .WithMany(x => x.Repayments)
                .HasForeignKey(x => x.LoanId);
        }
    }
}
