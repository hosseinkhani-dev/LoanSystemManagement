using LoanManagement.Entities;
using LoanManagement.Services.FinancialInformations.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Persistance.EF.FinancialInformations
{
    public class EFFianancialInformationRepository :
        FinancialInformationRepository
    {
        private readonly EFDbContext _context;

        public EFFianancialInformationRepository(EFDbContext context)
        {
            _context = context;
        }

        public async Task Add(FinancialInformation financialInformation)
        {
            await _context.FinancialInformations.
                AddAsync(financialInformation);
        }

        public async Task<FinancialInformation?> FindById(int customerId)
        {
            return await _context.FinancialInformations
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        public async Task<bool> IsExistById(int id)
        {
            return await _context.FinancialInformations.
                AnyAsync(x => x.CustomerId == id);
        }
    }
}
