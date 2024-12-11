using LoanManagement.Entities;
using LoanManagement.Services.FinancialInformations.Contracts;

namespace LoanManagement.Persistance.EF.FinancialInformations
{
    public class EFFinancialInformationRepository :
        FinancialInformationRepository
    {
        private readonly EFDbContext _context;

        public EFFinancialInformationRepository(EFDbContext context)
        {
            _context = context;
        }

        public async Task Add(FinancialInformation financialInformation)
        {
            await _context.FinancialInformations.
                AddAsync(financialInformation);
        }
    }
}
