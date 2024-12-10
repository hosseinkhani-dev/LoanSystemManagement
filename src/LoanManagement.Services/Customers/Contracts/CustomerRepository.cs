using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;

namespace LoanManagement.Services.Customers.Contracts
{
    public interface CustomerRepository : Repository
    {
        Task Add(Customer customer);
        Task<bool> IsNationalCodeExist(string nationalCode);
        Task<bool> IsPhoneNumberExist(string phoneNumber);
    }
}
