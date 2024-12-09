using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagement.Entities
{
    [Table("Customers")]
    public class Customer : User
    {
        [Required]
        [StringLength(10)]
        public string NationalCode { get; set; }
        [Required]
        public int Score { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public FinancialInformation FinancialInformation { get; set; }

        public List<Loan> Loans { get; set; } = new();
    }
}
