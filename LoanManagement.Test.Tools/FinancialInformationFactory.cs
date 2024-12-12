using LoanManagement.Entities;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;

namespace LoanManagement.Tests.Tools
{
    public static class FinancialInformationFactory
    {
        public static FinancialInformation CreateFinancialInformation(
            int customerId,
            decimal monthlyIncome,
            JobType job)
        {
            return new FinancialInformation
            {
                CustomerId = customerId,
                MonthlyIncome = monthlyIncome,
                Job = job
            };
        }

        public static AddFinancialInformationDto CreateAddDto(
            int customerId,
            decimal monthlyIncome,
            JobType job)
        {
            return new AddFinancialInformationDto
            {
                CustomerId = customerId,
                MonthlyIncome = monthlyIncome,
                Job = job
            };
        }
    }
}
