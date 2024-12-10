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

        public async Task AddAdmin(AddUserDto dto)
        {
            await StopIfFirstNameAndLastNameExist(dto.FirstName, dto.LastName);

            User user = CreateAdminRoleUser(dto);

            await _repository.AddAdmin(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task AddManager(AddUserDto dto, int id)
        {
            User? adminUser = await _repository.FindById(id);

            StopIfAdminUserNotFound(adminUser);

            StopIfItWasNotAdmin(adminUser);

            User user = CreateManagerRoleUser(dto);

            await _repository.AddManager(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<GetAllUsersDto>> GetAll()
        {
            return await _repository.GetAll();
        }

        private static User CreateManagerRoleUser(AddUserDto dto)
        {
            return new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = Role.Management,
            };
        }

        private static void StopIfItWasNotAdmin(User? adminUser)
        {
            if (adminUser!.Role != Role.Admin)
            {
                throw new UserForAddingManagerIsNotAdminException();
            }
        }

        private static void StopIfAdminUserNotFound(User? adminUser)
        {
            if (adminUser == null)
            {
                throw new UserForAddingManagerNotFoundException();
            }
        }

        private User CreateAdminRoleUser(AddUserDto dto)
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
