using LoanManagement.Services.FinancialInformations.Contracts;
using LoanManagement.Services.FinancialInformations.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.WebAPI.Controllers
{
    [Route("api/FinancialInformations")]
    [ApiController]
    public class FinancialInformationsController : ControllerBase
    {
        private readonly FinancialInformationService _service;

        public FinancialInformationsController(
            FinancialInformationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task Add(AddFinancialInformationDto dto)
        {
            await _service.Add(dto);
        }
    }
}
