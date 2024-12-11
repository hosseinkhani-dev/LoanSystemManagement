using Bogus;
using Castle.Core.Resource;
using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.FinancialInformations;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.FinancialInformations;
using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;
using LoanManagement.Tests.Tools;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace LoanManagement.Services.Tests.Unit.FinancialInformations
{
    public class FinancialInformationServiceTests
    {
        private readonly FinancialInformationService _sut;
        private readonly FinancialInformationRepository _repository;
        private readonly EFDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private Mock<CustomerRepository> mockCustomerRepository;

        public FinancialInformationServiceTests()
        {
            _context = new InMemoryDataBase().CreateDataContext<EFDbContext>();
            _repository = new EFFinancialInformationRepository(_context);
            _unitOfWork = new EFUnitOfWork(_context);
           mockCustomerRepository = 
                new Mock<CustomerRepository>();
            _sut = new FinancialInformationAppService(
                _repository, _unitOfWork, mockCustomerRepository.Object);
        }

        [Fact]
        public async Task Add_adds_financail_information_of_a_customer_properly()
        {
            Customer customer = CustomerFactory.CreateCustomer();
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            mockCustomerRepository.Setup(repo => repo.FindById(customer.Id))
               .ReturnsAsync(customer);

            AddFinancialInformationDto dto = FinancialInformationFactory.
                GenerateAddFinancialInformationDto(customer.Id);
            await _sut.Add(dto);

            Customer expectedCustomer = await _context.Customers.SingleAsync();
            expectedCustomer.Id.Should().Be(customer.Id);
            
            FinancialInformation expected = 
                await _context.FinancialInformations.SingleAsync();
            expected.MonthlyIncome.Should().Be(dto.MonthlyIncome);
            expected.Job.Should().Be(dto.Job);
            expected.CustomerId.Should().Be(dto.CustomerId);
            expected.FinancialAssets.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        public async Task AddFails_when_CustomerNotFoundException(int dummyId)
        {
             mockCustomerRepository.Setup(repo => repo.FindById(dummyId))
                .ReturnsAsync((Customer?)null);

            AddFinancialInformationDto dto = FinancialInformationFactory.
               GenerateAddFinancialInformationDto(dummyId);
            Func<Task> expected = async() => await _sut.Add(dto);

            await expected.Should()
                .ThrowExactlyAsync<CustomerNotFoundException>();
        }
    }
}
