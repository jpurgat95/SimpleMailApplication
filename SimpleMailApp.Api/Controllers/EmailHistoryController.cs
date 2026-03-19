using Microsoft.AspNetCore.Mvc;
using SimpleMailApp.Api.Services.Contracts;

namespace SimpleMailApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailHistoryController : ControllerBase
    {
        private readonly IEmailHistoryService _history;

        public EmailHistoryController(IEmailHistoryService history)
        {
            _history = history;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entries = await _history.GetAllAsync();
            return Ok(entries);
        }
    }
}