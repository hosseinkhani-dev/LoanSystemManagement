using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.LoanTypes.Contracts.DTOs;

namespace LoanManagement.Services.LoanTypes.Contracts
{
    public interface LoanTypeRepository : Repository
    {
        Task Add(LoanType loanType);
        Task<LoanType?> FindById(int id);
        Task<List<GetAllLoanTypeDto>> GetAll();
        Task<bool> IsExist(decimal amount, decimal interestRate);
    }
}
