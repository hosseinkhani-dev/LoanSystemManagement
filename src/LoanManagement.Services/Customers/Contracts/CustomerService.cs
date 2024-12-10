using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts.DTOs;

namespace LoanManagement.Services.Customers.Contracts
{
    public interface CustomerService : Service
    {
        Task Activate(int id);
        Task Add(AddCustomerDto dto);
        Task Edit(EditCustomerDto dto, int id);
        Task<List<GetAllCustomerDto>> GetAll();
    }
}
