using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;
using LoanManagement.Services.FinancialInformations.Exceptions;

namespace LoanManagement.Services.FinancialInformations
{
    public class FinancialInformationAppService : FinancialInformationService
    {
        private readonly FinancialInformationRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly CustomerRepository _customerRepository;

        public FinancialInformationAppService(
            FinancialInformationRepository repository,
            UnitOfWork unitOfWork,
            CustomerRepository customerRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
        }

        public async Task Add(
           AddFinancialInformationDto dto)
        {
            Customer? customer = await _customerRepository.
                FindById(dto.CustomerId);
            StopWhenCustomerNotFound(customer);

            StopIfCustomerIsNotActive(customer!.IsActive);

            await StopIfFinancialInformationExist(dto.CustomerId);

            FinancialInformation financialInformation =
                CreateFinancialInformation(dto);

            customer!.Score += CalculateCustomerScore(
                dto.Job, dto.MonthlyIncome);

            await _repository.Add(financialInformation);
            await _unitOfWork.CommitAsync();
        }

        public async Task Edit(EditFinancialInformationDto dto, int customerId)
        {
            Customer? customer = await _customerRepository.
                FindById(customerId);
            StopWhenCustomerNotFound(customer);

            FinancialInformation? financialInformation =
                await _repository.FindById(customerId);
            StopIdFinancialInformationNotFound(financialInformation);

            int score = MinusScorOnDifferentSituations(
                dto, customer!.Score, financialInformation);

            customer!.Score += CalculateCustomerScore(
               dto.Job, dto.MonthlyIncome);

            customer!.Score = score;
            financialInformation!.MonthlyIncome = dto.MonthlyIncome;
            financialInformation.Job = dto.Job;
            financialInformation.FinancialAssets =
                dto.FinancialAssets;

            await _unitOfWork.CommitAsync();
        }

        private static void StopIdFinancialInformationNotFound(FinancialInformation? financialInformation)
        {
            if (financialInformation == null)
            {
                throw new
                    ThereIsNoFinancialInformationForTheCustomerException();
            }
        }

        private static int MinusScorOnDifferentSituations(
            EditFinancialInformationDto dto,int score, FinancialInformation? financialInformation)
        {
            JobType job = financialInformation.Job;
            decimal monthlyIncome = financialInformation.MonthlyIncome;

            if (job == JobType.GovernmentJob &&
                dto.Job == JobType.FreelanceJob)
            {
                score -= 10;
            }

            if (job == JobType.GovernmentJob &&
                 dto.Job == JobType.WithoutJob)
            {
                score -= 20;
            }
            ///
            if (monthlyIncome > 4 &&
                monthlyIncome < 11 &&
                dto.MonthlyIncome < 5)
            {
                score -= 10;
            }
            if (monthlyIncome > 10 &&
                dto.MonthlyIncome > 4 &&
                dto.MonthlyIncome < 11)
            {
                score -= 10;
            }
            if (monthlyIncome > 10 &&
                dto.MonthlyIncome < 5)
            {
                score -= 20;
            }

            //
            if (score < 0)
            {
                score = 0;
            }

            return score;
        }

        private static void StopWhenCustomerNotFound(Customer? customer)
        {
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
        }

        private static void StopIfCustomerIsNotActive(bool isActive)
        {
            if (!isActive)
            {
                throw new CustomerIsNotActiveException();
            }
        }

        private async Task StopIfFinancialInformationExist(int id)
        {
            bool isExist =
                            await _repository.
                            IsExistById(id);
            if (isExist)
            {
                throw new FinancialInformationIsAlreadyExistException();
            }
        }

        private static FinancialInformation CreateFinancialInformation(
          AddFinancialInformationDto dto)
        {
            FinancialInformation financialInformation =
                new FinancialInformation
                {
                    CustomerId = dto.CustomerId,
                    MonthlyIncome = dto.MonthlyIncome,
                    Job = dto.Job,
                    FinancialAssets = dto.FinancialAssets,
                };
            return financialInformation;
        }

        private static int CalculateCustomerScore(
           JobType job, decimal monthlyIncome)
        {
            int score = 0;
            if (job == JobType.GovernmentJob)
            {
                score += 20;
            }
            if (job == JobType.FreelanceJob)
            {
                score += 10;
            }
            if (monthlyIncome > 10)
            {
                score += 20;
            }
            if (monthlyIncome < 11 && monthlyIncome > 6)
            {
                score += 10;
            }
            return score;
        }
    }
}
