using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace SimpleMailApp.WebAssembly.Pages
{
    public class EmailSenderBase : ComponentBase
    {
        [Inject] public IEmailService EmailSender { get; set; }
        [Inject] public IToastService ToastService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        public bool popup = true;
        public EmailDto model = new EmailDto();

        public string NewCcAddress { get; set; } = string.Empty;
        public string NewBccAddress { get; set; } = string.Empty;

        private bool _signatureLoaded = false;

        private readonly string[] allowedExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".pdf", ".xlsx", ".xls", ".doc", ".docx",
            ".txt", ".zip", ".rar", ".mp4", ".mp3", ".csv", ".ppt", ".pptx"
        };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !_signatureLoaded)
            {
                _signatureLoaded = true;
                await JSRuntime.InvokeVoidAsync("initQuill");
                var saved = await LoadSignature();
                if (!string.IsNullOrEmpty(saved))
                {
                    model.Signature = saved;
                    await JSRuntime.InvokeVoidAsync("quillSetContent", saved);
                }
            }
        }

        // CC
        public void AddCc()
        {
            if (!IsValidEmail(NewCcAddress))
            {
                ToastService.ShowError("Invalid CC email address.");
                return;
            }
            if (model.Cc.Contains(NewCcAddress))
            {
                ToastService.ShowWarning("This CC address is already added.");
                return;
            }
            model.Cc.Add(NewCcAddress);
            NewCcAddress = string.Empty;
        }

        public void RemoveCc(string address)
        {
            model.Cc.Remove(address);
        }

        // BCC
        public void AddBcc()
        {
            if (!IsValidEmail(NewBccAddress))
            {
                ToastService.ShowError("Invalid BCC email address.");
                return;
            }
            if (model.Bcc.Contains(NewBccAddress))
            {
                ToastService.ShowWarning("This BCC address is already added.");
                return;
            }
            model.Bcc.Add(NewBccAddress);
            NewBccAddress = string.Empty;
        }

        public void RemoveBcc(string address)
        {
            model.Bcc.Remove(address);
        }

        // Signature
        public async Task OnSignatureChanged()
        {
            model.Signature = await JSRuntime.InvokeAsync<string>("quillGetContent");
        }

        public async Task SaveSignature()
        {
            model.Signature = await JSRuntime.InvokeAsync<string>("quillGetContent");
            await JSRuntime.InvokeVoidAsync("localStorageSaveSignature", model.Signature);
            ToastService.ShowSuccess("Signature saved!");
        }

        public async Task ClearSignature()
        {
            model.Signature = string.Empty;
            await JSRuntime.InvokeVoidAsync("quillSetContent", string.Empty);
            await JSRuntime.InvokeVoidAsync("localStorageSaveSignature", string.Empty);
            ToastService.ShowInfo("Signature cleared.");
        }

        private async Task<string> LoadSignature()
        {
            return await JSRuntime.InvokeAsync<string>("localStorageGetSignature");
        }

        protected async Task SendEmail()
        {
            try
            {
                model.Signature = await JSRuntime.InvokeAsync<string>("quillGetContent");

                if (!IsValidEmail(model.To))
                {
                    ToastService.ShowError("Wrong email address, sir!");
                    return;
                }

                const long maxTotalAttachmentSize = 20 * 1024 * 1024;
                long totalAttachmentSize = model.Attachments?.Sum(a => a.Content?.LongLength ?? 0) ?? 0;

                if (totalAttachmentSize > maxTotalAttachmentSize)
                {
                    var totalMB = totalAttachmentSize / 1024.0 / 1024.0;
                    var maxMB = maxTotalAttachmentSize / 1024.0 / 1024.0;
                    ToastService.ShowError($"Total attachments size is {totalMB:F2} MB, which exceeds the {maxMB} MB limit.");
                    return;
                }

                await EmailSender.SendEmail(model);
                ToastService.ShowSuccess("Message sent, sir!");
                await Task.Delay(3000);
                NavigationManager.NavigateTo("", forceLoad: true);
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
            const long maxSingleFileSize = 10 * 1024 * 1024;
            const long maxTotalSize = 20 * 1024 * 1024;
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
                    ToastService.ShowError($"You can add max {maxFiles} files at once.");
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
                        if (read == 0) break;
                        totalRead += read;
                    }

                    if (totalRead != buffer.Length)
                    {
                        ToastService.ShowError($"Error loading file: '{file.Name}'.");
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
                    ToastService.ShowError($"Error reading file '{file.Name}': {ex.Message}");
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
            model.Attachments?.Remove(attachment);
        }

        public void RemoveAllAttachments()
        {
            model.Attachments?.Clear();
        }

        bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
    }
}