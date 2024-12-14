using LoanManagement.Entities;

namespace LoanManagement.Tests.Tools
{
    public class LoanTypeBuilder
    {
        private readonly LoanType _loanType;

        public LoanTypeBuilder()
        {
            _loanType = new LoanType();
        }

        public LoanTypeBuilder WithName(string name)
        {
            _loanType.Name = name;
            return this;
        }

        public LoanTypeBuilder WithAmount(decimal amount)
        {
            _loanType.Amount = amount;
            return this;
        }

        public LoanTypeBuilder WithInterestRate(decimal interestRate)
        {
            _loanType.InterestRate = interestRate;
            return this;
        }

        public LoanTypeBuilder WithRepaymentPeriod(byte repaymentPeriod)
        {
            _loanType.RepaymentPeriod = repaymentPeriod;
            return this;
        }

        public LoanTypeBuilder WithMonthlyRepayment(decimal monthlyRepayment)
        {
            _loanType.MonthlyRepayment = monthlyRepayment;
            return this;
        }

        public LoanType Build()
        {
            return _loanType;
        }
    }
}
