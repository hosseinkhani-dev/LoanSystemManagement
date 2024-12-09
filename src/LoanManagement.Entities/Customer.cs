using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagement.Entities
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(10)]
        public string NationalCode { get; set; }
        [StringLength(200)]
        public string? Email { get; set; }
        [Required]
        public int Score { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public FinancialInformation FinancialInformation { get; set; }

        public List<Loan> Loans { get; set; } = new();
    }
}
