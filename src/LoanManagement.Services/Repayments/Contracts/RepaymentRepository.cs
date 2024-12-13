using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Repayments.Contracts.DTOs;

namespace LoanManagement.Services.Repayments.Contracts
{
    public interface RepaymentRepository : Repository
    {
        Task<Repayment?> FindById(int id);
        Task GenerateRepayments(Repayment repayment);
        Task<List<GetAllUnpaidRepaymentsDto?>> GetAllUnpaid(int loanId);
    }
}
