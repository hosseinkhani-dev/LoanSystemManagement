using LoanManagement.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.Loans.Contracts.DTOs
{
    public class GetAllLoanDto
    {
        public int Id { get; set; }
        public string State { get; set; }
        public DateTime? LoanStartDate { get; set; }
        public int CustomerId { get; set; }
        public int LoanTypeId { get; set; }
    }
}
