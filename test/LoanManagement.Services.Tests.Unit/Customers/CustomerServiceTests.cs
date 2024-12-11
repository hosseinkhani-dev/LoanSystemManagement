using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Customers;
using LoanManagement.Services.Customers;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Contracts.DTOs;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Tests.Tools;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LoanManagement.Services.Tests.Unit.Customers
{
    public class CustomerServiceTests
    {
        private readonly CustomerService _sut;
        private readonly CustomerRepository _repository;
        private readonly EFDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private Mock<FinancialInformationRepository> mockFinancialInfoRepo;

        public CustomerServiceTests()
        {
            _context = new InMemoryDataBase().CreateDataContext<EFDbContext>();
            _repository = new EFCustomerRepository(_context);
            _unitOfWork = new EFUnitOfWork(_context);
            mockFinancialInfoRepo = new Mock<FinancialInformationRepository>();
            _sut = new CustomerAppService(
                _repository,
                _unitOfWork,
                mockFinancialInfoRepo.Object);
        }

        [Fact]
        public async Task Add_adds_a_Customer_properly()
        {
            AddCustomerDto dto = CreateAddCustomerDtoWithDummyValues();
            await _sut.Add(dto);

            Customer customer = await _context.Customers.SingleAsync();
            customer.FirstName.Should().Be(dto.FirstName);
            customer.LastName.Should().Be(dto.LastName);
            customer.PhoneNumber.Should().Be(dto.PhoneNumber);
            customer.NationalCode.Should().Be(dto.NationalCode);
            customer.Score.Should().Be(0);
            customer.Email.Should().BeNull();
        }

        [Fact]
        public async Task AddFails_When_PhoneNumberExistException()
        {
            Customer customer = await CreateCustomerWithdummyValuesOnDb();

            var dto = new AddCustomerDto()
            {
                FirstName = "dummy-firstName",
                LastName = "dummy-lastName",
                PhoneNumber = customer.PhoneNumber,
                NationalCode = "1234567891"
            };
            Func<Task> expected = async () => await _sut.Add(dto);

            _context.Customers.Should().HaveCount(1);
            await expected.Should()
                .ThrowExactlyAsync<PhoneNumberExistException>();
        }

        [Fact]
        public async Task AddFails_When_NationalCodeExistException()
        {
            Customer customer = await CreateCustomerWithdummyValuesOnDb();

            var dto = new AddCustomerDto()
            {
                FirstName = "dummy-firstName",
                LastName = "dummy-lastName",
                PhoneNumber = "dummy-pnoneNumber",
                NationalCode = customer.NationalCode,
            };
            Func<Task> expected = async () => await _sut.Add(dto);

            _context.Customers.Should().HaveCount(1);
            await expected.Should()
                .ThrowExactlyAsync<NationalCodeExistException>();
        }

        [Fact]
        public async Task Active_activate_the_customer_account_properly()
        {
            Customer customer = new Customer()
            {
                FirstName = "dummyFirstName",
                LastName = "dummyLastName",
                NationalCode = "1234567891",
                PhoneNumber = "0917123456",
                IsActive = false,
                Score = 0,
            };
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            await _sut.Activate(customer.Id);

            Customer expected = await _context.Customers.SingleAsync();
            expected.Id.Should().Be(customer.Id);
            expected.IsActive.Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        public async Task ActivateFails_when_CustomerNotFoundException(
            int dummyId)
        {
            Func<Task> expected = async () => await _sut.Activate(dummyId);

            await expected.Should()
                .ThrowExactlyAsync<CustomerNotFoundException>();
        }

        [Fact]
        public async Task ActivateFails_when_NationalCodeLengthIsNotValidException()
        {
            Customer customer = new Customer()
            {
                FirstName = "dummyFirstName",
                LastName = "dummyLastName",
                NationalCode = "12345678912",
                PhoneNumber = "0917123456",
                IsActive = false,
                Score = 0,
            };
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            Func<Task> expected = async () => await _sut.Activate(customer.Id);

            await expected.Should()
               .ThrowExactlyAsync<NationalCodeLengthIsNotValidException>();
        }

        [Fact]
        public async Task ActivateFails_when_PhoneNumberLenghtIsNotValidException()
        {
            Customer customer = new Customer()
            {
                FirstName = "dummyFirstName",
                LastName = "dummyLastName",
                NationalCode = "1234567891",
                PhoneNumber = "091712345678",
                IsActive = false,
                Score = 0,
            };
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            Func<Task> expected = async () => await _sut.Activate(customer.Id);

            await expected.Should()
               .ThrowExactlyAsync<PhoneNumberLenghtIsNotValidException>();
        }

        [Fact]
        public async Task Edit_edits_the_property_of_a_customer_properly()
        {
            Customer customer = await CreateCustomerWithdummyValuesOnDb();
            EditCustomerDto dto = CreateEditCustomerDto();
            await _sut.Edit(dto, customer.Id);

            Customer expected = await _context.Customers.SingleAsync();
            expected.FirstName.Should().Be(dto.FirstName);
            expected.LastName.Should().Be(dto.LastName);
            expected.PhoneNumber.Should().Be(dto.PhoneNumber);
            expected.NationalCode.Should().Be(dto.NationalCode);
            expected.Email.Should().Be(dto.Email);
        }

        [Fact]
        public async Task EditFails_when_ActiveCustomerCannotChangeThierNationalCodeException()
        {
            Customer customer = new Customer()
            {
                FirstName = "dummyFirstName",
                LastName = "dummyLastName",
                NationalCode = "1234567891",
                PhoneNumber = "0917123456",
                IsActive = true,
                Score = 0,
            };
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            EditCustomerDto dto = CreateEditCustomerDto();
            Func<Task> expected = async () => await _sut.Edit(dto, customer.Id);

            await expected.Should()
                .ThrowExactlyAsync<ActiveCustomerCannotChangeThierNationalCodeException>();
        }

        [Fact]
        public async Task GetAll_returns_all_customers_properly()
        {
            Customer firstCustomer = await CreateCustomerWithdummyValuesOnDb();
            Customer SecondCustomer = await CreateCustomerWithdummyValuesOnDb();

            await _sut.GetAll();

            _context.Customers.Should().HaveCount(2);
        }

        [Fact]
        public async Task AddFinancialInformation_updates_financial_information_of_an_customer_properly()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
                new AddFinancialInformationDto
                {
                    CustomerId = customer.Id,
                    MonthlyIncome = Generator.GenerateDecimalNumber(),
                    Job = JobType.WithoutJob,
                };
            await _sut.AddFinancialInformation(dto);

            FinancialInformation expected =
                await _context.FinancialInformations.SingleAsync();
            expected.CustomerId.Should().Be(dto.CustomerId);
            expected.MonthlyIncome.Should().Be(dto.MonthlyIncome);
            expected.Job.Should().Be(dto.Job);
        }

        [Fact]
        public async Task AddFinancialInformation_with_government_job_updates_customer_score_to_20()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
               new AddFinancialInformationDto
               {
                   CustomerId = customer.Id,
                   MonthlyIncome = Generator.GenerateDecimalNumber(),
                   Job = JobType.GovernmentJob,
               };
            await _sut.AddFinancialInformation(dto);

            customer.Score.Should().Be(20);
        }

        [Fact]
        public async Task AddFinancialInformation_with_freelance_job_updates_customer_score_to_10()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
               new AddFinancialInformationDto
               {
                   CustomerId = customer.Id,
                   MonthlyIncome = Generator.GenerateDecimalNumber(),
                   Job = JobType.FreelanceJob,
               };
            await _sut.AddFinancialInformation(dto);

            customer.Score.Should().Be(10);
        }

        [Fact]
        public async Task AddFinancialInformation_with_monthly_income_greater_than_10m_updates_customer_score_to_20()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
               new AddFinancialInformationDto
               {
                   CustomerId = customer.Id,
                   MonthlyIncome = 11,
                   Job = JobType.WithoutJob,
               };
            await _sut.AddFinancialInformation(dto);

            customer.Score.Should().Be(20);
        }

        [Fact]
        public async Task AddFinancialInformation_with_monthly_income_between_10m_and_5_customer_score_to_10()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
               new AddFinancialInformationDto
               {
                   CustomerId = customer.Id,
                   MonthlyIncome = 10,
                   Job = JobType.WithoutJob,
               };
            await _sut.AddFinancialInformation(dto);

            customer.Score.Should().Be(10);
        }

        [Fact]
        public async Task AddFinancialInformationFails_When_FinancialInformationIsAlreadyExistException()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            FinancialInformation financialInformation =
                new FinancialInformation
                {
                    CustomerId = customer.Id,
                    MonthlyIncome = 10,
                    Job = JobType.WithoutJob,
                };
            await _context.FinancialInformations
                .AddAsync(financialInformation);
            await _unitOfWork.CommitAsync();
            mockFinancialInfoRepo.Setup(repo => repo.IsExistById(
                financialInformation.CustomerId)).ReturnsAsync(true);
                

            AddFinancialInformationDto dto =
             new AddFinancialInformationDto
             {
                 CustomerId = customer.Id,
                 MonthlyIncome = Generator.GenerateDecimalNumber(),
                 Job = JobType.WithoutJob,
             };
            Func<Task> expected = async () => 
            await _sut.AddFinancialInformation(dto);

            await expected.Should()
                .ThrowExactlyAsync<
                    FinancialInformationIsAlreadyExistException>();
        }

        [Fact]
        public async Task AddFinancialInformationFails_when_CustomerIsNotActiveException()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, false);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
            new AddFinancialInformationDto
            {
                CustomerId = customer.Id,
                MonthlyIncome = Generator.GenerateDecimalNumber(),
                Job = JobType.WithoutJob,
            };
            Func<Task> expected = async () =>
            await _sut.AddFinancialInformation(dto);

            await expected.Should()
                .ThrowExactlyAsync<
                    CustomerIsNotActiveException>();
        }

        private static AddCustomerDto CreateAddCustomerDtoWithDummyValues()
        {
            return new AddCustomerDto()
            {
                FirstName = "dummy-firstName",
                LastName = "dummy-lastName",
                PhoneNumber = "09178304319",
                NationalCode = "1234567891"
            };
        }

        private async Task<Customer> CreateCustomerWithdummyValuesOnDb()
        {
            Customer customer = new Customer()
            {
                FirstName = "dummyFirstName",
                LastName = "dummyLastName",
                PhoneNumber = "dummyPhoneNumber",
                NationalCode = "dummyNationalCode",
                IsActive = false,
                Score = 0,
            };
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            return customer;
        }

        private static EditCustomerDto CreateEditCustomerDto()
        {
            return new EditCustomerDto
            {
                FirstName = "dummy-f",
                LastName = "dummy-l",
                NationalCode = "dummy-n",
                PhoneNumber = "1234567891",
                Email = "dummy_e"
            };
        }
    }
}
