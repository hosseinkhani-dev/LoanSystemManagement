using LoanManagement.Entities;

namespace LoanManagement.Tests.Tools
{
    public class RepaymentBuilder
    {
        private readonly Repayment _repayment;

        public RepaymentBuilder(int loanId)
        {
            _repayment = new Repayment
            {
                LoanId = loanId,
            };
        }

        public RepaymentBuilder DueDate(DateTime dueDate)
        {
            _repayment.DueDate = dueDate;
            return this;
        }

        public RepaymentBuilder Amount(decimal amount)
        {
            _repayment.Amount = amount;
            return this;
        }

        public RepaymentBuilder TotalRepaid(decimal totalRepaid)
        {
            _repayment.TotalRepaid = totalRepaid;
            return this;
        }

        public RepaymentBuilder RepaymentCount(byte repaymentCount)
        {
            _repayment.RepaymentCount = repaymentCount;
            return this;
        }

        public RepaymentBuilder TotalLatePenalty(decimal totalLatePenalty)
        {
            _repayment.TotalLatePenalty = totalLatePenalty;
            return this;
        }
        public RepaymentBuilder LatePenaltyCount(byte latePenaltyCount)
        {
            _repayment.LatePenaltyCount = latePenaltyCount;
            return this;
        }
        public RepaymentBuilder IsFullyRepaid(bool isFullyRepaid)
        {
            _repayment.IsFullyRepaid = isFullyRepaid;
            return this;
        }
        public RepaymentBuilder IsRepaid(bool isRepaid)
        {
            _repayment.IsRepaid = isRepaid;
            return this;
        }
        public RepaymentBuilder PaymentDate(DateTime paymentDate)
        {
            _repayment.PaymentDate = paymentDate;
            return this;
        }

        public Repayment Build()
        {
            return _repayment;
        }
    }
}
