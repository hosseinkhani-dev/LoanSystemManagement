using Castle.Core.Resource;
using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Customers;
using LoanManagement.Persistance.EF.Loans;
using LoanManagement.Persistance.EF.LoanTypes;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.Loans;
using LoanManagement.Services.Loans.Contracts;
using LoanManagement.Services.Loans.Contracts.DTOs;
using LoanManagement.Services.LoanTypes.Contracts;
using LoanManagement.Services.LoanTypes.Exceptions;
using LoanManagement.Tests.Tools;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Services.Tests.Unit.Loans
{
    public class LoanServiceTests
    {
        private readonly LoanService _sut;
        private readonly LoanRepository _loanRepository;
        private readonly EFDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly CustomerRepository _customerRepository;
        private readonly LoanTypeRepository _loanTypeRepository;

        public LoanServiceTests()
        {
            _context = new InMemoryDataBase().CreateDataContext<EFDbContext>();
            _unitOfWork = new EFUnitOfWork(_context);
            _loanRepository = new EFLoanRepository(_context);
            _customerRepository = new EFCustomerRepository(_context);
            _loanTypeRepository = new EFLoanTypeRepository(_context);
            _sut = new LoanAppService(
                _loanRepository, _unitOfWork,
                _customerRepository, _loanTypeRepository);
        }

        [Fact]
        public async Task Add_adds_loan_with_uder_review_state()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(
                Generator.GenerateByte());
            await _context.AddAsync(loanType);
            await _context.SaveChangesAsync();

            AddLoanDto dto = new AddLoanDto
            {
                CustomerId = customer.Id,
                LoanTypeId = loanType.Id,
            };
            await _sut.Add(dto);

            Loan loan = await _context.Loans.SingleAsync();
            loan.State.Should().Be(LoanState.UnderReview);
            loan.LoanStartDate.Should().BeNull();
            loan.CustomerId.Should().Be(customer.Id);
            loan.LoanTypeId.Should().Be(loanType.Id);
        }

        [Theory]
        [InlineData(1)]
        public async Task AddFails_when_CustomerNotFoundException(int dummyId)
        {
            LoanType loanType = LoanTypeFactory.CreateLoanType(
                Generator.GenerateByte());
            await _context.AddAsync(loanType);
            await _context.SaveChangesAsync();

            AddLoanDto dto = new AddLoanDto
            {
                CustomerId = dummyId,
                LoanTypeId = loanType.Id,
            };
            Func<Task> expected = async () =>
            await _sut.Add(dto);

            await expected.Should().
                ThrowExactlyAsync<CustomerNotFoundException>();
        }

        [Theory]
        [InlineData(1)]
        public async Task AddFails_when_LoanTypeNotFoundException(int dummyId)
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.AddAsync(customer);
            await _context.SaveChangesAsync();

            AddLoanDto dto = new AddLoanDto
            {
                CustomerId = customer.Id,
                LoanTypeId = dummyId,
            };
            Func<Task> expected = async () =>
            await _sut.Add(dto);

            await expected.Should().
                ThrowExactlyAsync<LoanTypeNotFoundException>();
        }

    }
}
