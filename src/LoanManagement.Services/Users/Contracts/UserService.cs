using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Users.Contracts.DTOs;

namespace LoanManagement.Services.Users.Contracts
{
    public interface UserService : Service
    {
        Task AddAdmin(AddAdminDto dto);
    }
}
