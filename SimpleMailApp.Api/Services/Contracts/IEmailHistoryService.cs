namespace SimpleMailApp.Api.Services.Contracts
{
    public interface IEmailHistoryService
    {
        Task SaveAsync(EmailHistoryEntry entry);
        Task<List<EmailHistoryEntry>> GetAllAsync();
    }
}