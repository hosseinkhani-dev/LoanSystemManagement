using LoanManagement.Entities;

namespace LoanManagement.Tests.Tools
{
    public static class LoanTypeFactory
    {
        public static LoanType CreateLoanType(byte repaymentPeriod)
        {
            return new LoanType
            {
                Name = Generator.GenerateString(),
                Amount = Generator.GenerateDecimal(),
                RepaymentPeriod = repaymentPeriod,
                InterestRate = repaymentPeriod < 12 ? 0.15m : 0.20m,
                MonthlyRepayment = Generator.GenerateDecimal()
            };
        }
    }
}
