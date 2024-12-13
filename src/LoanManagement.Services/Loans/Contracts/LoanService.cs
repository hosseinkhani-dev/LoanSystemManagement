using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Loans.Contracts.DTOs;

namespace LoanManagement.Services.Loans.Contracts
{
    public interface LoanService : Service
    {
        Task Add(AddLoanDto dto);
    }
}
