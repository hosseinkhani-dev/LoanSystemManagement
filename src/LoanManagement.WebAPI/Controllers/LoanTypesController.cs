using LoanManagement.Services.Loans.Contracts.DTOs;
using LoanManagement.Services.LoanTypes.Contracts;
using LoanManagement.Services.LoanTypes.Contracts.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.WebAPI.Controllers
{
    [Route("api/LoanTypes")]
    [ApiController]
    public class LoanTypesController : ControllerBase
    {
        private readonly LoanTypeService _service;

        public LoanTypesController(LoanTypeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task Add(AddLoanTypeDto dto) 
        {
            await _service.Add(dto);
        }

        [HttpGet]
        public async Task<List<GetAllLoanTypeDto>> GetAll()
        {
            return await _service.GetAll();
        }
    }
}
