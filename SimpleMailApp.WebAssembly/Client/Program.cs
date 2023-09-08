global using SimpleMail.Lib;
global using SimpleMailApp.WebAssembly.Services.Contracts;
global using Blazored.Toast;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleMailApp.WebAssembly;
using SimpleMailApp.WebAssembly.Services;
using Radzen;

namespace SimpleMailApp.WebAssembly
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            //HttpClient address added
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7016/") });

            //DI
            builder.Services.AddScoped<IEmailService, EmailService>();

            //Toast Notification
            builder.Services.AddBlazoredToast();

            //Radzen DI registrations
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();

            await builder.Build().RunAsync();
        }
    }
}