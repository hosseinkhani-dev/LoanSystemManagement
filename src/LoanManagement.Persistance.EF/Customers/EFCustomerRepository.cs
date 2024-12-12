using LoanManagement.Entities;
using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Contracts.DTOs;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Persistance.EF.Customers
{
    public class EFCustomerRepository : CustomerRepository
    {
        private readonly EFDbContext _context;

        public EFCustomerRepository(EFDbContext context)
        {
            _context = context;
        }

        public async Task Add(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task<Customer?> FindById(int id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<GetAllCustomerDto>> GetAll()
        {
            return await _context.Customers
                .Select(x => new GetAllCustomerDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    NationalCode = x.NationalCode,
                    PhoneNumber = x.PhoneNumber,
                    IsActive = x.IsActive,
                    Score = x.Score,
                    Email = x.Email,
                    GetFinancialInformationDto =
                        x.FinancialInformation == null ?
                        null : new GetFinancialInformationDto
                        {
                            MonthlyIncome = x.FinancialInformation.
                            MonthlyIncome,
                            Job = x.FinancialInformation.Job.ToString(),
                            FinancialAssets =
                        x.FinancialInformation.FinancialAssets,
                        }
                }).ToListAsync();
        }

        public async Task<bool> IsNationalCodeExist(string nationalCode)
        {
            return await _context.Customers.AnyAsync(
                x => x.NationalCode == nationalCode);
        }

        public async Task<bool> IsPhoneNumberExist(string phoneNumber)
        {
            return await _context.Customers.AnyAsync(
                x => x.PhoneNumber == phoneNumber);
        }
    }
}
