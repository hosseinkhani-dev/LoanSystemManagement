using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Loans.Contracts.DTOs;

namespace LoanManagement.Services.Loans.Contracts
{
    public interface LoanRepository : Repository
    {
        Task Add(Loan loan);
        Task<Loan?> FindById(int id);
        Task<List<GetAllLoanActiveReportDto>> 
            GetAllActiveLoansReport();
        Task<List<GetAllLoanDto?>> GetAll();
    }
}
