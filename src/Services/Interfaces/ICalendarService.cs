using OutlookInspiredApp.Blazor.Models;

namespace OutlookInspiredApp.Blazor.Services.Interfaces
{
    /// <summary>
    /// Calendar service for orchestrating calendar operations
    /// </summary>
    public interface ICalendarService
    {
        /// <summary>
        /// Load appointments for date range
        /// </summary>
        Task LoadAppointmentsAsync(DateTime start, DateTime end);

        /// <summary>
        /// Load all resources (attendees, rooms, equipment)
        /// </summary>
        Task LoadResourcesAsync();

        /// <summary>
        /// Create new appointment
        /// </summary>
        Task<string> CreateAppointmentAsync(Appointment appointment);

        /// <summary>
        /// Update existing appointment
        /// </summary>
        Task UpdateAppointmentAsync(Appointment appointment);

        /// <summary>
        /// Delete appointment
        /// </summary>
        Task DeleteAppointmentAsync(string appointmentId);

        /// <summary>
        /// Search appointments by text
        /// </summary>
        Task SearchAsync(string query);

        /// <summary>
        /// Get current state
        /// </summary>
        CalendarServiceState GetState();

        /// <summary>
        /// Set the active calendar view (Day, Week, WorkWeek, Month, Agenda)
        /// </summary>
        Task SetViewAsync(string viewName);

        /// <summary>
        /// Navigate to today's date
        /// </summary>
        Task NavigateTodayAsync();

        /// <summary>
        /// Navigate to a specific date
        /// </summary>
        Task NavigateToDateAsync(DateTime date);

        /// <summary>
        /// State change event
        /// </summary>
        event Action? StateChanged;
    }

    /// <summary>
    /// State container for calendar service
    /// </summary>
    public class CalendarServiceState
    {
        public List<Appointment> Appointments { get; set; } = new();
        public List<Resource> Resources { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public Appointment? SelectedAppointment { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(30);
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }
        public string CurrentView { get; set; } = "Week";
        public DateTime CurrentDate { get; set; } = DateTime.Today;
    }
}
