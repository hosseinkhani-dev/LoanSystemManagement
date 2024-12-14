using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Services.Loans.Contracts;
using LoanManagement.Services.Loans.Contracts.DTOs;
using LoanManagement.Services.Loans.Exceptions;
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
        private readonly FinancialInformationRepository
            _financialInformationRepository;

        public LoanAppService(
            LoanRepository repository,
            UnitOfWork unitOfWork,
            CustomerRepository customerRepository,
            LoanTypeRepository loanTypeRepository,
            FinancialInformationRepository financialInformationRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _loanTypeRepository = loanTypeRepository;
            _financialInformationRepository = financialInformationRepository;
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

        public async Task<string> LoanCheck(int id)
        {
            Loan? loan = await _repository.FindById(id);
            StopIfLoanNotFound(loan);
            if (loan!.Customer == null)
            {
                throw new CustomerNotFoundException();
            }
            if (loan.State == LoanState.UnderReview ||
                loan.State == LoanState.Rejected)
            {
                FinancialInformation? financialInformation =
               await _financialInformationRepository.
               FindById(loan!.CustomerId);

                int scoreBeforeChange = loan.Customer.Score;
                int score = loan.Customer.Score;

                if (financialInformation?.FinancialAssets != null)
                {
                    score = CahngeScoreBasedOnLoanToAssetRatio(
                        loan.Customer.Score,
                        loan.LoanType.Amount,
                        financialInformation.FinancialAssets ?? 0);
                    loan.Customer.Score = score;
                }

                ApproveOrRejectBaseOnScore(loan, scoreBeforeChange, score);
            }

            await _unitOfWork.CommitAsync();
            return loan.State.ToString();
        }

        public async Task<List<GetAllLoanActiveReportDto>>
            GetAllActiveLoansReport()
        {
            return await _repository.GetAllActiveLoansReport();
        }

        public async Task<GetTotalInterestAndPenaltyDto> GetTotalInterestAndPenalty()
        {
            return await _repository.GetTotalInterestAndPenalty();
        }

        public async Task<List<GetAllClosedLoanReportDto>>
            GetAllClosedReport()
        {
            return await _repository.GetAllClosedReport();
        }

        private static void ApproveOrRejectBaseOnScore(
            Loan loan, int scoreBeforeChange, int score)
        {
            if (score > 59)
            {
                loan.State = LoanState.Approved;
            }
            else if (score < 60)
            {
                loan.State = LoanState.Rejected;
                loan.Customer.Score = scoreBeforeChange;
            }
        }

        private static int CahngeScoreBasedOnLoanToAssetRatio(
            int score, decimal loanAmount, decimal asset)
        {


            if (loanAmount < 0.5m * asset)
            {
                score += 20;
            }
            else if (loanAmount >= 0.5m *
                asset && loanAmount <= 0.7m * asset)
            {
                score += 10;
            }

            return score;
        }

        private static void StopIfLoanNotFound(Loan? loan)
        {
            if (loan == null)
            {
                throw new LoanNotFoundException();
            }
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
