using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Customers;
using LoanManagement.Persistance.EF.FinancialInformations;
using LoanManagement.Persistance.EF.Loans;
using LoanManagement.Persistance.EF.LoanTypes;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.FinancialInformations.Contracts;
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

        [Fact]
        public async Task GetActiveLoansReport_Shows_all_loan_reports()
        {
            Customer customer = CustomerFactory.CreateCustomer(10, true);
            await _context.Customers.AddAsync(customer);

            LoanType firstLoanType = new LoanTypeBuilder()
                .WithAmount(50)
                .WithRepaymentPeriod(12)
                .WithInterestRate(0.5m)
                .WithName("first")
                .WithMonthlyRepayment(1.2m)
                .Build();
            LoanType secondLoanType = new LoanTypeBuilder()
                .WithAmount(100)
                .WithRepaymentPeriod(6)
                .WithInterestRate(0.2m)
                .WithName("second")
                .WithMonthlyRepayment(2.8m)
                .Build();
            await _context.LoanTypes.AddRangeAsync(
                firstLoanType, secondLoanType);
            await _unitOfWork.CommitAsync();

            Loan firstLoan = LoanFactory.CreateLoan(
                    customer.Id, firstLoanType.Id, LoanState.Repaying);
            Loan secondLoan = LoanFactory.CreateLoan(
                   customer.Id, secondLoanType.Id, LoanState.DelayInRepayment);
            await _context.Loans.AddRangeAsync(
                firstLoan, secondLoan);
            await _unitOfWork.CommitAsync();

            Repayment firstRepayment = new RepaymentBuilder(firstLoan.Id)
                .Amount(firstLoanType.MonthlyRepayment)
                .TotalRepaid((firstLoanType.MonthlyRepayment) * 8)
                .DueDate(DateTime.Now.AddDays(1))
                .TotalLatePenalty(0)
                .LatePenaltyCount(0)
                .IsFullyRepaid(false)
                .IsRepaid(true)
                .Build();
            Repayment secondRepayment = new RepaymentBuilder(secondLoan.Id)
               .Amount(secondLoanType.MonthlyRepayment)
               .TotalRepaid((secondLoanType.MonthlyRepayment) * 4)
               .DueDate(DateTime.Now)
               .RepaymentCount(4)
               .TotalLatePenalty(2000)
               .LatePenaltyCount(2)
               .IsFullyRepaid(false)
               .IsRepaid(true)
               .Build();
            Repayment thirdRepayment = new RepaymentBuilder(secondLoan.Id)
               .Amount(secondLoanType.MonthlyRepayment)
               .DueDate(DateTime.Now.AddDays(1))
               .IsFullyRepaid(false)
               .IsRepaid(false)
               .Build();
            await _context.Repayments.AddRangeAsync(
                firstRepayment, secondRepayment, thirdRepayment);
            await _unitOfWork.CommitAsync();

            List<GetAllLoanActiveReportDto> result =
                await _sut.GetAllActiveLoansReport();

            result.Should().HaveCount(2);

            var firstLoanReport = result.Single(
                x => x.LoanId == firstLoan.Id);
            firstLoanReport.LoanState.Should()
                .Be(LoanState.Repaying.ToString());
            firstLoanReport!.TotalRepaid.Should()
                 .Be(firstRepayment.TotalRepaid);
            firstLoanReport.RemainingRepayments.Should().Be(0);

            var secondLoanReport = result.Single(
                x => x.LoanId == secondLoan.Id);
            secondLoanReport.LoanState.Should()
                .Be(LoanState.DelayInRepayment.ToString());
            secondLoanReport!.TotalRepaid.Should()
                .Be(secondRepayment.TotalRepaid);
            secondLoanReport.RemainingRepayments.Should().Be(1);
        }

        [Fact]
        public async Task GetTotalInterestAndPenalty_ReturnsCorrectValues()
        {
            Customer customer = CustomerFactory.CreateCustomer(10, true);
            await _context.Customers.AddAsync(customer);
            LoanType firstLoanType = new LoanTypeBuilder()
             .WithAmount(100)
             .WithRepaymentPeriod(6)
             .WithInterestRate(0.8m)
             .WithName("second")
             .WithMonthlyRepayment(2.8m)
             .Build();
            LoanType secondLoanType = new LoanTypeBuilder()
             .WithAmount(100)
             .WithRepaymentPeriod(6)
             .WithInterestRate(0.2m)
             .WithName("second")
             .WithMonthlyRepayment(2.8m)
             .Build();
            await _context.LoanTypes.AddRangeAsync(
                firstLoanType, secondLoanType);
            await _unitOfWork.CommitAsync();

            Loan firstLoan = LoanFactory.CreateLoan(
                    customer.Id, firstLoanType.Id, LoanState.DelayInRepayment);
            Loan secondLoan = LoanFactory.CreateLoan(
                  customer.Id, secondLoanType.Id, LoanState.DelayInRepayment);
            await _context.Loans.AddRangeAsync(firstLoan, secondLoan);
            await _unitOfWork.CommitAsync();

            Repayment firstRepayment = new RepaymentBuilder(firstLoan.Id)
                .TotalLatePenalty(40)
                .IsFullyRepaid(false)
                .IsRepaid(true)
                .Build();
            Repayment secondRepayment = new RepaymentBuilder(secondLoan.Id)
                .TotalLatePenalty(30)
                .IsFullyRepaid(false)
                .IsRepaid(true)
                .Build();
            await _context.Repayments.AddRangeAsync(
               firstRepayment, secondRepayment);
            await _unitOfWork.CommitAsync();

            GetTotalInterestAndPenaltyDto result = await _sut
                .GetTotalInterestAndPenalty();

            result.TotalLatePenalty.Should().Be(70m);
            result.TotalInterest.Should().Be(100m);
        }

        [Fact]
        public async Task GetAllClosedReport_returns_all_closed_loans()
        {
            Customer customer = CustomerFactory.CreateCustomer(10, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateWithAmount(60);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();

            Loan firstLoan = LoanFactory.CreateLoan(
                   customer.Id, loanType.Id, LoanState.Closed);
            Loan secondLoan = LoanFactory.CreateLoan(
                   customer.Id, loanType.Id, LoanState.Closed);
            await _context.Loans.AddRangeAsync(
                firstLoan, secondLoan);
            await _unitOfWork.CommitAsync();

            Repayment firstRepayment = new RepaymentBuilder(firstLoan.Id)
              .TotalLatePenalty(40)
              .IsFullyRepaid(true)
              .IsRepaid(true)
              .Build();
            Repayment secondRepayment = new RepaymentBuilder(secondLoan.Id)
                .TotalLatePenalty(30)
                .IsFullyRepaid(true)
                .IsRepaid(true)
                .Build();
            await _context.Repayments.AddRangeAsync(
               firstRepayment, secondRepayment);
            await _unitOfWork.CommitAsync();

            List<GetAllClosedLoanReportDto> expected =
                await _sut.GetAllClosedReport();

            expected.Should().HaveCount(2);
            var firstLoanReport = expected.Single(
               x => x.LoanId == firstLoan.Id);
            firstLoanReport.LoanId.Should()
                .Be(firstLoan.Id);
            firstLoanReport.Amount.Should().Be(loanType.Amount);
            firstLoanReport.RepaymentCount.Should()
                .Be(firstRepayment.RepaymentCount);
            firstLoanReport.TotalLatePenalty.Should()
                .Be(firstRepayment.TotalLatePenalty);

        }
    }
}

