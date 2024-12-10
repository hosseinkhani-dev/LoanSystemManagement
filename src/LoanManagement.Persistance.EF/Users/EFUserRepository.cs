using LoanManagement.Entities;
using LoanManagement.Services.Users.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Persistance.EF.Users
{
    public class EFUserRepository : UserRepository
    {
        private readonly EFDbContext _context;

        public EFUserRepository(EFDbContext context)
        {
            _context = context;
        }

        public async Task AddAdmin(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<bool> IsNameExist(string firstName, string lastName)
        {
            return await _context.Users.AnyAsync(
                x => x.FirstName == firstName && x.LastName == lastName);
        }
    }
}
