namespace SimpleMailApp.WebAssembly.Services.Contracts
{
    public interface IEmailHistoryClient
    {
        Task<List<EmailHistoryEntry>> GetAllAsync();
    }
}