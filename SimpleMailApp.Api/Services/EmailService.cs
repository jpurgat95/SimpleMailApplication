using MailKit.Security;
using MimeKit;
using SimpleMailApp.Api.Services.Contracts;
using MailKit.Net.Smtp;

namespace SimpleMailApp.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IEmailHistoryService _history;

        public EmailService(IConfiguration config, IEmailHistoryService history)
        {
            _config = config;
            _history = history;
        }

        public async void SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));

            if (request.Cc != null)
                foreach (var cc in request.Cc)
                    if (!string.IsNullOrWhiteSpace(cc))
                        email.Cc.Add(MailboxAddress.Parse(cc));

            if (request.Bcc != null)
                foreach (var bcc in request.Bcc)
                    if (!string.IsNullOrWhiteSpace(bcc))
                        email.Bcc.Add(MailboxAddress.Parse(bcc));

            email.Subject = request.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = string.IsNullOrWhiteSpace(request.Signature)
                ? request.Body
                : $"{request.Body}<br/><br/><hr/>{request.Signature}";

            if (request.Attachments != null && request.Attachments.Any())
                foreach (var attachment in request.Attachments)
                    builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);

            // Zapisz do historii
            await _history.SaveAsync(new EmailHistoryEntry
            {
                To = request.To,
                Cc = request.Cc ?? new List<string>(),
                Bcc = request.Bcc ?? new List<string>(),
                Subject = request.Subject,
                Body = request.Body,
                HasAttachments = request.Attachments != null && request.Attachments.Any(),
                SentAt = DateTime.UtcNow
            });
        }
    }
}