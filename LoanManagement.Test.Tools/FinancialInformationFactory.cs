using LoanManagement.Services.FinancialInformations.Contracts.DTOs;

namespace LoanManagement.Tests.Tools
{
    public static class FinancialInformationFactory
    {
        public static AddFinancialInformationDto
            GenerateAddFinancialInformationDto(int customerId)
        {
            return new AddFinancialInformationDto()
            {
                MonthlyIncome = Generator.GenerateDecimalNumber(),
                Job = Generator.GenerateString(),
                CustomerId = customerId
            };
        }
    }
}
