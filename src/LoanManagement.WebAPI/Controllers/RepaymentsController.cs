using LoanManagement.Services.Repayments.Contracts;
using LoanManagement.Services.Repayments.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.WebAPI.Controllers
{
    [Route("api/Repayments")]
    [ApiController]
    public class RepaymentsController : ControllerBase
    {
        private readonly RepaymentService _service;

        public RepaymentsController(RepaymentService service)
        {
            _service = service;
        }

        [HttpPost("generateRepayments")]
        public async Task GenerateRepayments(int loanId)
        {
            await _service.GenerateRepayments(loanId);
        }

        [HttpGet("allUnpaid")]
        public async Task<List<GetAllUnpaidRepaymentsDto?>> GetAllUnpaid(
            int loanId)
        {
            return await _service.GetAllUnpaid(loanId);
        }

        [HttpPut("{id}/payRepayment")]
        public async Task PayRepayment(int id)
        {
            await _service.PayRepayment(id);
        }

    }
}
