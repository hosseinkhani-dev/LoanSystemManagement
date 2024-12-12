using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;

namespace LoanManagement.Services.FinancialInformations.Contracts
{
    public interface FinancialInformationRepository : Repository
    {
        Task Add(FinancialInformation financialInformation);
        Task<FinancialInformation?> FindById(int customerId);
        Task<bool> IsExistById(int id);
    }
}
