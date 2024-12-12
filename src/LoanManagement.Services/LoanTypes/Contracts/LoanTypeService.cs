using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.LoanTypes.Contracts.DTOs;

namespace LoanManagement.Services.LoanTypes.Contracts
{
    public interface LoanTypeService : Service
    {
        Task Add(AddLoanTypeDto dto);
        Task<List<GetAllLoanTypeDto>> GetAll();
    }
}
