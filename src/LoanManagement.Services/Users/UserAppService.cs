using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Users.Contracts;
using LoanManagement.Services.Users.Contracts.DTOs;

namespace LoanManagement.Services.Users
{
    public class UserAppService : UserService
    {
        private readonly UserRepository _repository;
        private readonly UnitOfWork _unitOfWork;

        public UserAppService(UserRepository repository,
            UnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddAdmin(AddAdminDto dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = Role.Admin,
            };

            await _repository.AddAdmin(user);

            await _unitOfWork.CommitAsync();
        }
    }
}
