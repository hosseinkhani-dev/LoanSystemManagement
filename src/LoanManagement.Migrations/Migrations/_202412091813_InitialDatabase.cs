using FluentMigrator;

namespace LoanManagement.Migrations.Migrations
{
    [Migration(202412091813)]
    public class _202412091813_InitialDatabase : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("FirstName").AsString(30).NotNullable()
            .WithColumn("LastName").AsString(30).NotNullable()
            .WithColumn("Role").AsByte().NotNullable();

            Create.Table("Customers")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("FirstName").AsString(30).NotNullable()
            .WithColumn("LastName").AsString(30).NotNullable()
            .WithColumn("PhoneNumber").AsString(10).NotNullable()
            .WithColumn("Email").AsString(200).Nullable()
            .WithColumn("NationalCode").AsString(10).NotNullable()
            .WithColumn("Score").AsInt32().NotNullable()
            .WithColumn("IsActive").AsBoolean().NotNullable();

            Create.Table("FinancialInformations")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("MonthlyIncome").AsDecimal().NotNullable()
            .WithColumn("Job").AsString(20).NotNullable()
            .WithColumn("FinancialAssets").AsDecimal().Nullable()
            .WithColumn("CustomerId").AsInt32().NotNullable()
            .ForeignKey("FK_FinancialInformations_Customers", "Customers", "Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade).Unique();

            Create.Table("LoanTypes")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(30).NotNullable()
            .WithColumn("Amount").AsDecimal().NotNullable()
            .WithColumn("InterestRate").AsDecimal().NotNullable()
            .WithColumn("RepaymentPeriod").AsByte().NotNullable()
            .WithColumn("MonthlyRepayment").AsDecimal().NotNullable();

            Create.Table("Loans")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("State").AsByte().NotNullable()
            .WithColumn("LoanStartDate").AsDateTime().Nullable()
            .WithColumn("CustomerId").AsInt32().NotNullable()
            .ForeignKey("FK_Loans_Customers", "Customers", "Id")
            .OnDelete(System.Data.Rule.Cascade)
            .WithColumn("LoanTypeId").AsInt32().NotNullable()
            .ForeignKey("FK_Loans_LoanTypes", "LoanTypes", "Id")
            .OnDelete(System.Data.Rule.Cascade);

            Create.Table("Repayments")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("DueDate").AsDateTime().NotNullable()
            .WithColumn("Amount").AsDecimal().NotNullable()
            .WithColumn("TotalRepaid").AsDecimal().NotNullable()
            .WithColumn("RepaymentCount").AsByte().NotNullable()
            .WithColumn("TotalLatePenalty").AsDecimal().NotNullable()
            .WithColumn("LatePenaltyCount").AsByte().NotNullable()
            .WithColumn("IsFullyRepaid").AsBoolean().NotNullable()
            .WithColumn("IsRepaid").AsBoolean().NotNullable()
            .WithColumn("PaymentDate").AsDateTime().Nullable()
            .WithColumn("LoanId").AsInt32().NotNullable()
            .ForeignKey("FK_Repayments_Loans", "Loans", "Id")
            .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("Repayments");
            Delete.Table("Loans");
            Delete.Table("LoanTypes");
            Delete.Table("FinancialInformations");
            Delete.Table("Customers");
            Delete.Table("Users");
        }
    }
}
