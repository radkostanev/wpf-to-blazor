namespace OutlookInspiredApp.Blazor.Models
{
    /// <summary>
    /// Email message entity
    /// </summary>
    public class Email
    {
        public string EmailID { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Cc { get; set; } = string.Empty;
        public string Bcc { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime ReceivedDate { get; set; }
        public DateTime SentDate { get; set; }
        public EmailStatus Status { get; set; } = EmailStatus.Unread;
        public string FolderID { get; set; } = string.Empty;
        public string CategoryID { get; set; } = string.Empty;
        public List<string> Attachments { get; set; } = new List<string>();
        public bool HasAttachments { get; set; } = false;
        public FollowUpType FollowUpType { get; set; } = FollowUpType.None;
    }
}
