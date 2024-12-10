using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Users;
using LoanManagement.Services.Users;
using LoanManagement.Services.Users.Contracts;
using LoanManagement.Services.Users.Contracts.DTOs;
using LoanManagement.Services.Users.Exceptions;
using LoanManagement.Tests.Spec.Infrastructions;
using static LoanManagement.Tests.Spec.Infrastructions.BDDHelper;

namespace LoanManagement.Tests.Spec.Users
{
    [Scenario("تعریف ادمین هنگامی که نام و نام خانوادگی وحود دارد")]
    public class AddAdminWhenNameExist : EFDatabaseFixture
    {
        private readonly UserService _sut;
        private readonly EFDbContext _context;
        private readonly UserRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private User _user;
        private AddAdminDto _dto;
        Func<Task> expected;

        public AddAdminWhenNameExist(ConfigurationFixture configurationFixture)
            : base(configurationFixture)
        {
            _context = CreateDataContext();
            _repository = new EFUserRepository(_context);
            _unitOfWork = new EFUnitOfWork(_context);
            _sut = new UserAppService(_repository, _unitOfWork);
        }

        [Given("کاربری به نام حسین" +
            "و نام خانوادگی خانی" +
            "و نقش ادمین" +
            "در فهرست کاربران وجود دارد")]
        private async Task Given()
        {
            await CreateAdminRoleUserInDb();
        }

        [When("کاربری با نام حسین" +
            "و نام خانوادگی خانی" +
            "و نقش ادمین ثبت میکنم")]
        private async Task When()
        {
            CreateAddAdminDto();
            expected = () => _sut.AddAdmin(_dto);
        }

        [Then("تنها یک کاربر در فهرست کاربران باید وجود داشته باشد")]
        private void Then()
        {
            _context.Users.Should().HaveCount(1);
        }

        [And("باید خطایی با عنوان نام کاربر برای نقش ادمین وجود دارد رخ دهد")]
        private async Task And()
        {
            await expected.Should()
                .ThrowExactlyAsync<
                    ThisUserByAdminRoleIsAlreadyExistException>();
        }

        [Fact]
        public async Task Run()
        {
            await Given();
            await When();
            Then();
            await And();
        }

        private async Task CreateAdminRoleUserInDb()
        {
            _user = new User
            {
                FirstName = "حسین",
                LastName = "خانی",
                Role = Role.Admin,
            };
            await _context.Users.AddAsync(_user);
            await _context.SaveChangesAsync();
        }

        private void CreateAddAdminDto()
        {
            _dto = new AddAdminDto
            {
                FirstName = "حسین",
                LastName = "خانی",
                Role = Role.Admin,
            };
        }
    }
}
