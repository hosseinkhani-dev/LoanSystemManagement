using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.Customers.Contracts.DTOs
{
    public class AddCustomerDto
    {
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
    }
}
