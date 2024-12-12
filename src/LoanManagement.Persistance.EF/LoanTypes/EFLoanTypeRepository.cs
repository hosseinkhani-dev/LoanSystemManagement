using LoanManagement.Entities;
using LoanManagement.Services.LoanTypes.Contracts;
using LoanManagement.Services.LoanTypes.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Persistance.EF.LoanTypes
{
    public class EFLoanTypeRepository : LoanTypeRepository
    {
        private readonly EFDbContext _context;

        public EFLoanTypeRepository(EFDbContext context)
        {
            _context = context;
        }

        public async Task Add(LoanType loanType)
        {
            await _context.AddAsync(loanType);
        }

        public async Task<List<GetAllLoanTypeDto>> GetAll()
        {
            return await _context.LoanTypes.
                Select(x => new GetAllLoanTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    InterestRate = x.InterestRate,
                    MonthlyRepayment = x.MonthlyRepayment,
                    RepaymentPeriod = x.RepaymentPeriod,
                }).ToListAsync();
        }

        public async Task<bool> IsExist(decimal amount, decimal interestRate)
        {
            return await _context.LoanTypes.
                AnyAsync(x => x.Amount == amount &&
                x.InterestRate == interestRate);
        }
    }
}
