using Microsoft.AspNetCore.Components;

namespace SimpleMailApp.WebAssembly.Pages;

public class IndexBase : ComponentBase
{
    [Inject]
    IEmailService EmailSender { get; set; }

    protected async Task SendEmail()
    {
        try
        {
            var test = new EmailDto
            {
                To = "cassandre.goodwin@ethereal.email",
                Subject = "Testowy temat",
                Body = "Testowa wiadomość",
            };
            EmailSender.SendEmail(test);
        }
        catch (Exception)
        {

            throw;
        }
    }
}
