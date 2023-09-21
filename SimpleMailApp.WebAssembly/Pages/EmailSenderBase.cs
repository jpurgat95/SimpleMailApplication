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

    protected async Task SendEmail()
    {
        try
        {
            var check = model.From;
            if(check.Contains("@") && check.Contains(".") ) 
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
    public string value;

    public void OnChange(string value, string name)
    {
        Console.WriteLine($"{name} value changed to {value}");
    }

    public bool popup = true;

    public EmailDto model = new EmailDto();

    public void Log(string eventName, string value)
    {
        Console.WriteLine($"{eventName}: {value}");
    }

    public void OnSubmit(EmailDto model)
    {
        Log("Submit", JsonSerializer.Serialize(model, new JsonSerializerOptions() { WriteIndented = true }));
    }

    public void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        Log("InvalidSubmit", JsonSerializer.Serialize(args, new JsonSerializerOptions() { WriteIndented = true }));
    }
}
