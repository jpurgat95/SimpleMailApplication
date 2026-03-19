namespace SimpleMail.Lib
{
    public class EmailDto
    {
        public string To { get; set; } = string.Empty;
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public List<EmailAttachmentDto> Attachments { get; set; } = new List<EmailAttachmentDto>();
    }
}