namespace OutlookInspiredApp.Blazor.Models
{
    /// <summary>
    /// Follow-up flag type for emails
    /// </summary>
    public enum FollowUpType
    {
        None = 0,
        Today = 1,
        Tomorrow = 2,
        ThisWeek = 3,
        NextWeek = 4,
        NoDate = 5
    }
}
