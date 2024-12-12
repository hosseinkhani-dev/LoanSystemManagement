using FluentAssertions;
using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Customers;
using LoanManagement.Persistance.EF.FinancialInformations;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.FinancialInformations;
using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;
using LoanManagement.Services.FinancialInformations.Exceptions;
using LoanManagement.Tests.Tools;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Services.Tests.Unit.FinancialInformations
{
    public class FinancialInformationUnitTests
    {
        private readonly FinancialInformationService _sut;
        private readonly FinancialInformationRepository _repository;
        private readonly EFDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly CustomerRepository _customerRepository;

        public FinancialInformationUnitTests()
        {
            _context = new InMemoryDataBase().CreateDataContext<EFDbContext>();
            _unitOfWork = new EFUnitOfWork(_context);
            _customerRepository = new EFCustomerRepository(_context);
            _repository = new EFFianancialInformationRepository(_context);
            _sut = new FinancialInformationAppService(
                _repository,
                _unitOfWork,
                _customerRepository);
        }

        [Fact]
        public async Task Add_adds_financial_information_of_an_customer_properly()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
                  FinancialInformationFactory.CreateAddDto(
                      customer.Id,
                      Generator.GenerateDecimalNumber(),
                      JobType.FreelanceJob);
            await _sut.Add(dto);

            FinancialInformation expected =
                await _context.FinancialInformations.SingleAsync();
            expected.CustomerId.Should().Be(dto.CustomerId);
            expected.MonthlyIncome.Should().Be(dto.MonthlyIncome);
            expected.Job.Should().Be(dto.Job);
        }

        [Fact]
        public async Task Add_with_government_job_updates_customer_score_to_20()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
                  FinancialInformationFactory.CreateAddDto(
                      customer.Id,
                      Generator.GenerateDecimalNumber(),
                      JobType.GovernmentJob);
            await _sut.Add(dto);

            customer.Score.Should().Be(20);
        }

        [Fact]
        public async Task Add_with_freelance_job_updates_customer_score_to_10()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
                  FinancialInformationFactory.CreateAddDto(
                      customer.Id,
                      Generator.GenerateDecimalNumber(),
                      JobType.FreelanceJob);
            await _sut.Add(dto);

            customer.Score.Should().Be(10);
        }

        [Fact]
        public async Task Add_with_monthly_income_greater_than_10m_updates_customer_score_to_20()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
               FinancialInformationFactory.
               CreateAddDto(customer.Id, 11, JobType.WithoutJob);
            await _sut.Add(dto);

            customer.Score.Should().Be(20);
        }

        [Fact]
        public async Task Add_with_monthly_income_between_10m_and_5_customer_score_to_10()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
                FinancialInformationFactory.
                CreateAddDto(customer.Id, 10, JobType.WithoutJob);
            await _sut.Add(dto);

            customer.Score.Should().Be(10);
        }

        [Fact]
        public async Task AddFails_When_FinancialInformationIsAlreadyExistException()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            FinancialInformation financialInformation =
                FinancialInformationFactory.CreateFinancialInformation(
                   customer.Id, 10, JobType.WithoutJob);
            await _context.FinancialInformations
                .AddAsync(financialInformation);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
                FinancialInformationFactory.CreateAddDto(
                    customer.Id,
                    Generator.GenerateDecimalNumber(),
                    JobType.WithoutJob);
            Func<Task> expected = async () =>
            await _sut.Add(dto);

            await expected.Should()
                .ThrowExactlyAsync<
                    FinancialInformationIsAlreadyExistException>();
        }

        [Fact]
        public async Task AddFails_when_CustomerIsNotActiveException()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, false);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            AddFinancialInformationDto dto =
                 FinancialInformationFactory.CreateAddDto(
                     customer.Id,
                     Generator.GenerateDecimalNumber(),
                     JobType.WithoutJob);
            Func<Task> expected = async () =>
            await _sut.Add(dto);

            await expected.Should()
                .ThrowExactlyAsync<
                    CustomerIsNotActiveException>();
        }

        [Fact]
        public async Task Edit_changes_financial_information_properly()
        {
            Customer customer = CustomerFactory.CreateCustomer(0, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            FinancialInformation financialInformation =
                 FinancialInformationFactory.CreateFinancialInformation(
                    customer.Id, 0, JobType.WithoutJob);
            await _context.FinancialInformations
                .AddAsync(financialInformation);
            await _unitOfWork.CommitAsync();

            EditFinancialInformationDto dto = new EditFinancialInformationDto
            {
                MonthlyIncome = 3,
                Job = JobType.GovernmentJob,
                FinancialAssets = 22,
            };
            await _sut.Edit(
                dto, financialInformation.CustomerId);

            var expected = await _context.FinancialInformations.SingleAsync();
            expected.MonthlyIncome.Should().Be(dto.MonthlyIncome);
            expected.Job.Should().Be(dto.Job);
            expected.FinancialAssets.Should().Be(dto.FinancialAssets);
        }

        [Fact]
        public async Task Edit_Job_from_governemrnt_to_freelance_changes_score_to_minus_10()
        {
            Customer customer = CustomerFactory.CreateCustomer(20, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            FinancialInformation financialInformation =
                 FinancialInformationFactory.CreateFinancialInformation(
                    customer.Id, 0, JobType.GovernmentJob);
            await _context.FinancialInformations
                .AddAsync(financialInformation);
            await _unitOfWork.CommitAsync();

            EditFinancialInformationDto dto = new EditFinancialInformationDto
            {
                MonthlyIncome = financialInformation.MonthlyIncome,
                Job = JobType.FreelanceJob,
                FinancialAssets = financialInformation.FinancialAssets,
            };
            await _sut.Edit(
                dto, financialInformation.CustomerId);

            var expected = await _context.FinancialInformations.SingleAsync();
            expected.Job.Should().Be(dto.Job);
            expected.Customer.Score.Should().Be(10);
        }

        [Fact]
        public async Task Edit_Job_from_governemrnt_to_wtihoutjob_changes_score_to_minus_20()
        {
            Customer customer = CustomerFactory.CreateCustomer(20, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            FinancialInformation financialInformation =
                 FinancialInformationFactory.CreateFinancialInformation(
                    customer.Id, 0, JobType.GovernmentJob);
            await _context.FinancialInformations
                .AddAsync(financialInformation);
            await _unitOfWork.CommitAsync();

            EditFinancialInformationDto dto = new EditFinancialInformationDto
            {
                MonthlyIncome = financialInformation.MonthlyIncome,
                Job = JobType.WithoutJob,
                FinancialAssets = financialInformation.FinancialAssets,
            };
            await _sut.Edit(
                dto, financialInformation.CustomerId);

            var expected = await _context.FinancialInformations.SingleAsync();
            expected.Job.Should().Be(dto.Job);
            expected.Customer.Score.Should().Be(0);
        }

        [Fact]
        public async Task Edit_monthlyincome_from_between_5_10_to_0_5_changes_score_to_minus_10()
        {
            Customer customer = CustomerFactory.CreateCustomer(10, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            FinancialInformation financialInformation =
                 FinancialInformationFactory.CreateFinancialInformation(
                    customer.Id, 6, JobType.WithoutJob);
            await _context.FinancialInformations
                .AddAsync(financialInformation);
            await _unitOfWork.CommitAsync();

            EditFinancialInformationDto dto = new EditFinancialInformationDto
            {
                MonthlyIncome = 4,
                Job = JobType.WithoutJob,
                FinancialAssets = financialInformation.FinancialAssets,
            };
            await _sut.Edit(
                dto, financialInformation.CustomerId);

            var expected = await _context.FinancialInformations.SingleAsync();
            expected.Job.Should().Be(dto.Job);
            expected.Customer.Score.Should().Be(0);
        }

        [Fact]
        public async Task Edit_monthlyincome_from_greater_than_10_to_5_10_changes_score_to_minus_10()
        {
            Customer customer = CustomerFactory.CreateCustomer(20, true);
            await _context.Customers.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            FinancialInformation financialInformation =
                 FinancialInformationFactory.CreateFinancialInformation(
                    customer.Id, 11, JobType.WithoutJob);
            await _context.FinancialInformations
                .AddAsync(financialInformation);
            await _unitOfWork.CommitAsync();

            EditFinancialInformationDto dto = new EditFinancialInformationDto
            {
                MonthlyIncome = 5,
                Job = JobType.WithoutJob,
                FinancialAssets = financialInformation.FinancialAssets,
            };
            await _sut.Edit(
                dto, financialInformation.CustomerId);

            var expected = await _context.FinancialInformations.SingleAsync();
            expected.Job.Should().Be(dto.Job);
            expected.Customer.Score.Should().Be(10);
        }
    }
}
