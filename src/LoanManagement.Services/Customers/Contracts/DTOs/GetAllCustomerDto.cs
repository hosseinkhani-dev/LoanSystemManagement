using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.Customers.Contracts.DTOs
{
    public class GetAllCustomerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalCode { get; set; }
        public string? Email { get; set; }
        public int Score { get; set; }
        public bool IsActive { get; set; }
    }
}
