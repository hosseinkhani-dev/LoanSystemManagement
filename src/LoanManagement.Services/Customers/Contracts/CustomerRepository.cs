using LoanManagement.Entities;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts.DTOs;

namespace LoanManagement.Services.Customers.Contracts
{
    public interface CustomerRepository : Repository
    {
        Task Add(Customer customer);
        Task<Customer?> FindById(int id);
        Task<List<GetAllCustomerDto>> GetAll();
        Task<bool> IsNationalCodeExist(string nationalCode);
        Task<bool> IsPhoneNumberExist(string phoneNumber);
    }
}
