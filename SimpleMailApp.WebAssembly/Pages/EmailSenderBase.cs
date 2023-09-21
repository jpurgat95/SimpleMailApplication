using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace SimpleMailApp.WebAssembly.Pages;

public class EmailSenderBase : ComponentBase
{
    [Inject]
    IEmailService EmailSender { get; set; }
    [Inject]
    IToastService ToastService { get; set; }
    [Inject]
    NavigationManager NavigationManager { get; set; }
    public bool popup = true;

    public EmailDto model = new EmailDto();
    protected async Task SendEmail()
    {
        try
        {
            var emailAddres = model.From;
            if(emailAddres.Contains("@") && emailAddres.Contains(".") )
            {
                EmailSender.SendEmail(model);
                ToastService.ShowSuccess("Messeage sent, sir!");

                var timer = new Timer(new TimerCallback(_ =>
                {
                    NavigationManager.NavigateTo("", forceLoad: true);
                }), null, 3000, 3000);
            }
            else
            {
                ToastService.ShowError("Wrong email addres, sir!");
            }

        }
        catch (Exception)
        {

            throw;
        }
    }
}