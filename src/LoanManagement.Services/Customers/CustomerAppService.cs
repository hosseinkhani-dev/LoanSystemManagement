using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Contracts.DTOs;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;
using LoanManagement.Services.FinancialInformations.Exceptions;

namespace LoanManagement.Services.Customers
{
    public class CustomerAppService : CustomerService
    {
        private readonly CustomerRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly FinancialInformationRepository 
            _financialInformationRepository;

        public CustomerAppService(
            CustomerRepository repository,
            UnitOfWork unitOfWork,
            FinancialInformationRepository financialInformationRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _financialInformationRepository = financialInformationRepository;
        }

        public async Task Add(AddCustomerDto dto)
        {
            await StopIfPhoneNumberExist(dto.PhoneNumber);

            await StopIfNationalCodeExist(dto.NationalCode);

            Customer customer = CreateCustomer(dto);

            await _repository.Add(customer);
            await _unitOfWork.CommitAsync();
        }

        public async Task Activate(int id)
        {
            Customer? customer = await _repository.FindById(id);
            StopWhenCustomerNotFound(customer);

            StopWhenNationalCodeLenghtIsNotValid(customer);

            StopWhenPhoneNumberIsNotValid(customer!.PhoneNumber);

            customer!.IsActive = true;

            await _unitOfWork.CommitAsync();
        }

        public async Task Edit(EditCustomerDto dto, int id)
        {
            Customer? customer = await _repository.FindById(id);
            StopWhenCustomerNotFound(customer);

            StopIfAnActiveCustomerWantsToChangeNationalCode(dto, customer);

            StopWhenPhoneNumberIsNotValid(dto.PhoneNumber);

            EditCustomer(dto, customer);

            await _unitOfWork.CommitAsync();
        }

        public async Task<List<GetAllCustomerDto>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task AddFinancialInformation(
            AddFinancialInformationDto dto)
        {
            Customer? customer = await _repository.FindById(dto.CustomerId);
            StopWhenCustomerNotFound(customer);

            StopIfCustomerIsNotActive(customer!.IsActive);

            await StopIfFinancialInformationExist(customer!.Id);

            CreateFinancialInformation(dto, customer!);

            customer!.Score += CalculateCustomerScore(
                dto.Job, dto.MonthlyIncome);

            await _unitOfWork.CommitAsync();
        }

        private static void StopIfCustomerIsNotActive(bool isActive)
        {
            if (!isActive)
            {
                throw new CustomerIsNotActiveException();
            }
        }

        private static Customer CreateCustomer(AddCustomerDto dto)
        {
            return new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                NationalCode = dto.NationalCode,
                IsActive = false,
                Score = 0,
                Email = dto.Email,
            };
        }

        private static void CreateFinancialInformation(
            AddFinancialInformationDto dto, Customer customer)
        {
            customer!.FinancialInformation = new FinancialInformation
            {
                CustomerId = dto.CustomerId,
                MonthlyIncome = dto.MonthlyIncome,
                Job = dto.Job,
                FinancialAssets = dto.FinancialAssets,
            };
        }

        private static void EditCustomer(
            EditCustomerDto dto, Customer? customer)
        {
            customer!.FirstName = dto.FirstName;
            customer.LastName = dto.LastName;
            customer.PhoneNumber = dto.PhoneNumber;
            customer.NationalCode = dto.NationalCode;
            customer.Email = dto.Email;
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

        private static void StopIfAnActiveCustomerWantsToChangeNationalCode(
            EditCustomerDto dto, Customer? customer)
        {
            if (customer!.IsActive == true &&
                dto.NationalCode != customer.NationalCode)
            {
                throw new
                    ActiveCustomerCannotChangeThierNationalCodeException();
            }
        }

        private static void StopWhenPhoneNumberIsNotValid(string phoneNumber)
        {
            if (phoneNumber.Length != 10)
            {
                throw new PhoneNumberLenghtIsNotValidException();
            }
        }

        private static void StopWhenNationalCodeLenghtIsNotValid(
            Customer? customer)
        {
            if (customer.NationalCode.Length != 10)
            {
                throw new NationalCodeLengthIsNotValidException();
            }
        }

        private static void StopWhenCustomerNotFound(Customer? customer)
        {
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
        }

        private async Task StopIfPhoneNumberExist(string phoneNumber)
        {
            bool isPhoneNumberExist = await _repository
                            .IsPhoneNumberExist(phoneNumber);
            if (isPhoneNumberExist)
            {
                throw new PhoneNumberExistException();
            }
        }

        private async Task StopIfNationalCodeExist(string nationalCode)
        {
            bool isNationalCodeExist = await _repository
                .IsNationalCodeExist(nationalCode);
            if (isNationalCodeExist)
            {
                throw new NationalCodeExistException();
            }
        }

        private async Task StopIfFinancialInformationExist(int id)
        {
            bool isExist =
                            await _financialInformationRepository.
                            IsExistById(id);
            if (isExist)
            {
                throw new FinancialInformationIsAlreadyExistException();
            }
        }
    }
}
