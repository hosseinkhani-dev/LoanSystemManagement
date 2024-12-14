using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Loans.Contracts.DTOs;

namespace LoanManagement.Services.Loans.Contracts
{
    public interface LoanService : Service
    {
        Task Add(AddLoanDto dto);
        Task<List<GetAllLoanActiveReportDto>> 
            GetAllActiveLoansReport();
        Task<List<GetAllLoanDto?>> GetAll();
        Task<string> LoanCheck(int id);
    }
}
