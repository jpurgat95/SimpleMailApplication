using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
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
            EmailSender.SendEmail(model);
            ToastService.ShowSuccess("Messeage sent, sir!");

            var timer = new Timer(new TimerCallback(_ =>
            {
                NavigationManager.NavigateTo("", forceLoad: true);
            }), null, 3000, 3000);
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

    public bool popup;

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
