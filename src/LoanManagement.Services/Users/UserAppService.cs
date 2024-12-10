using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Users.Contracts;
using LoanManagement.Services.Users.Contracts.DTOs;
using LoanManagement.Services.Users.Exceptions;

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
            await StopIfFirstNameAndLastNameExist(dto.FirstName, dto.LastName);

            User user = CreateAdminRoleUser(dto);

            await _repository.AddAdmin(user);

            await _unitOfWork.CommitAsync();
        }

        private User CreateAdminRoleUser(AddAdminDto dto)
        {
            return new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = Role.Admin,
            };
        }

        private async Task StopIfFirstNameAndLastNameExist(
            string firstName, string lastName)
        {
            bool isExistByName =
                            await _repository.IsNameExist(firstName, lastName);
            if (isExistByName)
            {
                throw new ThisUserByAdminRoleIsAlreadyExistException();
            }
        }
    }
}
