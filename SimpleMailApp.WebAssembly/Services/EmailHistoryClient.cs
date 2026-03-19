using System.Net.Http.Json;
using SimpleMailApp.WebAssembly.Services.Contracts;

namespace SimpleMailApp.WebAssembly.Services
{
    public class EmailHistoryClient : IEmailHistoryClient
    {
        private readonly HttpClient _http;

        public EmailHistoryClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<EmailHistoryEntry>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<EmailHistoryEntry>>("api/EmailHistory")
                   ?? new List<EmailHistoryEntry>();
        }
    }
}