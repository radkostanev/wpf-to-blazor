using OutlookInspiredApp.Blazor.Models;
using OutlookInspiredApp.Blazor.Services.Interfaces;

namespace OutlookInspiredApp.Blazor.Services.Implementation
{
    /// <summary>
    /// Mail repository implementation providing email data access
    /// </summary>
    public class MailRepository : IMailRepository
    {
        private readonly IDataStore _dataStore;

        public MailRepository(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<List<Email>> GetEmailsByFolderAsync(string folderId, int skip = 0, int take = 50)
        {
            var allEmails = await _dataStore.GetEmailsByFolderAsync(folderId);
            return allEmails.Skip(skip).Take(take).ToList();
        }

        public async Task<int> GetUnreadCountAsync(string folderId)
        {
            var emails = await _dataStore.GetEmailsByFolderAsync(folderId);
            return emails.Count(e => e.Status == EmailStatus.Unread);
        }

        public async Task MarkAsReadAsync(string emailId)
        {
            var email = await _dataStore.GetEmailAsync(emailId);
            if (email != null)
            {
                email.Status = EmailStatus.Read;
                await _dataStore.SaveEmailAsync(email);
            }
        }

        public async Task DeleteEmailAsync(string emailId)
        {
            await _dataStore.DeleteEmailAsync(emailId);
        }

        public async Task MoveToFolderAsync(string emailId, string targetFolderId)
        {
            var email = await _dataStore.GetEmailAsync(emailId);
            if (email != null)
            {
                email.FolderID = targetFolderId;
                await _dataStore.SaveEmailAsync(email);
            }
        }

        public async Task<List<Email>> SearchAsync(string searchTerm)
        {
            return await _dataStore.QueryEmailsAsync(searchTerm);
        }

        public async Task SaveEmailAsync(Email email)
        {
            await _dataStore.SaveEmailAsync(email);
        }

        public async Task<List<Folder>> GetFoldersAsync()
        {
            return await _dataStore.GetFoldersAsync();
        }

        public async Task<Email?> GetEmailAsync(string emailId)
        {
            return await _dataStore.GetEmailAsync(emailId);
        }
    }
}
