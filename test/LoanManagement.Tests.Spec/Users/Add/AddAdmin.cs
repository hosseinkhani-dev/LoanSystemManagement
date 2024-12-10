using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Users;
using LoanManagement.Services.Users;
using LoanManagement.Services.Users.Contracts;
using LoanManagement.Services.Users.Contracts.DTOs;
using LoanManagement.Tests.Spec.Infrastructions;
using Microsoft.EntityFrameworkCore;
using static LoanManagement.Tests.Spec.Infrastructions.BDDHelper;

namespace LoanManagement.Tests.Spec.Users.Add
{
    [Scenario("تعریف مشتری")]
    public class AddAdmin : EFDatabaseFixture
    {
        private readonly UserService _sut;
        private readonly EFDbContext _context;
        private readonly UserRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private AddUserDto _dto;

        public AddAdmin(ConfigurationFixture configurationFixture)
            : base(configurationFixture)
        {
            _context = CreateDataContext();
            _repository = new EFUserRepository(_context);
            _unitOfWork = new EFUnitOfWork(_context);
            _sut = new UserAppService(_repository, _unitOfWork);
        }

        [Given("هیچ کاربری در فهرست کاربران وجود ندارد")]
        private void Given() { }

        [When("کاربری با نام حسین و" +
            " نام خانوادگی خانی و" +
            " نقش ادمین ثبت میکنیم")]
        private async Task When()
        {
            CreateAddAdminDto();
            await _sut.AddAdmin(_dto);
        }

        [Then("کاربری با اسم حسین و" +
            " نام خانوادگی خانی " +
            "و نقش ادمین" +
            " باید در فهرست کاربران وجود داشته باشد")]
        private async Task Then()
        {
            var user = await _context.Users.SingleAsync();
            user.FirstName.Should().Be(_dto.FirstName);
            user.LastName.Should().Be(_dto.LastName);
            user.Role.Should().Be(_dto.Role);
        }

        [Fact]
        public async Task Run()
        {
            await When();
            await Then();
        }

        private void CreateAddAdminDto()
        {
            _dto = new AddUserDto
            {
                FirstName = "حسین",
                LastName = "خانی",
                Role = Role.Admin,
            };
        }

    }
}
