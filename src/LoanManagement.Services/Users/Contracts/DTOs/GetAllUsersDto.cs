using LoanManagement.Entities;
using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.Users.Contracts.DTOs
{
    public class GetAllUsersDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}
