namespace OutlookInspiredApp.Blazor.Models
{
    /// <summary>
    /// Calendar resource (attendee, room, equipment, etc.)
    /// </summary>
    public class Resource
    {
        public string ResourceID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Type { get; set; } = "Person"; // Person, Room, Equipment
    }
}
