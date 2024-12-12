using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.LoanTypes.Contracts;
using LoanManagement.Services.LoanTypes.Contracts.DTOs;
using LoanManagement.Services.LoanTypes.Exceptions;

namespace LoanManagement.Services.LoanTypes
{
    public class LoanTypeAppService : LoanTypeService
    {
        private readonly LoanTypeRepository _repository;
        private readonly UnitOfWork _unitOfWork;

        public LoanTypeAppService(
            LoanTypeRepository repository, UnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Add(AddLoanTypeDto dto)
        {
            bool isLoanExist = await _repository.
                IsExist(dto.Amount, dto.InterestRate);
            if (isLoanExist) 
            {
                throw new AmountAndInterestRateIsExistException();
            }

            LoanType loanType = new LoanType
            {
                Name = dto.Name,
                Amount = dto.Amount,
                InterestRate = dto.InterestRate,
                RepaymentPeriod = dto.RepaymentPeriod,
                MonthlyRepayment = dto.MonthlyRepayment,
            };

            await _repository.Add(loanType);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<GetAllLoanTypeDto>> GetAll()
        {
            return await _repository.GetAll();
        }
    }
}
