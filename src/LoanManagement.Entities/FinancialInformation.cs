using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagement.Entities
{
    [Table("FinancialInformations")]
    public class FinancialInformation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public decimal MonthlyIncome { get; set; }
        [Required]
        [StringLength(20)]
        public string Job { get; set; }
        public decimal? FinancialAssets { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
