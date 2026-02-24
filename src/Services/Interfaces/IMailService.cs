using OutlookInspiredApp.Blazor.Models;

namespace OutlookInspiredApp.Blazor.Services.Interfaces
{
    /// <summary>Compose actions triggered from the Ribbon or elsewhere</summary>
    public enum MailComposeAction { None, New, Reply, ReplyAll, Forward }

    /// <summary>
    /// Mail service for orchestrating mail operations
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Load folders from repository
        /// </summary>
        Task LoadFoldersAsync();

        /// <summary>
        /// Load emails for selected folder
        /// </summary>
        Task LoadEmailsAsync(string folderId);

        /// <summary>
        /// Select an email (populates detail view)
        /// </summary>
        Task SelectEmailAsync(string emailId);

        /// <summary>
        /// Create new email (compose)
        /// </summary>
        Task<string> ComposeAsync(Email email);

        /// <summary>
        /// Search emails by text
        /// </summary>
        Task SearchAsync(string query);

        /// <summary>
        /// Reply to selected email
        /// </summary>
        Task ReplyAsync(string emailId, string body);

        /// <summary>
        /// Request a compose action (New/Reply/ReplyAll/Forward) from the Ribbon.
        /// MailPage listens for StateChanged and picks up the pending action.
        /// </summary>
        Task RequestComposeAsync(MailComposeAction action);

        /// <summary>
        /// Set category on the selected email. Pass null or empty to clear.
        /// </summary>
        Task SetCategoryAsync(string? categoryId);

        /// <summary>
        /// Set follow-up flag on the selected email.
        /// </summary>
        Task SetFollowUpAsync(FollowUpType followUp);

        /// <summary>
        /// Toggle read/unread status on the selected email.
        /// </summary>
        Task ToggleReadAsync();

        /// <summary>
        /// Get current state (folders, selected email, etc.)
        /// </summary>
        MailServiceState GetState();

        /// <summary>
        /// Fires when the mail state changes (folder selected, emails loaded, etc.)
        /// </summary>
        event Action? StateChanged;
    }

    /// <summary>
    /// State container for mail service
    /// </summary>
    public class MailServiceState
    {
        public List<Folder> Folders { get; set; } = new();
        public List<Email> Emails { get; set; } = new();
        public Email? SelectedEmail { get; set; }
        public string? SelectedFolderId { get; set; }
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }
        public MailComposeAction PendingCompose { get; set; } = MailComposeAction.None;
    }
}
