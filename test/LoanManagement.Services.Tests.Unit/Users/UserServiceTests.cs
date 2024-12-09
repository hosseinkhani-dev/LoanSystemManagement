using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Users;
using LoanManagement.Services.Users;
using LoanManagement.Services.Users.Contracts;
using LoanManagement.Services.Users.Contracts.DTOs;
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
            var dto = new AddAdminDto
            {
                FirstName = "dummy-firstName",
                LastName = "dummy-lastName",
                Role = Role.Admin,
            };

            await _sut.AddAdmin(dto);

            var user = await _context.Users.SingleAsync();
            user.FirstName.Should().Be(dto.FirstName);
            user.LastName.Should().Be(dto.LastName);
            user.Role.Should().Be(dto.Role);
        }
    }
}
