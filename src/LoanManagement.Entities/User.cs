using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagement.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        public string LastName{ get; set; }
        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }
        [StringLength(200)]
        public string? Email { get; set; }
        [Required]
        public Role Role{ get; set; }
    }

    public enum Role : byte
    {
        Admin,
        Management,
        Customer
    }
}
