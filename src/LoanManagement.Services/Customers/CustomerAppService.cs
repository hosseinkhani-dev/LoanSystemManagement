using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Contracts.DTOs;
using LoanManagement.Services.Customers.Exceptions;

namespace LoanManagement.Services.Customers
{
    public class CustomerAppService : CustomerService
    {
        private readonly CustomerRepository _repository;
        private readonly UnitOfWork _unitOfWork;

        public CustomerAppService(CustomerRepository repository,
            UnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Add(AddCustomerDto dto)
        {
            bool isPhoneNumberExist = await _repository
                .IsPhoneNumberExist(dto.PhoneNumber);
            if (isPhoneNumberExist)
            {
                throw new PhoneNumberExistException();
            }

            bool isNationalCodeExist = await _repository
                .IsNationalCodeExist(dto.NationalCode);
            if (isNationalCodeExist)
            {
                throw new NationalCodeExistException();
            }

            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                NationalCode = dto.NationalCode,
                IsActive = false,
                Score = 0,
                Email = dto.Email,
            };

            await _repository.Add(customer);
            await _unitOfWork.CommitAsync();
        }
    }
}
