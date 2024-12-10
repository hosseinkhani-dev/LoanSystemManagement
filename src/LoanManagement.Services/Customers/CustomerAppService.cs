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

        public async Task Activate(int id)
        {
            Customer? customer = await _repository.FindById(id);
            StopWhenCustomerNotFound(customer);

            StopWhenNationalCodeLenghtIsNotValid(customer);

            StopWhenPhoneNumberCodeIsNotValid(customer);

            customer!.IsActive = true;

            await _unitOfWork.CommitAsync();
        }

        private static void StopWhenPhoneNumberCodeIsNotValid(Customer? customer)
        {
            if (customer.PhoneNumber.Length != 10)
            {
                throw new PhoneNumberLenghtIsNotValidException();
            }
        }

        private static void StopWhenNationalCodeLenghtIsNotValid(Customer? customer)
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
    }
}
