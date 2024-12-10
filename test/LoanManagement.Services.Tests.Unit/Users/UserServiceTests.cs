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
            AddAdminDto dto = CreateAddAdminDto();
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

            AddAdminDto dto = 
                CreateAdminDtoWithTheSameFirstNameLastNameAsUser(user);
            Func<Task> expected = () => _sut.AddAdmin(dto);

            _context.Users.Should().HaveCount(1);
            await expected.Should()
                .ThrowExactlyAsync<
                    ThisUserByAdminRoleIsAlreadyExistException>();
        }

        private static AddAdminDto CreateAdminDtoWithTheSameFirstNameLastNameAsUser(User user)
        {
            return new AddAdminDto
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

        private static AddAdminDto CreateAddAdminDto()
        {
            return new AddAdminDto
            {
                FirstName = "dummy-firstName",
                LastName = "dummy-lastName",
                Role = Role.Admin,
            };
        }

    }
}
