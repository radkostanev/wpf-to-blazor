using OutlookInspiredApp.Blazor.Models;

namespace OutlookInspiredApp.Blazor.Services.Interfaces
{
    /// <summary>
    /// Mail repository for email business logic
    /// </summary>
    public interface IMailRepository
    {
        /// <summary>
        /// Get emails for folder with paging
        /// </summary>
        Task<List<Email>> GetEmailsByFolderAsync(string folderId, int skip = 0, int take = 50);

        /// <summary>
        /// Get unread email count for folder
        /// </summary>
        Task<int> GetUnreadCountAsync(string folderId);

        /// <summary>
        /// Mark email as read
        /// </summary>
        Task MarkAsReadAsync(string emailId);

        /// <summary>
        /// Delete email
        /// </summary>
        Task DeleteEmailAsync(string emailId);

        /// <summary>
        /// Move email to folder
        /// </summary>
        Task MoveToFolderAsync(string emailId, string targetFolderId);

        /// <summary>
        /// Search emails by text content
        /// </summary>
        Task<List<Email>> SearchAsync(string searchTerm);

        /// <summary>
        /// Save (create or update) email
        /// </summary>
        Task SaveEmailAsync(Email email);

        /// <summary>
        /// Get all folders
        /// </summary>
        Task<List<Folder>> GetFoldersAsync();

        /// <summary>
        /// Get single email
        /// </summary>
        Task<Email?> GetEmailAsync(string emailId);
    }
}
