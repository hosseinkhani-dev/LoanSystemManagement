using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Users.Contracts.DTOs;

namespace LoanManagement.Services.Users.Contracts
{
    public interface UserService : Service
    {
        Task AddAdmin(AddUserDto dto);
        Task AddManager(AddUserDto dto, int id);
        Task<List<GetAllUsersDto>> GetAll();
    }
}
