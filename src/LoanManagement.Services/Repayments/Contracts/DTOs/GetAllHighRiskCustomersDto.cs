namespace LoanManagement.Services.Repayments.Contracts.DTOs
{
    public class GetAllHighRiskCustomersDto
    {
        public int CustomerId { get; set; }
        public byte TottalLateCount { get; set; }
    }
}
