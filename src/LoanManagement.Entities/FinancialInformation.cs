using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagement.Entities
{
    [Table("FinancialInformations")]
    public class FinancialInformation
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        public decimal MonthlyIncome { get; set; }
        [Required]
        public JobType Job { get; set; }
        public decimal? FinancialAssets { get; set; }
        
        public Customer Customer { get; set; }
    }

    public enum JobType : byte
    {
        GovernmentJob,
        FreelanceJob,
        WithoutJob
    }
}
