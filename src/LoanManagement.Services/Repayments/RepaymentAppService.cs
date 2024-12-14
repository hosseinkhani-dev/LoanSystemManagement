using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Exceptions;
using LoanManagement.Services.Loans.Contracts;
using LoanManagement.Services.Loans.Exceptions;
using LoanManagement.Services.LoanTypes.Contracts;
using LoanManagement.Services.LoanTypes.Exceptions;
using LoanManagement.Services.Repayments.Contracts;
using LoanManagement.Services.Repayments.Contracts.DTOs;
using LoanManagement.Services.Repayments.Exceptions;

namespace LoanManagement.Services.Repayments
{
    public class RepaymentAppService : RepaymentService
    {
        private readonly RepaymentRepository _repository;
        private readonly UnitOfWork _untiOfWork;
        private readonly LoanRepository _loanRepository;
        private readonly LoanTypeRepository _loanTypeRepository;

        public RepaymentAppService(
            RepaymentRepository repository,
            UnitOfWork unitOfWork,
            LoanRepository loanRepository,
            LoanTypeRepository loanTypeRepository)
        {
            _repository = repository;
            _untiOfWork = unitOfWork;
            _loanRepository = loanRepository;
            _loanTypeRepository = loanTypeRepository;
        }

        public async Task GenerateRepayments(int loanId)
        {
            Loan? loan = await _loanRepository.FindById(loanId);
            StopIfLoanNotFound(loan);
            StopIfRepaymentAlreadyExist(loan);
            StopIfLoanTypeNotFound(loan!.LoanType);

            await GenerateRepaymentsAndSendToRepository(loan);
        }

        public async Task<List<GetAllUnpaidRepaymentsDto?>> GetAllUnpaid(
           int loanId)
        {
            return await _repository.GetAllUnpaid(loanId);
        }

        public async Task PayRepayment(int id)
        {
            Repayment? repayment = await _repository.FindById(id);
            StopIfRepaymentNotFound(repayment);

            var loan = await _loanRepository.FindById(repayment!.Id);
            StopIfLoanNotFound(loan);

            var customer = loan!.Customer;
            StopWhenCustomerNotFound(customer);

            StopIfRepaymentAlreadyPaid(repayment);

            repayment.IsRepaid = true;
            repayment.PaymentDate = DateTime.Now;
            repayment.TotalRepaid += repayment.Amount;
            repayment.RepaymentCount += 1;
            if (DateTime.Now.Day > repayment.DueDate.Day)
            {
                var penalty = repayment.Amount * 0.02m;
                repayment.TotalLatePenalty += penalty;
                repayment.LatePenaltyCount += 1;
                customer.Score -= 5;
                loan.State = LoanState.DelayInRepayment;
            }
            if (repayment.RepaymentCount >= loan.LoanType.RepaymentPeriod)
            {
                repayment.IsFullyRepaid = true;
                loan.State = LoanState.Closed;
            }

            if (repayment.IsFullyRepaid == true &&
                repayment.LatePenaltyCount == 0)
            {
                customer.Score += 30;
            }

            await _untiOfWork.CommitAsync();
        }

        public async Task<List<GetAllHighRiskCustomersDto>>
           GetAllHighRiskCustomers()
        {
            return await _repository.GetAllHighRiskCustomers();
        }

        private static void StopIfRepaymentNotFound(Repayment? repayment)
        {
            if (repayment == null)
            {
                throw new RepaymentNotFoundException();
            }
        }

        private static void StopIfRepaymentAlreadyPaid(Repayment? repayment)
        {
            if (repayment.IsRepaid)
            {
                throw new ThisRepaymentIsAlreadyPaidException();
            }
        }

        private async Task GenerateRepaymentsAndSendToRepository(Loan loan)
        {
            var loanType = loan!.LoanType;
            var monthlyRepayment = loanType.MonthlyRepayment;
            var repaymentPeriod = loanType.RepaymentPeriod;
            var startDate = DateTime.Now;

            for (int i = 0; i < repaymentPeriod; i++)
            {
                var repayment = new Repayment
                {
                    DueDate = startDate.AddMonths(i),
                    Amount = monthlyRepayment,
                    LoanId = loan.Id,
                    TotalRepaid = 0,
                    RepaymentCount = 0,
                    TotalLatePenalty = 0,
                    LatePenaltyCount = 0,
                    IsFullyRepaid = false,
                    IsRepaid = false
                };

                await _repository.GenerateRepayments(repayment);
                await _untiOfWork.CommitAsync();
            }
            loan.State = LoanState.Repaying;
        }

        private static void StopIfRepaymentAlreadyExist(Loan? loan)
        {
            if (loan!.Repayments.Any())
            {
                throw new RepaymentsAlreadyExistForThisLoanException();
            }
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
        private static void StopWhenCustomerNotFound(Customer? customer)
        {
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
        }
    }
}
