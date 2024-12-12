using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.LoanTypes;
using LoanManagement.Services.LoanTypes;
using LoanManagement.Services.LoanTypes.Contracts;
using LoanManagement.Services.LoanTypes.Contracts.DTOs;
using LoanManagement.Services.LoanTypes.Exceptions;
using LoanManagement.Tests.Tools;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Services.Tests.Unit.LoanTypes
{
    public class LoanTypeServiceTests
    {
        private readonly LoanTypeService _sut;
        private readonly LoanTypeRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly EFDbContext _context;

        public LoanTypeServiceTests()
        {
            _context = new InMemoryDataBase().CreateDataContext<EFDbContext>();
            _unitOfWork = new EFUnitOfWork(_context);
            _repository = new EFLoanTypeRepository(_context);
            _sut = new LoanTypeAppService(_repository, _unitOfWork);
        }

        [Fact]
        public async Task Add_adds_loan_type_properly()
        {
            AddLoanTypeDto dto = CreateAddLoanTypeDto();
            await _sut.Add(dto);

            LoanType expected = await _context.LoanTypes.SingleAsync();
            expected.Name.Should().Be(dto.Name);
            expected.Amount.Should().Be(dto.Amount);
            expected.InterestRate.Should().Be(dto.InterestRate);
            expected.RepaymentPeriod.Should().Be(dto.RepaymentPeriod);
            expected.MonthlyRepayment.Should().Be(dto.MonthlyRepayment);
        }

        [Fact]
        public async Task AddFails_when_AmountAndInterestRateIsExistException()
        {
            LoanType loanType = LoanTypeFactory.CreateLoanType();
            await _context.LoanTypes.AddAsync(loanType);
            await _unitOfWork.CommitAsync();

            AddLoanTypeDto dto = new AddLoanTypeDto
            {
                Name = loanType.Name,
                Amount = loanType.Amount,
                InterestRate = loanType.InterestRate,
                RepaymentPeriod = loanType.RepaymentPeriod,
                MonthlyRepayment = loanType.RepaymentPeriod
            };
            Func<Task> expected = async () => await _sut.Add(dto);

            await expected.Should()
                .ThrowExactlyAsync<AmountAndInterestRateIsExistException>();
        }

        [Fact]
        public async Task GetAll_returns_all_LoanType_properly()
        {
            LoanType firstLoanType = LoanTypeFactory.CreateLoanType();
            await _context.LoanTypes.AddAsync(firstLoanType);
            await _unitOfWork.CommitAsync();
            LoanType secondLoanType = LoanTypeFactory.CreateLoanType();
            await _context.LoanTypes.AddAsync(secondLoanType);
            await _unitOfWork.CommitAsync();

            List<GetAllLoanTypeDto> expected = await _sut.GetAll();

            _context.LoanTypes.Should().HaveCount(2);
            expected.Should().HaveCount(2);
        }
        private static AddLoanTypeDto CreateAddLoanTypeDto()
        {
            return new AddLoanTypeDto
            {
                Name = Generator.GenerateString(),
                Amount = Generator.GenerateDecimal(),
                InterestRate = Generator.GenerateDecimal(),
                RepaymentPeriod = Generator.GenerateByte(),
                MonthlyRepayment = Generator.GenerateDecimal()
            };
        }
    }
}
