using LoanManagement.Entities;
using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

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

        public async Task<GetFinancialInformationDto?> GetByCustomer(int customerId)
        {
            return await _context.FinancialInformations.
                Where(x => x.CustomerId == customerId).
                Select(x => new GetFinancialInformationDto
                {
                    Id = x.Id,
                    MonthlyIncome = x.MonthlyIncome,
                    Job = x.Job,
                    FinancialAssets = x.FinancialAssets,
                }).FirstOrDefaultAsync();
        }
    }
}
