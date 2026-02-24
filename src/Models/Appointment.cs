namespace OutlookInspiredApp.Blazor.Models
{
    /// <summary>
    /// Calendar appointment entity
    /// </summary>
    public class Appointment
    {
        public string AppointmentID { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> ResourceIDs { get; set; } = new List<string>();
        public string CategoryID { get; set; } = string.Empty;
        public bool IsAllDay { get; set; } = false;
        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;
        public int ReminderMinutes { get; set; } = 15;
        public string Body { get; set; } = string.Empty;
        // Required by TelerikScheduler for recurring appointments
        public string? RecurrenceRule { get; set; }
    }
}
