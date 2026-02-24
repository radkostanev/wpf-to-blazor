using OutlookInspiredApp.Blazor.Models;

namespace OutlookInspiredApp.Blazor.Services.Interfaces
{
    /// <summary>
    /// Data store abstraction for storage operations
    /// Supports IndexedDB (Phase 1) and future API migration (Phase 2+)
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Initialize the data store (load seed data)
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Get all emails for a specific folder
        /// </summary>
        Task<List<Email>> GetEmailsByFolderAsync(string folderId);

        /// <summary>
        /// Get email by ID
        /// </summary>
        Task<Email?> GetEmailAsync(string emailId);

        /// <summary>
        /// Save or update email
        /// </summary>
        Task SaveEmailAsync(Email email);

        /// <summary>
        /// Delete email
        /// </summary>
        Task DeleteEmailAsync(string emailId);

        /// <summary>
        /// Query emails by criteria (search)
        /// </summary>
        Task<List<Email>> QueryEmailsAsync(string searchTerm);

        /// <summary>
        /// Get all folders
        /// </summary>
        Task<List<Folder>> GetFoldersAsync();

        /// <summary>
        /// Get all appointments
        /// </summary>
        Task<List<Appointment>> GetAppointmentsAsync();
    }
}
