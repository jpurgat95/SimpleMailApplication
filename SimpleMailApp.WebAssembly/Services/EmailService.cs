using System.Net.Http.Json;

namespace SimpleMailApp.WebAssembly.Services;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;

    public EmailService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task SendEmail(EmailDto email)
    {
        var response = await _httpClient.PostAsJsonAsync("api/email", email);
        response.EnsureSuccessStatusCode();
    }
}
