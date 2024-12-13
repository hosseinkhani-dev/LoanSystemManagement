using LoanManagement.Entities;

namespace LoanManagement.Tests.Tools
{
    public static class RepaymentFactory
    {
        public static Repayment GenerateRepayment(int loanId)
        {
            return new Repayment()
            {
                DueDate = DateTime.Now,
                Amount = 1,
                LoanId = loanId,
                TotalRepaid = 0,
                RepaymentCount = 0,
                TotalLatePenalty = 0,
                LatePenaltyCount = 0,
                IsFullyRepaid = false,
                IsRepaid = false
            };
        }
    }
}
