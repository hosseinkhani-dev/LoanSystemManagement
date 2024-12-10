using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Customers;
using LoanManagement.Services.Customers;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Contracts.DTOs;
using LoanManagement.Services.Customers.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Services.Tests.Unit.Customers
{
    public class CustomerServiceTests
    {
        private readonly CustomerService _sut;
        private readonly CustomerRepository _repository;
        private readonly EFDbContext _context;
        private readonly UnitOfWork _unitOfWork;

        public CustomerServiceTests()
        {
            _context = new InMemoryDataBase().CreateDataContext<EFDbContext>();
            _repository = new EFCustomerRepository(_context);
            _unitOfWork = new EFUnitOfWork(_context);
            _sut = new CustomerAppService(_repository, _unitOfWork);
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
    }
}
