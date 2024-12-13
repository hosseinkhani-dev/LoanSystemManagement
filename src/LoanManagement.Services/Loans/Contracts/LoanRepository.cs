using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Loans.Contracts.DTOs;

namespace LoanManagement.Services.Loans.Contracts
{
    public interface LoanRepository : Repository
    {
        Task Add(Loan loan);
        Task<List<GetAllLoanDto?>> GetAll();
    }
}
