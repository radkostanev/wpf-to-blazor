using OutlookInspiredApp.Blazor.Models;
using OutlookInspiredApp.Blazor.Services.Interfaces;

namespace OutlookInspiredApp.Blazor.Services.Implementation
{
    /// <summary>
    /// Mail service implementation orchestrating mail operations
    /// </summary>
    public class MailService : IMailService
    {
        private readonly IMailRepository _repository;
        private readonly MailServiceState _state = new();

        public event Action? StateChanged;

        public MailService(IMailRepository repository)
        {
            _repository = repository;
        }

        public async Task LoadFoldersAsync()
        {
            _state.IsLoading = true;
            try
            {
                _state.Folders = await _repository.GetFoldersAsync();
                _state.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                _state.ErrorMessage = $"Error loading folders: {ex.Message}";
            }
            finally
            {
                _state.IsLoading = false;
                StateChanged?.Invoke();
            }
        }

        public async Task LoadEmailsAsync(string folderId)
        {
            _state.IsLoading = true;
            _state.SelectedFolderId = folderId;
            _state.SelectedEmail = null;  // Clear selection when switching folders
            try
            {
                _state.Emails = await _repository.GetEmailsByFolderAsync(folderId);
                _state.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                _state.ErrorMessage = $"Error loading emails: {ex.Message}";
            }
            finally
            {
                _state.IsLoading = false;
                StateChanged?.Invoke();
            }
        }

        public async Task SelectEmailAsync(string emailId)
        {
            _state.SelectedEmail = await _repository.GetEmailAsync(emailId);
            if (_state.SelectedEmail != null && _state.SelectedEmail.Status == EmailStatus.Unread)
            {
                await _repository.MarkAsReadAsync(emailId);
                _state.SelectedEmail.Status = EmailStatus.Read;

                // Decrement unread count on the matching folder
                var folderId = _state.SelectedEmail.FolderID;
                var folder = _state.Folders
                    .Concat(_state.Folders.SelectMany(f => f.SubFolders))
                    .FirstOrDefault(f => f.FolderID == folderId);
                if (folder != null && folder.UnreadCount > 0)
                    folder.UnreadCount--;

                // Also update the in-memory email list so the unread indicator clears immediately
                var listEmail = _state.Emails.FirstOrDefault(e => e.EmailID == emailId);
                if (listEmail != null)
                    listEmail.Status = EmailStatus.Read;
            }
            StateChanged?.Invoke();
        }

        public async Task<string> ComposeAsync(Email email)
        {
            email.EmailID = Guid.NewGuid().ToString();
            email.SentDate = DateTime.UtcNow;
            email.Status = EmailStatus.Read;
            await _repository.SaveEmailAsync(email);
            return email.EmailID;
        }

        public async Task SearchAsync(string query)
        {
            _state.IsLoading = true;
            try
            {
                _state.Emails = await _repository.SearchAsync(query);
                _state.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                _state.ErrorMessage = $"Error searching: {ex.Message}";
            }
            finally
            {
                _state.IsLoading = false;
            }
        }

        public async Task ReplyAsync(string emailId, string body)
        {
            var originalEmail = await _repository.GetEmailAsync(emailId);
            if (originalEmail != null)
            {
                var reply = new Email
                {
                    EmailID = Guid.NewGuid().ToString(),
                    Subject = originalEmail.Subject.StartsWith("RE:") 
                        ? originalEmail.Subject 
                        : $"RE: {originalEmail.Subject}",
                    From = "user@example.com",
                    To = originalEmail.From,
                    Body = body,
                    SentDate = DateTime.UtcNow,
                    Status = EmailStatus.Read
                };
                await _repository.SaveEmailAsync(reply);
            }
        }

        public MailServiceState GetState()
        {
            return _state;
        }

        public Task RequestComposeAsync(MailComposeAction action)
        {
            _state.PendingCompose = action;
            StateChanged?.Invoke();
            return Task.CompletedTask;
        }

        public async Task SetCategoryAsync(string? categoryId)
        {
            if (_state.SelectedEmail == null) return;
            _state.SelectedEmail.CategoryID = categoryId ?? string.Empty;
            await _repository.SaveEmailAsync(_state.SelectedEmail);
            // Sync in the email list too
            var listEmail = _state.Emails.FirstOrDefault(e => e.EmailID == _state.SelectedEmail.EmailID);
            if (listEmail != null) listEmail.CategoryID = _state.SelectedEmail.CategoryID;
            StateChanged?.Invoke();
        }

        public async Task SetFollowUpAsync(FollowUpType followUp)
        {
            if (_state.SelectedEmail == null) return;
            _state.SelectedEmail.FollowUpType = followUp;
            await _repository.SaveEmailAsync(_state.SelectedEmail);
            var listEmail = _state.Emails.FirstOrDefault(e => e.EmailID == _state.SelectedEmail.EmailID);
            if (listEmail != null) listEmail.FollowUpType = followUp;
            StateChanged?.Invoke();
        }

        public async Task ToggleReadAsync()
        {
            if (_state.SelectedEmail == null) return;
            var folderId = _state.SelectedEmail.FolderID;
            var folder = _state.Folders
                .Concat(_state.Folders.SelectMany(f => f.SubFolders))
                .FirstOrDefault(f => f.FolderID == folderId);

            if (_state.SelectedEmail.Status == EmailStatus.Read)
            {
                _state.SelectedEmail.Status = EmailStatus.Unread;
                if (folder != null) folder.UnreadCount++;
            }
            else
            {
                _state.SelectedEmail.Status = EmailStatus.Read;
                if (folder != null && folder.UnreadCount > 0) folder.UnreadCount--;
            }
            await _repository.SaveEmailAsync(_state.SelectedEmail);
            var listEmail = _state.Emails.FirstOrDefault(e => e.EmailID == _state.SelectedEmail.EmailID);
            if (listEmail != null) listEmail.Status = _state.SelectedEmail.Status;
            StateChanged?.Invoke();
        }
    }
}