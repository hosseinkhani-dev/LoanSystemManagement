using LoanManagement.Services.Customers.Contracts;
using LoanManagement.Services.Customers.Contracts.DTOs;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;
using LoanManagement.Services.Users.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.WebAPI.Controllers
{
    [Route("api/Customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _service;

        public CustomersController(CustomerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task Add(AddCustomerDto dto)
        {
            await _service.Add(dto);
        }

        [HttpPatch("{id}/activate")]
        public async Task Activate(int id)
        {
            await _service.Activate(id);
        }

        [HttpPatch("{id}/edit")]
        public async Task Edit(EditCustomerDto dto, int id)
        {
            await _service.Edit(dto, id);
        }

        [HttpGet]
        public async Task<List<GetAllCustomerDto>> GetAll()
        {
            return await _service.GetAll();
        }
    }
}
