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

        public async Task<Loan?> FindById(int id)
        {
            return await _context.Loans.
                Include(x => x.Customer).
                Include(x => x.LoanType).
                Include(x => x.Repayments)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<GetAllLoanActiveReportDto>>
            GetAllActiveLoansReport()
        {
            return await _context.Loans
        .Where(loan => loan.State == LoanState.Repaying ||
         loan.State == LoanState.DelayInRepayment)
        .Select(loan => new GetAllLoanActiveReportDto
        {
            LoanId = loan.Id,
            LoanState = loan.State.ToString(),
            TotalRepaid = loan.Repayments
            .Where(x => x.IsRepaid && x.LoanId == loan.Id)
            .OrderByDescending(r => r.PaymentDate)
            .Select(r => r.TotalRepaid)
            .FirstOrDefault(),
            RemainingRepayments = loan.Repayments
            .Count(x => x.IsRepaid == false && x.LoanId == loan.Id),
        }).ToListAsync();
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
