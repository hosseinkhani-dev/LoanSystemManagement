using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagement.Entities
{
    [Table("Repayments")]
    public class Repayment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public byte RepaymentCount { get; set; }
        [Required]
        public decimal TotalLatePenalty { get; set; }
        [Required]
        public byte LatePenaltyCount { get; set; }
        [Required]
        public bool IsRepaid { get; set; }
        [Required]
        public bool IsFullyRepaid { get; set; }

        [ForeignKey("Loan")]
        public int LoanId { get; set; }
        public Loan Loan { get; set; }
    }
}
