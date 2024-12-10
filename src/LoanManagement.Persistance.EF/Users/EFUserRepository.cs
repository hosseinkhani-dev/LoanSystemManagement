using LoanManagement.Entities;
using LoanManagement.Services.Users.Contracts;
using LoanManagement.Services.Users.Contracts.DTOs;
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

        public async Task AddManager(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> FindById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<GetAllUsersDto>> GetAll()
        {
            return await _context.Users.
                Select(x => new GetAllUsersDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Role = x.Role.ToString(),
                }).ToListAsync();
        }

        public async Task<bool> IsNameExist(string firstName, string lastName)
        {
            return await _context.Users.AnyAsync(
                x => x.FirstName == firstName && x.LastName == lastName);
        }
    }
}
