using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;

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

        public async Task Add(AddFinancialInformationDto dto)
        {
            Customer? customer = 
                await _customerRepository.FindById(dto.CustomerId);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            var financialInformation = new FinancialInformation
            {
                MonthlyIncome = dto.MonthlyIncome,
                Job = dto.Job,
                FinancialAssets = dto.FinancialAssets,
                CustomerId = dto.CustomerId,
            };

            await _repository.Add(financialInformation);
            await _unitOfWork.CommitAsync();
        }

        public async Task<GetFinancialInformationDto?> GetByCustomer(
            int customerId)
        {
            return await _repository.GetByCustomer(customerId);
        }
    }
}
