global using SimpleMail.Lib;
using SimpleMailApp.Api.Services;
using SimpleMailApp.Api.Services.Contracts;

namespace SimpleMailApp.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //DI
        builder.Services.AddScoped<IEmailService, EmailService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //Avoiding cors policy
        app.UseCors(policy =>
        policy.WithOrigins("https://localhost:7123", "http://localhost:7123")
        .AllowAnyMethod()
        .AllowAnyHeader()
        //.WithHeaders(HeaderNames.ContentType)
        );

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}