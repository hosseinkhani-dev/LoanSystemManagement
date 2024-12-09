using LoanManagement.Infrastructures.Applications;

namespace LoanManagement.Persistance.EF
{
    public class EFUnitOfWork : UnitOfWork
    {
        private readonly EFDbContext _context;

        public EFUnitOfWork(EFDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
