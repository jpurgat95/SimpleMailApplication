using Microsoft.Azure.Cosmos;
using SimpleMailApp.Api.Services.Contracts;

namespace SimpleMailApp.Api.Services
{
    public class EmailHistoryService : IEmailHistoryService
    {
        private readonly Container _container;

        public EmailHistoryService(IConfiguration config)
        {
            var client = new CosmosClient(config["CosmosDb:ConnectionString"]);
            var database = client.GetDatabase(config["CosmosDb:DatabaseName"]);
            _container = database.GetContainer(config["CosmosDb:ContainerName"]);
        }

        public async Task SaveAsync(EmailHistoryEntry entry)
        {
            await _container.CreateItemAsync(entry, new PartitionKey(entry.To));
        }

        public async Task<List<EmailHistoryEntry>> GetAllAsync()
        {
            var query = _container.GetItemQueryIterator<EmailHistoryEntry>(
                new QueryDefinition("SELECT * FROM c ORDER BY c.sentAt DESC"));

            var results = new List<EmailHistoryEntry>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }
    }
}