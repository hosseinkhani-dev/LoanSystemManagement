using LoanManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Persistance.EF.Loans
{
    public class LoanEntityMap : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.State).IsRequired();
            builder.Property(x => x.LoanStartDate);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Loans)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.LoanType)
                .WithMany(x => x.Loans)
                .HasForeignKey(x => x.LoanTypeId);
        }
    }
}
