using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts.DTOs;

namespace LoanManagement.Services.Customers.Contracts
{
    public interface CustomerService : Service
    {
        Task Add(AddCustomerDto dto);
    }
}
