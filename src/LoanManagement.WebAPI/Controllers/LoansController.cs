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

        [HttpPatch("{id}/LoanCheck")]
        public async Task<string> LoanCheck(int id)
        {
            return await _service.LoanCheck(id);
        }

        [HttpGet("GetAllActiveLoansReport")]
        public async Task<List<GetAllLoanActiveReportDto>>
            GetAllActiveLoansReport()
        {
            return await _service.GetAllActiveLoansReport();
        }

        [HttpGet("GetTotalInterestAndPenalty")]
        public async Task<GetTotalInterestAndPenaltyDto>
            GetTotalInterestAndPenalty()
        {
            return await _service.GetTotalInterestAndPenalty();
        }

        [HttpGet("GetAllClosedReport")]
        public async Task<List<GetAllClosedLoanReportDto>>
            GetAllClosedReport()
        {
            return await _service.GetAllClosedReport();
        }
    }
}
