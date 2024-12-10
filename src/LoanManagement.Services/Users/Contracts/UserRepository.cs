using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;

namespace LoanManagement.Services.Users.Contracts
{
    public interface UserRepository : Repository
    {
        Task AddAdmin(User user);
        Task<bool> IsNameExist(string firstName, string lastName);
    }
}
