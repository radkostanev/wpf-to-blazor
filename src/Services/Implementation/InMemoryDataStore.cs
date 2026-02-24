using System.Text.Json;
using OutlookInspiredApp.Blazor.Models;
using OutlookInspiredApp.Blazor.Services.Interfaces;

namespace OutlookInspiredApp.Blazor.Services.Implementation
{
    /// <summary>
    /// In-memory data store implementation for Phase 1
    /// Loads data from JSON files and manages in memory
    /// </summary>
    public class InMemoryDataStore : IDataStore
    {
        private readonly HttpClient _httpClient;
        private List<Email> _emails = new();
        private List<Folder> _folders = new();
        private List<Appointment> _appointments = new();
        private List<Category> _categories = new();
        private List<Resource> _resources = new();
        private List<TimeMarker> _timeMarkers = new();

        public InMemoryDataStore(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Load from JSON files
                await LoadEmailsAsync();
                await LoadFoldersAsync();
                await LoadAppointmentsAsync();

                // Recompute UnreadCount from actual email data (JSON values are stale)
                RecomputeUnreadCounts();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error initializing data store: {ex.Message}");
                // Initialize with empty data if loading fails
                InitializeDefaultData();
            }
        }

        private void RecomputeUnreadCounts()
        {
            // Build a lookup: folderId -> unread count from actual emails
            var counts = _emails
                .Where(e => e.Status == EmailStatus.Unread)
                .GroupBy(e => e.FolderID)
                .ToDictionary(g => g.Key, g => g.Count());

            void ApplyCount(Folder folder)
            {
                folder.UnreadCount = counts.TryGetValue(folder.FolderID, out var c) ? c : 0;
                foreach (var sub in folder.SubFolders)
                    ApplyCount(sub);
            }

            foreach (var folder in _folders)
                ApplyCount(folder);
        }

        public async Task<List<Email>> GetEmailsByFolderAsync(string folderId)
        {
            await Task.Delay(0); // Simulate async
            return _emails.Where(e => e.FolderID == folderId).ToList();
        }

        public async Task<Email?> GetEmailAsync(string emailId)
        {
            await Task.Delay(0); // Simulate async
            return _emails.FirstOrDefault(e => e.EmailID == emailId);
        }

        public async Task SaveEmailAsync(Email email)
        {
            await Task.Delay(0); // Simulate async
            var existing = _emails.FirstOrDefault(e => e.EmailID == email.EmailID);
            if (existing != null)
            {
                _emails.Remove(existing);
            }
            _emails.Add(email);
        }

        public async Task DeleteEmailAsync(string emailId)
        {
            await Task.Delay(0); // Simulate async
            var email = _emails.FirstOrDefault(e => e.EmailID == emailId);
            if (email != null)
            {
                _emails.Remove(email);
            }
        }

        public async Task<List<Email>> QueryEmailsAsync(string searchTerm)
        {
            await Task.Delay(0); // Simulate async
            var lowerSearch = searchTerm.ToLower();
            return _emails.Where(e =>
                e.Subject.ToLower().Contains(lowerSearch) ||
                e.From.ToLower().Contains(lowerSearch) ||
                e.Body.ToLower().Contains(lowerSearch)
            ).ToList();
        }

        public async Task<List<Folder>> GetFoldersAsync()
        {
            await Task.Delay(0); // Simulate async
            return _folders;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            await Task.Delay(0); // Simulate async
            return _appointments;
        }

        private async Task LoadEmailsAsync()
        {
            try
            {
                var json = await _httpClient.GetStringAsync("data/emails.json");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _emails = JsonSerializer.Deserialize<List<Email>>(json, options) ?? new();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading emails: {ex.Message}");
            }
        }

        private async Task LoadFoldersAsync()
        {
            try
            {
                var json = await _httpClient.GetStringAsync("data/folders.json");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _folders = JsonSerializer.Deserialize<List<Folder>>(json, options) ?? new();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading folders: {ex.Message}");
            }
        }

        private async Task LoadAppointmentsAsync()
        {
            try
            {
                var json = await _httpClient.GetStringAsync("data/appointments.json");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _appointments = JsonSerializer.Deserialize<List<Appointment>>(json, options) ?? new();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading appointments: {ex.Message}");
            }
        }

        private void InitializeDefaultData()
        {
            // Create default inbox folder
            _folders = new()
            {
                new Folder
                {
                    FolderID = "inbox",
                    Name = "Inbox",
                    UnreadCount = 0
                },
                new Folder
                {
                    FolderID = "sent",
                    Name = "Sent Items",
                    UnreadCount = 0
                },
                new Folder
                {
                    FolderID = "drafts",
                    Name = "Drafts",
                    UnreadCount = 0
                }
            };
        }
    }
}
