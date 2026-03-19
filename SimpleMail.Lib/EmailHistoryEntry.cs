using Newtonsoft.Json;
namespace SimpleMail.Lib
{
    public class EmailHistoryEntry
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("to")]
        public string To { get; set; } = string.Empty;
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool HasAttachments { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}