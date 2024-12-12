using LoanManagement.Infrastructures.Applications;
using LoanManagement.Services.Customers.Contracts.DTOs;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;

namespace LoanManagement.Services.Customers.Contracts
{
    public interface CustomerService : Service
    {
        Task Activate(int id);
        Task Add(AddCustomerDto dto);
        Task AddFinancialInformation(AddFinancialInformationDto dto);
        Task Edit(EditCustomerDto dto, int id);
        //Task EditFinancialInformation(
        //    EditFinancialInformationDto dto, int customerId);
        Task<List<GetAllCustomerDto>> GetAll();
    }
}
