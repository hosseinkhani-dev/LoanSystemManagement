using LoanManagement.Services.Users.Contracts;
using LoanManagement.Services.Users.Contracts.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.WebAPI.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;
        public UsersController(UserService service) 
        {
            _service = service;
        }

        [HttpPost]
        public async Task AddAdmin([FromBody] AddAdminDto dto)
        {
            await _service.AddAdmin(dto);
        }
    }
}
