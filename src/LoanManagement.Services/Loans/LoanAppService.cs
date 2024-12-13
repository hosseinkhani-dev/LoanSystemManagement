using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.Loans.Contracts;
using LoanManagement.Services.Loans.Contracts.DTOs;
using LoanManagement.Services.LoanTypes.Contracts;
using LoanManagement.Services.LoanTypes.Exceptions;

namespace LoanManagement.Services.Loans
{
    public class LoanAppService : LoanService
    {
        private readonly LoanRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly CustomerRepository _customerRepository;
        private readonly LoanTypeRepository _loanTypeRepository;

        public LoanAppService(
            LoanRepository repository,
            UnitOfWork unitOfWork,
            CustomerRepository customerRepository,
            LoanTypeRepository loanTypeRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _loanTypeRepository = loanTypeRepository;
        }

        public async Task Add(AddLoanDto dto)
        {
            Customer? customer = await _customerRepository.
                FindById(dto.CustomerId);
            StopIfCustomerNotFound(customer);

            LoanType? loanType = await _loanTypeRepository.
                FindById(dto.LoanTypeId);
            StopIfLoanTypeNotFound(loanType);

            Loan loan = new Loan
            {
                CustomerId = dto.CustomerId,
                LoanTypeId = dto.LoanTypeId,
                State = LoanState.UnderReview,
                LoanStartDate = null
            };

            await _repository.Add(loan);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<GetAllLoanDto?>> GetAll()
        {
            return await _repository.GetAll();
        }

        private static void StopIfLoanTypeNotFound(LoanType? loanType)
        {
            if (loanType == null)
            {
                throw new LoanTypeNotFoundException();
            }
        }

        private static void StopIfCustomerNotFound(Customer? customer)
        {
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
        }
    }
}
