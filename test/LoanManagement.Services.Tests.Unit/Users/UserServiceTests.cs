using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Users;
using LoanManagement.Services.Users;
using LoanManagement.Services.Users.Contracts;
using LoanManagement.Services.Users.Contracts.DTOs;
using LoanManagement.Services.Users.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Services.Tests.Unit.Users
{
    public class UserServiceTests
    {
        private readonly UserService _sut;
        private readonly EFDbContext _context;
        private readonly UserRepository _repository;
        private readonly UnitOfWork _unitOfWork;

        public UserServiceTests()
        {
            _context = new InMemoryDataBase().CreateDataContext<EFDbContext>();
            _repository = new EFUserRepository(_context);
            _unitOfWork = new EFUnitOfWork(_context);
            _sut = new UserAppService(_repository, _unitOfWork);
        }

        [Fact]
        public async Task AddAdmin_adds_user_with_role_admin_properly()
        {
            AddUserDto dto = CreateAddAdminDto();
            await _sut.AddAdmin(dto);

            var user = await _context.Users.SingleAsync();
            user.FirstName.Should().Be(dto.FirstName);
            user.LastName.Should().Be(dto.LastName);
            user.Role.Should().Be(dto.Role);
        }


        [Fact]
        public async Task AddAdminFails_when_ThisUserByAdminRoleIsAlreadyExistException()
        {
            User user = await CreateAdminRoleUserInDb();

            AddUserDto dto = 
                CreateAdminDtoWithTheSameFirstNameLastNameAsUser(user);
            Func<Task> expected = async () => await _sut.AddAdmin(dto);

            _context.Users.Should().HaveCount(1);
            await expected.Should()
                .ThrowExactlyAsync<
                    ThisUserByAdminRoleIsAlreadyExistException>();
        }

        [Fact]
        public async Task AddManager_adds_manager_by_admin_properly()
        {
            User adminUser = await CreateAdminRoleUserInDb();

            AddUserDto dto = CreatedAddManagerDto();
            await _sut.AddManager(dto, adminUser.Id);

            _context.Users.Should().HaveCount(2);
            var user = await _context.Users.FirstOrDefaultAsync(
                x => x.Role == dto.Role);
            user?.FirstName.Should().Be(dto.FirstName);
            user?.LastName.Should().Be(dto.LastName);
        }

        [Theory]
        [InlineData(1)]
        public async Task AddManagerFails_When_UserForAddingManagerNotFound(
            int fakeId)
        {
            AddUserDto dto = CreatedAddManagerDto();
            Func<Task> expected = async () => 
            await _sut.AddManager(dto, fakeId);

            _context.Users.Should().HaveCount(0);
            await expected.Should()
                .ThrowExactlyAsync<UserForAddingManagerNotFoundException>();
        }

        [Fact]
        public async Task AddManagerFails_When_UserForAddingManagerIsNotAdminException()
        {
            User user = await CreateManagerRoleUserOnDb();

            AddUserDto dto = CreatedAddManagerDto();

            Func<Task> expected = async () =>
           await _sut.AddManager(dto, user.Id);

            _context.Users.Should().HaveCount(1);
            await expected.Should()
                .ThrowExactlyAsync<UserForAddingManagerIsNotAdminException>();
        }

        [Fact]
        public async Task GetAll_returns_all_users_properly()
        {
            User adminUser = await CreateAdminRoleUserInDb();
            User managerUser = await CreateManagerRoleUserOnDb();

            var expected = await _sut.GetAll();

            _context.Users.Should().HaveCount(2);
        }

        private async Task<User> CreateManagerRoleUserOnDb()
        {
            User user = new User()
            {
                FirstName = "dummyFirstName",
                LastName = "dummyLastName",
                Role = Role.Management,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private static AddUserDto CreatedAddManagerDto()
        {
            return new AddUserDto
            {
                FirstName = "dummyFirstName",
                LastName = "dummyLastName",
                Role = Role.Management,
            };
        }

        private static AddUserDto CreateAdminDtoWithTheSameFirstNameLastNameAsUser(User user)
        {
            return new AddUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = Role.Admin,
            };
        }

        private async Task<User> CreateAdminRoleUserInDb()
        {
            var user = new User
            {
                FirstName = "dummy-firstName",
                LastName = "dummy-lastName",
                Role = Role.Admin,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private static AddUserDto CreateAddAdminDto()
        {
            return new AddUserDto
            {
                FirstName = "dummy-firstName",
                LastName = "dummy-lastName",
                Role = Role.Admin,
            };
        }

    }
}
