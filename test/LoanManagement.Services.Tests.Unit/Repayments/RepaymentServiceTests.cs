using Castle.Core.Resource;
using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Loans;
using LoanManagement.Persistance.EF.LoanTypes;
using LoanManagement.Persistance.EF.Repayments;
using LoanManagement.Services.Loans.Contracts;
using LoanManagement.Services.Loans.Exceptions;
using LoanManagement.Services.LoanTypes.Contracts;
using LoanManagement.Services.Repayments;
using LoanManagement.Services.Repayments.Contracts;
using LoanManagement.Services.Repayments.Contracts.DTOs;
using LoanManagement.Tests.Tools;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Services.Tests.Unit.Repayments
{
    public class RepaymentServiceTests
    {
        private readonly RepaymentService _sut;
        private readonly RepaymentRepository _repository;
        private readonly EFDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly LoanRepository _loanRepository;
        private readonly LoanTypeRepository _loanTypeRepository;

        public RepaymentServiceTests()
        {
            _context = new InMemoryDataBase().CreateDataContext<EFDbContext>();
            _unitOfWork = new EFUnitOfWork(_context);
            _repository = new EFRepaymentRepository(_context);
            _loanRepository = new EFLoanRepository(_context);
            _loanTypeRepository = new EFLoanTypeRepository(_context);
            _sut = new RepaymentAppService(
                _repository,
                _unitOfWork,
                _loanRepository,
                _loanTypeRepository);
        }

        [Fact]
        public async Task GenerateRepayments_creates_repayments_for_loan()
        {
            Customer customer = CustomerFactory.CreateCustomer(60, true);
            await _context.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(6);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.Approved);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            await _sut.GenerateRepayments(loan.Id);

            List<Repayment> repayments = await _context.Repayments
                .Where(r => r.LoanId == loan.Id).ToListAsync();
            repayments.Should().HaveCount(6);
            repayments.Should().AllSatisfy(r =>
            {
                r.TotalRepaid.Should().Be(0);
                r.IsRepaid.Should().BeFalse();
                r.IsFullyRepaid.Should().BeFalse();
            });
        }

        [Fact]
        public async Task GenerateRepaymentsFails_When_RepaymentsAlreadyExistForThisLoanException()
        {
            Customer customer = CustomerFactory.CreateCustomer(60, true);
            await _context.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(6);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.Approved);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            Repayment repayment = RepaymentFactory.GenerateRepayment(loan.Id);
            await _context.Repayments.AddAsync(repayment);
            await _context.SaveChangesAsync();

            Func<Task> expected = async () => await _sut.
            GenerateRepayments(loan.Id);

            await expected.Should().
                ThrowExactlyAsync<
                    RepaymentsAlreadyExistForThisLoanException>();

        }

        [Fact]
        public async Task GetAllUnpaid_returns_all_repayment_of_a_loan()
        {
            Customer customer = CustomerFactory.CreateCustomer(60, true);
            await _context.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(6);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.Approved);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();
            Repayment firstRepayment = RepaymentFactory.
                GenerateRepayment(loan.Id);
            await _context.Repayments.AddAsync(firstRepayment);
            Repayment secondRepayment = RepaymentFactory.
                GenerateRepayment(loan.Id);
            await _context.Repayments.AddAsync(secondRepayment);
            await _context.SaveChangesAsync();

            List<GetAllUnpaidRepaymentsDto?> dto = await _sut.GetAllUnpaid(1);

            dto.Should().HaveCount(2);
        }

        [Fact]
        public async Task PayRepayment_changed_to_paid_when_payment_is_onTime()
        {
            Customer customer = CustomerFactory.CreateCustomer(60, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(6);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.Approved);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();
            Repayment repayment = new RepaymentBuilder(loan.Id)
                .Amount(loanType.MonthlyRepayment)
                .TotalRepaid(0)
                .DueDate(DateTime.Now.AddDays(1))
                .RepaymentCount(0)
                .TotalLatePenalty(0)
                .LatePenaltyCount(0)
                .PaymentDate(DateTime.Now)
                .IsFullyRepaid(false)
                .IsRepaid(false)
                .Build();
            await _context.Repayments.AddAsync(repayment);
            await _context.SaveChangesAsync();

            await _sut.PayRepayment(repayment.Id);

            var expected = await _context.Repayments.SingleAsync();
            expected.IsRepaid.Should().BeTrue();
            expected.TotalRepaid.Should().Be(repayment.TotalRepaid);
            expected.PaymentDate.Should().Be(repayment.PaymentDate);
            expected.RepaymentCount.Should().Be(1);
            expected.TotalLatePenalty.Should().Be(0);
            expected.LatePenaltyCount.Should().Be(0);
            expected.IsFullyRepaid.Should().BeFalse();
        }

        [Fact]
        public async Task PayRepayment_applies_penalty_when_payment_is_late()
        {
            Customer customer = CustomerFactory.CreateCustomer(60, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(6);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.Approved);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();
            Repayment repayment = new RepaymentBuilder(loan.Id)
                .Amount(loanType.MonthlyRepayment)
                .TotalRepaid(0)
                .DueDate(DateTime.Now.AddDays(-1))
                .RepaymentCount(0)
                .TotalLatePenalty(0)
                .LatePenaltyCount(0)
                .IsFullyRepaid(false)
                .IsRepaid(false)
                .Build();
            await _context.Repayments.AddAsync(repayment);
            await _context.SaveChangesAsync();

            await _sut.PayRepayment(repayment.Id);

            var expectedRepayment = await _context.Repayments.SingleAsync();
            var expectedCustomer = await _context.Customers.SingleAsync();
            expectedRepayment.IsRepaid.Should().BeTrue();
            expectedRepayment.TotalRepaid.Should().Be(repayment.Amount);
            expectedRepayment.PaymentDate.Should()
                .BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
            expectedRepayment.RepaymentCount.Should().Be(1);
            expectedRepayment.TotalLatePenalty.Should()
                .Be(repayment.Amount * 0.02m);
            expectedRepayment.LatePenaltyCount.Should().Be(1);
            expectedCustomer.Score.Should().Be(55);
            expectedRepayment.IsFullyRepaid.Should().BeFalse();
        }

        [Fact]
        public async Task PayRepayment_changes_customer_score_when_payment_is_full_and_there_is_no_penalty()
        {
            // Arrange
            Customer customer = CustomerFactory.CreateCustomer(60, true);
            await _context.Customers.AddAsync(customer);
            LoanType loanType = LoanTypeFactory.CreateLoanType(1);
            await _context.LoanTypes.AddAsync(loanType);
            await _context.SaveChangesAsync();
            Loan loan = LoanFactory.CreateLoan(
                customer.Id, loanType.Id, LoanState.Approved);
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();
            Repayment repayment = new RepaymentBuilder(loan.Id)
                .Amount(loanType.MonthlyRepayment)
                .TotalRepaid(0)
                .DueDate(DateTime.Now.AddDays(1))
                .RepaymentCount(0)
                .TotalLatePenalty(0)
                .LatePenaltyCount(0)
                .IsFullyRepaid(false)
                .IsRepaid(false)
                .Build();
            await _context.Repayments.AddAsync(repayment);
            await _context.SaveChangesAsync();

            await _sut.PayRepayment(repayment.Id);

            var expectedCustomer = await _context.Customers.SingleAsync();
            var expectedRepayment = await _context.Repayments.SingleAsync();
            expectedRepayment.IsFullyRepaid.Should().BeTrue();
            expectedRepayment.IsRepaid.Should().BeTrue();
            expectedRepayment.RepaymentCount.Should().Be(1);
            expectedCustomer.Score.Should().Be(90);
        }
    }
}
