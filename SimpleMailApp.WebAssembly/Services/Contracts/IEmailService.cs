namespace SimpleMailApp.WebAssembly.Services.Contracts;

public interface IEmailService
{
    Task SendEmail(EmailDto email);
}
