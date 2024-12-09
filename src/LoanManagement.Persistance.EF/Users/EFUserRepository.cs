using LoanManagement.Entities;
using LoanManagement.Services.Users.Contracts;

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
    }
}
