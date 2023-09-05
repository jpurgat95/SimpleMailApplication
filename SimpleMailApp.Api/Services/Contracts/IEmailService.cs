namespace SimpleMailApp.Api.Services.Contracts;

public interface IEmailService
{
    public void SendEmail(EmailDto request);
}
