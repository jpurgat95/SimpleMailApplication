namespace SimpleMailApp.WebAssembly.Services.Contracts;

public interface IEmailService
{
    Task<EmailDto> SendEmail(EmailDto email);
}
