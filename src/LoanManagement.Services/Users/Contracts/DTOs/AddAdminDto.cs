using LoanManagement.Entities;
using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.Users.Contracts.DTOs
{
    public class AddAdminDto
    {
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
