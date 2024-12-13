using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;

namespace LoanManagement.Services.Loans.Contracts
{
    public interface LoanRepository : Repository
    {
        Task Add(Loan loan);
    }
}
