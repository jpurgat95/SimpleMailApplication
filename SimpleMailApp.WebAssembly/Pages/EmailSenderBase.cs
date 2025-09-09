using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SimpleMail.Lib; // zakładam, że EmailDto i EmailAttachmentDto są tutaj
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleMailApp.WebAssembly.Pages
{
    public class EmailSenderBase : ComponentBase
    {
        [Inject]
        public IEmailService EmailSender { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public bool popup = true;

        public EmailDto model = new EmailDto();

        protected async Task SendEmail()
        {
            try
            {
                var emailAddress = model.To;
                if (!string.IsNullOrWhiteSpace(emailAddress) && emailAddress.Contains("@") && emailAddress.Contains("."))
                {
                    await EmailSender.SendEmail(model);
                    ToastService.ShowSuccess("Message sent, sir!");
                    await Task.Delay(3000);
                    NavigationManager.NavigateTo("", forceLoad: true);
                }
                else
                {
                    ToastService.ShowError("Wrong email address, sir!");
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error sending email: {ex.Message}");
            }
        }

        protected async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            model.Attachments = new List<EmailAttachmentDto>();
            foreach (var file in e.GetMultipleFiles())
            {
                var buffer = new byte[file.Size];
                await file.OpenReadStream().ReadAsync(buffer);
                model.Attachments.Add(new EmailAttachmentDto
                {
                    FileName = file.Name,
                    Content = buffer,
                    ContentType = file.ContentType
                });
            }
        }
    }
}
