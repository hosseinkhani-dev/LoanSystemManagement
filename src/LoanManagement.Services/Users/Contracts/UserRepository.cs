using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Users.Contracts.DTOs;

namespace LoanManagement.Services.Users.Contracts
{
    public interface UserRepository : Repository
    {
        Task AddAdmin(User user);
        Task AddManager(User user);
        Task<User?> FindById(int id);
        Task<List<GetAllUsersDto>> GetAll();
        Task<bool> IsNameExist(string firstName, string lastName);
    }
}
