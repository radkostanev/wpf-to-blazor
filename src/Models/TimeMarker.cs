namespace OutlookInspiredApp.Blazor.Models
{
    /// <summary>
    /// Time marker for calendar views (task list item)
    /// </summary>
    public class TimeMarker
    {
        public string TimeMarkerID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
