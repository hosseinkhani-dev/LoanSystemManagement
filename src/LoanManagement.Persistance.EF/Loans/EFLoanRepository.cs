using LoanManagement.Entities;
using LoanManagement.Services.Loans.Contracts;
using LoanManagement.Services.Loans.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Persistance.EF.Loans
{
    public class EFLoanRepository : LoanRepository
    {
        private readonly EFDbContext _context;

        public EFLoanRepository(EFDbContext context)
        {
            _context = context;
        }

        public async Task Add(Loan loan)
        {
            await _context.Loans.AddAsync(loan);
        }

        public async Task<List<GetAllLoanDto?>> GetAll()
        {
            return await _context.Loans.
                Select(x => x == null ? null : 
                 new GetAllLoanDto
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    LoanTypeId = x.LoanTypeId,
                    LoanStartDate = x.LoanStartDate,
                    State = x.State.ToString()
                }).ToListAsync();
        }
    }
}
