using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Customers;
using LoanManagement.Persistance.EF.FinancialInformations;
using LoanManagement.Persistance.EF.Loans;
using LoanManagement.Persistance.EF.LoanTypes;
using LoanManagement.Persistance.EF.Repayments;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Services.Loans;
using LoanManagement.Services.Loans.Contracts;
using LoanManagement.Services.Loans.Contracts.DTOs;
using LoanManagement.Services.LoanTypes.Contracts;
using LoanManagement.Services.LoanTypes.Exceptions;
using LoanManagement.Services.Repayments.Contracts;
using LoanManagement.Tests.Tools;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Services.Tests.Unit.Loans
{
    public class LoanServiceTests
    {
        private readonly LoanService _sut;
        private readonly LoanRepository _repository;
        private readonly EFDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly CustomerRepository _customerRepository;
        private readonly LoanTypeRepository _loanTypeRepository;
        private readonly FinancialInformationRepository 
            _finacialInformationRepository;

        public LoanServiceTests()
        {
            _context = new InMemoryDataBase().CreateDataContext<EFDbContext>();
            _unitOfWork = new EFUnitOfWork(_context);
            _repository = new EFLoanRepository(_context);
            _customerRepository = new EFCustomerRepository(_context);
            _loanTypeRepository = new EFLoanTypeRepository(_context);
            _finacialInformationRepository = 
                new EFFianancialInformationRepository(_context);
            _sut = new LoanAppService(
                _repository, _unitOfWork,
                _customerRepository, _loanTypeRepository, 
                _finacialInformationRepository);
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

        [Fact]
        public async Task GetAll_returns_all_loans_properly()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(
                Generator.GenerateByte());
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.UnderReview);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            List<GetAllLoanDto?> dtos = await _sut.GetAll();

            var expected = dtos.Single();
            expected?.CustomerId.Should().Be(customer.Id);
            expected?.LoanTypeId.Should().Be(loanType.Id);
            expected?.State.Should().Be(loan.State.ToString());
        }

        [Fact]
        public async Task LoanCheck_approve_loan_if_score_is_obove_60()
        {
            Customer customer = CustomerFactory.CreateCustomer(60, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(
                Generator.GenerateByte());
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.UnderReview);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            string state = await _sut.LoanCheck(loan.Id);

            state.Should().Be(LoanState.Approved.ToString());
        }

        [Fact]
        public async Task LoanCheck_reject_loan_if_score_is_less_than_60()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(
                Generator.GenerateByte());
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.UnderReview);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            string state = await _sut.LoanCheck(loan.Id);

            state.Should().Be(LoanState.Rejected.ToString());
        }

        [Fact]
        public async Task LoanCheck_up_score_to_20_if_loan_to_asset_ratio_less_than_50_percent_of_asset()
        {
            Customer customer = CustomerFactory.CreateCustomer(40, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateWithAmount(10);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            FinancialInformation financialInformation =
                FinancialInformationFactory.CreateWithAsset(
                    customer.Id, 100);
            await _context.FinancialInformations.
                AddAsync(financialInformation);
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.UnderReview);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            string state = await _sut.LoanCheck(loan.Id);

            customer.Score.Should().Be(60);
            state.Should().Be(LoanState.Approved.ToString());
        }

        [Fact]
        public async Task LoanCheck_up_score_to_10_if_loan_to_asset_ratio_less_than_70_percent_of_asset()
        {
            Customer customer = CustomerFactory.CreateCustomer(50, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateWithAmount(60);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            FinancialInformation financialInformation =
                FinancialInformationFactory.CreateWithAsset(
                    customer.Id, 100);
            await _context.FinancialInformations.
                AddAsync(financialInformation);
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.UnderReview);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            string state = await _sut.LoanCheck(loan.Id);

            customer.Score.Should().Be(60);
            state.Should().Be(LoanState.Approved.ToString());
        }

        [Fact]
        public async Task LoanCheckout_not_change_score_if_loan_rejected()
        {
            Customer customer = CustomerFactory.CreateCustomer(10, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateWithAmount(60);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            FinancialInformation financialInformation =
                FinancialInformationFactory.CreateWithAsset(
                    customer.Id, 100);
            await _context.FinancialInformations.
                AddAsync(financialInformation);
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.UnderReview);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

           string state = await _sut.LoanCheck(loan.Id);

            customer.Score.Should().Be(10);
            state.Should().Be(LoanState.Rejected.ToString());
        }

        
    }
}
