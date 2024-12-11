using LoanManagement.Infrastructures.Applications;

namespace LoanManagement.Services.FinancialInformations.Contracts
{
    public interface FinancialInformationRepository : Repository
    {
        Task<bool> IsExistById(int id);
    }
}
