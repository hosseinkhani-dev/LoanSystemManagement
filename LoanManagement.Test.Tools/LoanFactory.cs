using LoanManagement.Entities;

namespace LoanManagement.Tests.Tools
{
    public static class LoanFactory
    {
        public static Loan CreateLoan(
            int customerId, int loanTypeId, LoanState state)
        {
            return new Loan
            {
                CustomerId = customerId,
                LoanTypeId = loanTypeId,
                State = state
            };
        }
    }
}
