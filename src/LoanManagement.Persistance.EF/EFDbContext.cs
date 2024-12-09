using LoanManagement.Entities;
using LoanManagement.Persistance.EF.Users;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Persistance.EF
{
    public class EFDbContext : DbContext
    {
        public EFDbContext(string connectionString) :
         this(new DbContextOptionsBuilder()
             .UseSqlServer(connectionString).Options)
        { }

        public EFDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(UserEntityMap).Assembly);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<FinancialInformation> FinancialInformations { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Repayment> Repayments { get; set; }
    }
}
