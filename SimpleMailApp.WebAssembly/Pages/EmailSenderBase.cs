using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SimpleMail.Lib; // zakładam, że EmailDto i EmailAttachmentDto są tutaj
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

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
            ".jpg", ".jpeg", ".png", ".pdf", ".xlsx", ".xls", ".doc", ".docx", ".txt",".zip", ".rar", ".mp4", ".mp3", ".csv"
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

            var allowedExtensions = new[]
            {
                ".jpg", ".jpeg", ".png", ".pdf", ".xlsx", ".xls", ".doc", ".docx", ".txt", ".zip", ".rar", ".mp4", ".mp3", ".csv"
            };

            const long maxSingleFileSize = 10 * 1024 * 1024;
            const long maxTotalAttachmentSize = 20 * 1024 * 1024;
            long currentTotalSize = model.Attachments.Sum(a => a.Content?.LongLength ?? 0);

            foreach (var file in e.GetMultipleFiles())
            {
                var extension = System.IO.Path.GetExtension(file.Name).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ToastService.ShowError($"File type {extension} is not supported.");
                    continue;
                }

                if (file.Size > maxSingleFileSize)
                {
                    ToastService.ShowError($"File {file.Name} is too large. Max size is 10 MB.");
                    continue;
                }

                if (currentTotalSize + file.Size > maxTotalAttachmentSize)
                {
                    ToastService.ShowError($"Adding file {file.Name} would exceed total attachments size limit of 20 MB.");
                    continue;
                }

                try
                {
                    var buffer = new byte[file.Size];
                    using var stream = file.OpenReadStream(file.Size);
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
                        ToastService.ShowError($"Error reading full content of file {file.Name}.");
                        continue;
                    }

                    model.Attachments.Add(new EmailAttachmentDto
                    {
                        FileName = file.Name,
                        Content = buffer,
                        ContentType = file.ContentType
                    });

                    currentTotalSize += file.Size;

                    Console.WriteLine($"Processing file: {file.Name} Size: {file.Size}");
                }
                catch (Exception ex)
                {
                    ToastService.ShowError($"Error reading file {file.Name}: {ex.Message}");
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
