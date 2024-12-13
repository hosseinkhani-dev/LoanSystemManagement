using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Repayments.Contracts.DTOs;

namespace LoanManagement.Services.Repayments.Contracts
{
    public interface RepaymentService : Service
    {
        Task GenerateRepayments(int loanId);
        Task<List<GetAllUnpaidRepaymentsDto?>> GetAllUnpaid(int loanId);
        Task PayRepayment(int id);
    }
}
