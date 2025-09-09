using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleMailApp.Api.Services.Contracts;

namespace SimpleMailApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    [HttpPost]
    public IActionResult SendEmail([FromBody] EmailDto request)
    {
        _emailService.SendEmail(request);
        return Ok();
    }
}
