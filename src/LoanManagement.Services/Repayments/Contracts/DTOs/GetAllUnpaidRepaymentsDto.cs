using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.Repayments.Contracts.DTOs
{
    public class GetAllUnpaidRepaymentsDto
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
    }
}
