using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagement.Entities
{
    [Table("LoanTypes")]
    public class LoanType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal InterestRate { get; set; }
        [Required]
        public byte RepaymentPeriod { get; set; }
        [Required]
        public decimal MonthlyRepayment { get; set; }

        public List<Loan> Loans { get; set; } = new();
    }
}
