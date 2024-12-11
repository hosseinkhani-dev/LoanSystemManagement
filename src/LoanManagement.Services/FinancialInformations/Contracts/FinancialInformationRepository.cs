using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;

namespace LoanManagement.Services.FinancialInformations.Contracts
{
    public interface FinancialInformationRepository : Repository
    {
        Task Add(FinancialInformation financialInformation);
        Task<GetFinancialInformationDto?> GetByCustomer(int customerId);
    }
}
