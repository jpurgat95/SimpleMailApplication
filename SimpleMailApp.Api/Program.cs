global using SimpleMail.Lib;
using SimpleMailApp.Api.Services;
using SimpleMailApp.Api.Services.Contracts;

namespace SimpleMailApp.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //DI
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddSingleton<IEmailHistoryService, EmailHistoryService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(policy =>
        policy.WithOrigins("https://localhost:7123", "http://localhost:7123")
        .AllowAnyMethod()
        .AllowAnyHeader()
        );

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}