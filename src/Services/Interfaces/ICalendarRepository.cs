using OutlookInspiredApp.Blazor.Models;

namespace OutlookInspiredApp.Blazor.Services.Interfaces
{
    /// <summary>
    /// Calendar repository for appointment business logic
    /// </summary>
    public interface ICalendarRepository
    {
        /// <summary>
        /// Get appointments for date range
        /// </summary>
        Task<List<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end);

        /// <summary>
        /// Get appointment by ID
        /// </summary>
        Task<Appointment?> GetAppointmentAsync(string appointmentId);

        /// <summary>
        /// Save (create or update) appointment
        /// </summary>
        Task SaveAppointmentAsync(Appointment appointment);

        /// <summary>
        /// Delete appointment
        /// </summary>
        Task DeleteAppointmentAsync(string appointmentId);

        /// <summary>
        /// Get all resources (attendees, rooms, equipment)
        /// </summary>
        Task<List<Resource>> GetResourcesAsync();

        /// <summary>
        /// Get all categories
        /// </summary>
        Task<List<Category>> GetCategoriesAsync();

        /// <summary>
        /// Search appointments by text
        /// </summary>
        Task<List<Appointment>> SearchAsync(string searchTerm);

        /// <summary>
        /// Get all appointments
        /// </summary>
        Task<List<Appointment>> GetAllAppointmentsAsync();
    }
}
