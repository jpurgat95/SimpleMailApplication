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

    //public async Task<EmailDto> SendEmail(EmailDto email)
    //{
    //    try
    //    {
    //        var response = await _httpClient.PostAsJsonAsync<EmailDto>("api/Email", email);
    //        if (response.IsSuccessStatusCode)
    //        {
    //            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
    //            {
    //                return default(EmailDto);
    //            }
    //            return await response.Content.ReadFromJsonAsync<EmailDto>();
    //        }
    //        else
    //        {
    //            var message = await response.Content.ReadAsStringAsync();
    //            throw new Exception($"Http status:{response.StatusCode} Message -{message}");
    //        }
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }
    //}
}
