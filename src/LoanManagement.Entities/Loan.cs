using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagement.Entities
{
    [Table("Loans")]
    public class Loan
    {
        [Key]
        public int Id { get; set; }
        public DateTime? LoanStartDate { get; set; }
        [Required]
        public LoanState State { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer{ get; set; }

        [ForeignKey("LoanType")]
        public int LoanTypeId { get; set; }
        public LoanType LoanType { get; set; }

        public List<Repayment> Repayments { get; set; } = new();
    }

    public enum LoanState : byte
    {
        UnderReview,
        Approved,
        Rejected,
        Repaying,
        DelayInRepayment,
        Closed
    }
}
