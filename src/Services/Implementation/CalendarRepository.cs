using OutlookInspiredApp.Blazor.Models;
using OutlookInspiredApp.Blazor.Services.Interfaces;

namespace OutlookInspiredApp.Blazor.Services.Implementation
{
    /// <summary>
    /// Calendar repository implementation providing appointment data access
    /// </summary>
    public class CalendarRepository : ICalendarRepository
    {
        private readonly IDataStore _dataStore;

        public CalendarRepository(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<List<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end)
        {
            var allAppointments = await _dataStore.GetAppointmentsAsync();
            return allAppointments
                .Where(a => (a.Start >= start && a.Start <= end) ||
                           (a.End >= start && a.End <= end) ||
                           (a.Start <= start && a.End >= end))
                .ToList();
        }

        public async Task<Appointment?> GetAppointmentAsync(string appointmentId)
        {
            var allAppointments = await _dataStore.GetAppointmentsAsync();
            return allAppointments.FirstOrDefault(a => a.AppointmentID == appointmentId);
        }

        public async Task SaveAppointmentAsync(Appointment appointment)
        {
            var allAppointments = await _dataStore.GetAppointmentsAsync();
            var existing = allAppointments.FirstOrDefault(a => a.AppointmentID == appointment.AppointmentID);
            if (existing != null)
            {
                allAppointments.Remove(existing);
            }
            allAppointments.Add(appointment);
        }

        public async Task DeleteAppointmentAsync(string appointmentId)
        {
            var allAppointments = await _dataStore.GetAppointmentsAsync();
            var appointment = allAppointments.FirstOrDefault(a => a.AppointmentID == appointmentId);
            if (appointment != null)
            {
                allAppointments.Remove(appointment);
            }
        }

        public async Task<List<Resource>> GetResourcesAsync()
        {
            // In Phase 1, resources are hard-coded
            // Future: Load from data store
            return await Task.FromResult(new List<Resource>
            {
                new Resource { ResourceID = "room1", Name = "Conference Room A", Type = "Room" },
                new Resource { ResourceID = "room2", Name = "Conference Room B", Type = "Room" }
            });
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            // In Phase 1, categories are hard-coded
            // Future: Load from data store
            return await Task.FromResult(new List<Category>
            {
                new Category { CategoryID = "cat1", Name = "Work", Color = "#0078D7" },
                new Category { CategoryID = "cat2", Name = "Personal", Color = "#107C10" }
            });
        }

        public async Task<List<Appointment>> SearchAsync(string searchTerm)
        {
            var allAppointments = await _dataStore.GetAppointmentsAsync();
            var lowerSearch = searchTerm.ToLower();
            return allAppointments
                .Where(a => a.Subject.ToLower().Contains(lowerSearch) ||
                           a.Description.ToLower().Contains(lowerSearch))
                .ToList();
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _dataStore.GetAppointmentsAsync();
        }
    }
}
