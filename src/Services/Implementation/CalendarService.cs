using OutlookInspiredApp.Blazor.Models;
using OutlookInspiredApp.Blazor.Services.Interfaces;

namespace OutlookInspiredApp.Blazor.Services.Implementation
{
    /// <summary>
    /// Calendar service implementation orchestrating calendar operations
    /// </summary>
    public class CalendarService : ICalendarService
    {
        private readonly ICalendarRepository _repository;
        private readonly CalendarServiceState _state = new();

        public CalendarService(ICalendarRepository repository)
        {
            _repository = repository;
        }

        public async Task LoadAppointmentsAsync(DateTime start, DateTime end)
        {
            _state.IsLoading = true;
            _state.StartDate = start;
            _state.EndDate = end;
            try
            {
                _state.Appointments = await _repository.GetAppointmentsByDateRangeAsync(start, end);
                _state.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                _state.ErrorMessage = $"Error loading appointments: {ex.Message}";
            }
            finally
            {
                _state.IsLoading = false;
            }
        }

        public async Task LoadResourcesAsync()
        {
            try
            {
                _state.Resources = await _repository.GetResourcesAsync();
                _state.Categories = await _repository.GetCategoriesAsync();
                _state.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                _state.ErrorMessage = $"Error loading resources: {ex.Message}";
            }
        }

        public async Task<string> CreateAppointmentAsync(Appointment appointment)
        {
            appointment.AppointmentID = Guid.NewGuid().ToString();
            await _repository.SaveAppointmentAsync(appointment);
            return appointment.AppointmentID;
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            await _repository.SaveAppointmentAsync(appointment);
        }

        public async Task DeleteAppointmentAsync(string appointmentId)
        {
            await _repository.DeleteAppointmentAsync(appointmentId);
        }

        public async Task SearchAsync(string query)
        {
            _state.IsLoading = true;
            try
            {
                _state.Appointments = await _repository.SearchAsync(query);
                _state.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                _state.ErrorMessage = $"Error searching: {ex.Message}";
            }
            finally
            {
                _state.IsLoading = false;
            }
        }

        public CalendarServiceState GetState()
        {
            return _state;
        }

        public event Action? StateChanged;

        public Task SetViewAsync(string viewName)
        {
            _state.CurrentView = viewName;
            StateChanged?.Invoke();
            return Task.CompletedTask;
        }

        public Task NavigateTodayAsync()
        {
            _state.CurrentDate = DateTime.Today;
            StateChanged?.Invoke();
            return Task.CompletedTask;
        }

        public Task NavigateToDateAsync(DateTime date)
        {
            _state.CurrentDate = date;
            StateChanged?.Invoke();
            return Task.CompletedTask;
        }
    }
}
