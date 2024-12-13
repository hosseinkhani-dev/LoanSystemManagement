using LoanManagement.Services.Loans.Contracts;
using LoanManagement.Services.Loans.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.WebAPI.Controllers
{
    [Route("api/Loans")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly LoanService _service;

        public LoansController(LoanService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task Add(AddLoanDto dto)
        {
            await _service.Add(dto);
        }

        [HttpGet]
        public async Task<List<GetAllLoanDto?>> GetAll()
        {
            return await _service.GetAll();
        }
    }
}
