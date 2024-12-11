using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;

namespace LoanManagement.Services.FinancialInformations.Contracts
{
    public interface FinancialInformationService : Service
    {
        Task Add(AddFinancialInformationDto dto);
        Task<GetFinancialInformationDto?> GetByCustomer(int customerId);
    }
}
