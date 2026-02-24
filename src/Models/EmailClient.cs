namespace OutlookInspiredApp.Blazor.Models
{
    /// <summary>
    /// Email client account (potential multi-account support)
    /// </summary>
    public class EmailClient
    {
        public string ClientID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty; // Gmail, Outlook, etc.
    }
}
