using LoanManagement.Entities;
using LoanManagement.Services.Loans.Contracts;

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

       
    }
}
