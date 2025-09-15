using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

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
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public bool popup = true;
        public EmailDto model = new EmailDto();

        // Lista dozwolonych rozszerzeń plików
        private readonly string[] allowedExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".pdf", ".xlsx", ".xls", ".doc", ".docx", ".txt",".zip", 
            ".rar", ".mp4", ".mp3", ".csv",".ppt", ".pptx"
        };

        protected async Task SendEmail()
        {
            try
            {
                var emailAddress = model.To;
                if (!string.IsNullOrWhiteSpace(emailAddress) && emailAddress.Contains("@") && emailAddress.Contains("."))
                {
                    const long maxTotalAttachmentSize = 20 * 1024 * 1024; // 20 MB
                    long totalAttachmentSize = model.Attachments?.Sum(a => a.Content?.LongLength ?? 0) ?? 0;

                    if (totalAttachmentSize > maxTotalAttachmentSize)
                    {
                        var totalMB = totalAttachmentSize / 1024.0 / 1024.0;
                        var maxMB = maxTotalAttachmentSize / 1024.0 / 1024.0;
                        ToastService.ShowError($"Total attachments size is {totalMB:F2} MB, which exceeds the {maxMB} MB limit. Please remove some files.");
                        return;
                    }

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
            if (model.Attachments == null)
                model.Attachments = new List<EmailAttachmentDto>();

            const int maxFiles = 10;
            const long maxSingleFileSize = 10 * 1024 * 1024; // 10 MB
            const long maxTotalSize = 20 * 1024 * 1024; // 20 MB
            long currentTotalSize = model.Attachments.Sum(a => a.Content?.LongLength ?? 0);

            IReadOnlyList<IBrowserFile> selectedFiles;
            try
            {
                selectedFiles = e.GetMultipleFiles(maxFiles);
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message);
                return;
            }

            foreach (var file in selectedFiles)
            {
                if (model.Attachments.Count >= maxFiles)
                {
                    ToastService.ShowError($"You can add {maxFiles} files at once.");
                    break;
                }
                var extension = Path.GetExtension(file.Name)?.ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    ToastService.ShowError($"File type '{extension}' not allowed.");
                    continue;
                }

                if (file.Size > maxSingleFileSize)
                {
                    ToastService.ShowError($"File {file.Name} is too big (max 10 MB).");
                    continue;
                }

                if (currentTotalSize + file.Size > maxTotalSize)
                {
                    ToastService.ShowError("Total files size limit exceeded (20 MB).");
                    break;
                }

                try
                {
                    var buffer = new byte[file.Size];
                    using var stream = file.OpenReadStream(maxSingleFileSize);
                    int totalRead = 0;
                    while (totalRead < buffer.Length)
                    {
                        int read = await stream.ReadAsync(buffer, totalRead, buffer.Length - totalRead);
                        if (read == 0)
                            break;
                        totalRead += read;
                    }
                    if (totalRead != buffer.Length)
                    {
                        ToastService.ShowError($"Error during files loading: '{file.Name}'.");
                        continue;
                    }
                    model.Attachments.Add(new EmailAttachmentDto
                    {
                        FileName = file.Name,
                        Content = buffer,
                        ContentType = file.ContentType
                    });
                    currentTotalSize += file.Size;
                    ToastService.ShowSuccess($"File '{file.Name}' added.");
                }
                catch (InvalidOperationException ex)
                {
                    ToastService.ShowError($"Error reading file: '{file.Name}': {ex.Message}");
                }
            }
        }
        public async Task TriggerFileInput()
        {
            await JSRuntime.InvokeVoidAsync("triggerFileInputClick");
        }

        public double TotalAttachmentsSizeMB =>
            model.Attachments?.Sum(a => a.Content?.LongLength ?? 0) / 1024.0 / 1024.0 ?? 0;

        public void RemoveAttachment(EmailAttachmentDto attachment)
        {
            if (model.Attachments != null)
            {
                model.Attachments.Remove(attachment);
            }
        }

        public void RemoveAllAttachments()
        {
            if (model.Attachments != null)
            {
                model.Attachments.Clear();
            }
        }
    }
}
