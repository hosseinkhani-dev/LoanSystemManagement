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

        public async Task<bool> IsExistById(int id)
        {
            return await _context.FinancialInformations.
                AnyAsync(x => x.CustomerId == id);
        }
    }
}
