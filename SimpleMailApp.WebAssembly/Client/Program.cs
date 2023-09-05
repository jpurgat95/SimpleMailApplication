global using SimpleMail.Lib;
global using SimpleMailApp.WebAssembly.Services.Contracts;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleMailApp.WebAssembly;
using SimpleMailApp.WebAssembly.Services;

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

            await builder.Build().RunAsync();
        }
    }
}