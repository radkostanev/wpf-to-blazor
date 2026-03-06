# Phase 1: Data Model & Storage Schema

---

## MCP Server Resources Available

**Telerik WPF** (@telerik or /telerik command):
- Reference existing WPF control properties and behaviors
- Component mapping validation

**Telerik Blazor** (@telerikblazor or /ask_telerik command):
- Blazor component APIs and configuration
- Event handling patterns
- Data binding examples
- Styling and theming options

---

## 1. Data Model Overview

The following entities preserve the WPF application's data schema without modification. All models map 1:1 from WPF domain models.

### Core Entities

#### 1.1 Email.cs
```csharp
namespace OutlookInspiredApp.Models
{
    public class Email
    {
        public int EmailID { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public DateTime DateReceived { get; set; }
        public string Body { get; set; }
        public String HtmlBody { get; set; }
        public EmailStatus Status { get; set; }
        public int FolderID { get; set; } // Foreign key to Folder
        public int? CategoryID { get; set; } // Foreign key to Category (nullable)
        public FollowUpType FollowUp { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public List<string> Attachments { get; set; } = new();
        public bool IsRead { get; set; }
    }
}
```

**Source Data**: [Data/Email.json](../../OutlookInspiredApp/OutlookInspiredApp.Client/Data/Email.json)

---

#### 1.2 Folder.cs
```csharp
namespace OutlookInspiredApp.Models
{
    public class Folder
    {
        public int FolderID { get; set; }
        public string Name { get; set; }
        public int? ParentFolderID { get; set; } // NULL for root folders
        public int UnreadCount { get; set; }
        public List<Folder> SubFolders { get; set; } = new();

        [NotMapped]
        public List<Email> Emails { get; set; } = new(); // Lazy-loaded by MailRepository
    }
}
```

**Source Data**: [Data/Folder.json](../../OutlookInspiredApp/OutlookInspiredApp.Client/Data/Folder.json)

---

#### 1.3 Appointment.cs
```csharp
namespace OutlookInspiredApp.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsAllDay { get; set; }
        public int? CategoryID { get; set; }
        public int? TimeMarkerID { get; set; }
        public List<int> ResourceIDs { get; set; } = new(); // Many-to-many: Resources attending
        public int? ReminderMinutes { get; set; } // Minutes before appointment
        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
    }

    public enum RecurrenceType
    {
        None,
        Daily,
        Weekly,
        BiWeekly,
        Monthly,
        Quarterly,
        Yearly
    }
}
```

**Source Data**: [Data/Appointment.json](../../OutlookInspiredApp/OutlookInspiredApp.Client/Data/Appointment.json)

---

#### 1.4 EmailClient.cs
```csharp
namespace OutlookInspiredApp.Models
{
    public class EmailClient
    {
        public int EmailClientID { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
```

**Source Data**: [Data/EmailClient.json](../../OutlookInspiredApp/OutlookInspiredApp.Client/Data/EmailClient.json)

---

#### 1.5 Category.cs
```csharp
namespace OutlookInspiredApp.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryBrush { get; set; } // Color hex or brush name (e.g., "#FF0000", "Red")
    }
}
```

**Source Data**: [Data/Category.json](../../OutlookInspiredApp/OutlookInspiredApp.Client/Data/Category.json)

---

#### 1.6 Resource.cs (Calendar Resources)
```csharp
namespace OutlookInspiredApp.Models
{
    public class Resource
    {
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int ResourceTypeID { get; set; }
    }
}
```

**Source Data**: [Data/Resource.json](../../OutlookInspiredApp/OutlookInspiredApp.Client/Data/Resource.json)

---

#### 1.7 TimeMarker.cs (Calendar Time Markers)
```csharp
namespace OutlookInspiredApp.Models
{
    public class TimeMarker
    {
        public int TimeMarkerID { get; set; }
        public string Name { get; set; }
        public string AssociatedColor { get; set; } // Color for visual indication
    }
}
```

**Source Data**: [Data/TimeMarker.json](../../OutlookInspiredApp/OutlookInspiredApp.Client/Data/TimeMarker.json)

---

#### 1.8 Enumerations

```csharp
namespace OutlookInspiredApp.Models
{
    public enum EmailStatus
    {
        Unread = 0,
        Read = 1,
        Replied = 2,
        Forwarded = 3,
        Flagged = 4
    }

    public enum FollowUpType
    {
        None = 0,
        Today = 1,
        Tomorrow = 2,
        ThisWeek = 3,
        NextWeek = 4,
        CustomDate = 5
    }
}
```

---

## 2. Storage Architecture

### 2.1 In-Memory Storage Pattern (Phase 1)

**Philosophy**: Mirror the current WPF app's `InMemoryRepository` pattern. Load JSON data files at app startup into C# collections.

**Implementation**:

```csharp
// Program.cs - Service Registration (Blazor WASM)
builder.Services
    .AddScoped<IDataStore, InMemoryDataStore>()
    .AddScoped<IMailRepository, MailRepository>()
    .AddScoped<ICalendarRepository, CalendarRepository>();

var app = builder.Build();

// Initialize data on startup
using (var scope = app.Services.CreateScope())
{
    var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
    await dataStore.InitializeAsync();
}

await app.RunAsync();
```

**Storage Interface**:

```csharp
namespace OutlookInspiredApp.Services
{
    /// <summary>
    /// Abstraction for data storage. Supports in-memory, JSON, IndexedDB, or API backends.
    /// </summary>
    public interface IDataStore
    {
        Task InitializeAsync(); // Load seed data
        
        // Email operations
        Task<IEnumerable<Email>> GetEmailsByFolderAsync(int folderID);
        Task<Email> GetEmailAsync(int emailID);
        Task SaveEmailAsync(Email email);
        Task DeleteEmailAsync(int emailID);
        Task<IEnumerable<Email>> QueryEmailsAsync(EmailQuery query); // Filterable query

        // Folder operations
        Task<IEnumerable<Folder>> GetFoldersAsync();
        Task<Folder> GetFolderAsync(int folderID);
        Task SaveFolderAsync(Folder folder);

        // Appointment operations
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end);
        Task<Appointment> GetAppointmentAsync(int appointmentID);
        Task SaveAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(int appointmentID);

        // Reference data
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<IEnumerable<Resource>> GetResourcesAsync();
        Task<IEnumerable<TimeMarker>> GetTimeMarkersAsync();
        Task<EmailClient> GetCurrentEmailClientAsync();
    }
}
```

**In-Memory Implementation**:

```csharp
namespace OutlookInspiredApp.Services
{
    public class InMemoryDataStore : IDataStore
    {
        private Dictionary<int, Email> _emails = new();
        private Dictionary<int, Folder> _folders = new();
        private Dictionary<int, Appointment> _appointments = new();
        private List<Category> _categories = new();
        private List<Resource> _resources = new();
        private List<TimeMarker> _timeMarkers = new();
        private EmailClient _emailClient;

        private readonly IWebAssemblyHostEnvironment _environment;
        private readonly HttpClient _httpClient;

        public InMemoryDataStore(IWebAssemblyHostEnvironment environment, HttpClient httpClient)
        {
            _environment = environment;
            _httpClient = httpClient;
        }

        public async Task InitializeAsync()
        {
            // Load JSON files from wwwroot/data/
            _emails = JsonConvert.DeserializeObject<Dictionary<int, Email>>(
                await _httpClient.GetStringAsync("data/emails.json")) ?? new();
            _folders = JsonConvert.DeserializeObject<Dictionary<int, Folder>>(
                await _httpClient.GetStringAsync("data/folders.json")) ?? new();
            _appointments = JsonConvert.DeserializeObject<Dictionary<int, Appointment>>(
                await _httpClient.GetStringAsync("data/appointments.json")) ?? new();
            _categories = JsonConvert.DeserializeObject<List<Category>>(
                await _httpClient.GetStringAsync("data/categories.json")) ?? new();
            _resources = JsonConvert.DeserializeObject<List<Resource>>(
                await _httpClient.GetStringAsync("data/resources.json")) ?? new();
            _timeMarkers = JsonConvert.DeserializeObject<List<TimeMarker>>(
                await _httpClient.GetStringAsync("data/time-markers.json")) ?? new();
            _emailClient = JsonConvert.DeserializeObject<EmailClient>(
                await _httpClient.GetStringAsync("data/email-client.json"));
        }

        // Implement IDataStore methods...
        public async Task<IEnumerable<Email>> GetEmailsByFolderAsync(int folderID)
        {
            return _emails.Values.Where(e => e.FolderID == folderID).ToList();
        }

        public async Task<Email> GetEmailAsync(int emailID)
        {
            return _emails.TryGetValue(emailID, out var email) ? email : null;
        }

        public async Task SaveEmailAsync(Email email)
        {
            _emails[email.EmailID] = email;
        }

        public async Task DeleteEmailAsync(int emailID)
        {
            _emails.Remove(emailID);
        }

        public async Task<IEnumerable<Email>> QueryEmailsAsync(EmailQuery query)
        {
            var result = _emails.Values.AsEnumerable();

            if (query.FolderID.HasValue)
                result = result.Where(e => e.FolderID == query.FolderID.Value);

            if (!string.IsNullOrEmpty(query.SearchTerm))
                result = result.Where(e => 
                    e.Subject.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.Body.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase));

            if (query.StartDate.HasValue && query.EndDate.HasValue)
                result = result.Where(e => e.DateReceived >= query.StartDate && e.DateReceived <= query.EndDate);

            return result.ToList();
        }

        // ... (similar implementations for other methods)
    }
}

public class EmailQuery
{
    public int? FolderID { get; set; }
    public string SearchTerm { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public EmailStatus? Status { get; set; }
    public int PageSize { get; set; } = 50;
    public int PageNumber { get; set; } = 1;
}
```

### 2.2 Storage Upgrade Path (Future)

**Phase 2+**: Transition to advanced storage without changing service interfaces:

**Option A: IndexedDB (Client-side)**
```csharp
public class IndexedDbDataStore : IDataStore
{
    private readonly IJsRuntime _jsRuntime;
    
    // Uses JS interop to access IndexedDB API
    // Implements all IDataStore methods via IndexedDB queries
}
```

**Option B: ASP.NET Core API Backend**
```csharp
public class ApiDataStore : IDataStore
{
    private readonly HttpClient _httpClient;
    
    // All IDataStore methods → HTTP calls to API endpoints
    // GET /api/emails?folderId=123
    // POST /api/emails
    // DELETE /api/emails/456
}
```

**Key Point**: Repository classes and services remain unchanged; only `IDataStore` implementation swaps.

---

## 3. Repository Pattern (Business Logic Layer)

Repositories abstract data access and implement domain logic.

### 3.1 IMailRepository.cs

```csharp
namespace OutlookInspiredApp.Services
{
    public interface IMailRepository
    {
        Task<IEnumerable<Email>> GetEmailsByFolderAsync(int folderID, int pageSize = 50, int pageNumber = 1);
        Task<Email> GetEmailAsync(int emailID);
        Task<List<Folder>> GetFoldersAsync();
        Task<Folder> GetFolderAsync(int folderID);
        
        Task SaveEmailAsync(Email email);
        Task DeleteEmailAsync(int emailID);
        
        Task<IEnumerable<Email>> SearchEmailsAsync(string searchTerm);
        Task<IEnumerable<Email>> FilterByDateRangeAsync(DateTime start, DateTime end);
        
        Task<int> GetUnreadCountAsync(int folderID);
        Task MarkAsReadAsync(int emailID, bool isRead);
        
        Task<Email> SaveDraftAsync(Email draft); // Compose new email
        Task<Email> ReplyAsync(int emailID, Email reply);
        Task<Email> ForwardAsync(int emailID, Email forwarded);
    }
}
```

### 3.2 MailRepository.cs Implementation

```csharp
namespace OutlookInspiredApp.Services
{
    public class MailRepository : IMailRepository
    {
        private readonly IDataStore _dataStore;
        private readonly ILogger<MailRepository> _logger;

        // In-memory cache for current folder emails (optimize frequent access)
        private int _cachedFolderID;
        private List<Email> _cachedEmails;

        public MailRepository(IDataStore dataStore, ILogger<MailRepository> logger)
        {
            _dataStore = dataStore;
            _logger = logger;
        }

        public async Task<IEnumerable<Email>> GetEmailsByFolderAsync(int folderID, int pageSize = 50, int pageNumber = 1)
        {
            // Check cache
            if (_cachedFolderID == folderID && _cachedEmails != null)
            {
                return _cachedEmails
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            // Load from data store
            _cachedEmails = (await _dataStore.GetEmailsByFolderAsync(folderID)).ToList();
            _cachedFolderID = folderID;

            return _cachedEmails
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<Email> GetEmailAsync(int emailID)
        {
            return await _dataStore.GetEmailAsync(emailID);
        }

        public async Task<List<Folder>> GetFoldersAsync()
        {
            return (await _dataStore.GetFoldersAsync()).ToList();
        }

        public async Task SaveEmailAsync(Email email)
        {
            await _dataStore.SaveEmailAsync(email);
            _cachedEmails = null; // Invalidate cache
        }

        public async Task DeleteEmailAsync(int emailID)
        {
            await _dataStore.DeleteEmailAsync(emailID);
            _cachedEmails = null; // Invalidate cache
        }

        public async Task<IEnumerable<Email>> SearchEmailsAsync(string searchTerm)
        {
            var query = new EmailQuery { SearchTerm = searchTerm };
            return await _dataStore.QueryEmailsAsync(query);
        }

        public async Task<int> GetUnreadCountAsync(int folderID)
        {
            var emails = await GetEmailsByFolderAsync(folderID, int.MaxValue, 1);
            return emails.Count(e => !e.IsRead);
        }

        public async Task MarkAsReadAsync(int emailID, bool isRead)
        {
            var email = await GetEmailAsync(emailID);
            if (email != null)
            {
                email.IsRead = isRead;
                email.Status = isRead ? EmailStatus.Read : EmailStatus.Unread;
                await SaveEmailAsync(email);
            }
        }

        // ... other methods
    }
}
```

---

## 4. Service Layer (Business Logic & Orchestration)

Services are injected into Blazor components and coordinate between repositories and UI.

### 4.1 MailService.cs

```csharp
namespace OutlookInspiredApp.Services
{
    public interface IMailService
    {
        Task LoadFoldersAsync();
        Task LoadEmailsByFolderAsync(int folderID);
        Task SelectEmailAsync(int emailID);
        Task DeleteSelectedEmailAsync();
        Task ComposeNewEmailAsync(Email draft);
        Task ReplyToEmailAsync(int emailID, string replyText);
        Task SearchEmailsAsync(string searchTerm);

        // Observable collections for Blazor components
        List<Folder> Folders { get; }
        List<Email> CurrentEmails { get; }
        Email SelectedEmail { get; }
        string SearchQuery { get; set; }
    }

    public class MailService : IMailService
    {
        private readonly IMailRepository _repository;
        private readonly ILogger<MailService> _logger;

        public List<Folder> Folders { get; private set; } = new();
        public List<Email> CurrentEmails { get; private set; } = new();
        public Email SelectedEmail { get; private set; }
        public string SearchQuery { get; set; }

        public MailService(IMailRepository repository, ILogger<MailService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task LoadFoldersAsync()
        {
            try
            {
                Folders = await _repository.GetFoldersAsync();
                _logger.LogInformation($"Loaded {Folders.Count} folders");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading folders");
            }
        }

        public async Task LoadEmailsByFolderAsync(int folderID, int pageSize = 50, int pageNumber = 1)
        {
            try
            {
                CurrentEmails = (await _repository.GetEmailsByFolderAsync(folderID, pageSize, pageNumber)).ToList();
                _logger.LogInformation($"Loaded {CurrentEmails.Count} emails from folder {folderID}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading emails");
                CurrentEmails = new();
            }
        }

        public async Task SelectEmailAsync(int emailID)
        {
            SelectedEmail = await _repository.GetEmailAsync(emailID);
            if (SelectedEmail != null)
            {
                await _repository.MarkAsReadAsync(emailID, true);
            }
        }

        public async Task DeleteSelectedEmailAsync()
        {
            if (SelectedEmail != null)
            {
                await _repository.DeleteEmailAsync(SelectedEmail.EmailID);
                SelectedEmail = null;
                CurrentEmails.Remove(SelectedEmail); // Update local list
            }
        }

        public async Task ComposeNewEmailAsync(Email draft)
        {
            draft.EmailID = Math.Max(0, CurrentEmails.Max(e => e.EmailID)) + 1;
            await _repository.SaveEmailAsync(draft);
        }

        // ... other service methods
    }
}
```

### 4.2 CalendarService.cs (Similar Pattern)

```csharp
namespace OutlookInspiredApp.Services
{
    public interface ICalendarService
    {
        Task LoadAppointmentsByDateRangeAsync(DateTime start, DateTime end);
        Task SelectAppointmentAsync(int appointmentID);
        Task CreateAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(int appointmentID);

        List<Appointment> Appointments { get; }
        Appointment SelectedAppointment { get; }
    }

    public class CalendarService : ICalendarService
    {
        private readonly ICalendarRepository _repository;
        private readonly ILogger<CalendarService> _logger;

        public List<Appointment> Appointments { get; private set; } = new();
        public Appointment SelectedAppointment { get; private set; }

        public CalendarService(ICalendarRepository repository, ILogger<CalendarService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Implementation similar to MailService...
    }
}
```

---

## 5. JSON Data File Format

All JSON files are loaded from `wwwroot/data/` at app startup.

### 5.1 emails.json

```json
{
  "1": {
    "emailID": 1,
    "subject": "Welcome to Outlook App",
    "from": "system@example.com",
    "to": "user@example.com",
    "cc": "",
    "bcc": "",
    "dateReceived": "2026-02-01T10:00:00Z",
    "body": "Welcome to the Outlook-inspired Blazor application.",
    "htmlBody": "<p>Welcome to the Outlook-inspired Blazor application.</p>",
    "status": 1,
    "folderID": 1,
    "categoryID": null,
    "followUp": 0,
    "followUpDate": null,
    "attachments": [],
    "isRead": true
  }
}
```

### 5.2 folders.json

```json
[
  {
    "folderID": 1,
    "name": "Inbox",
    "parentFolderID": null,
    "unreadCount": 5,
    "subFolders": []
  },
  {
    "folderID": 2,
    "name": "Sent Items",
    "parentFolderID": null,
    "unreadCount": 0,
    "subFolders": []
  },
  {
    "folderID": 3,
    "name": "Deleted Items",
    "parentFolderID": null,
    "unreadCount": 0,
    "subFolders": []
  }
]
```

---

## 6. Comparison: WPF vs. Blazor Architecture

| Aspect | WPF | Blazor |
|--------|-----|--------|
| **Data Loading** | `App.xaml.cs` App_Startup | `Program.cs` service initialization |
| **State Management** | `ObservableCollection<T>` with PropertyChanged | `List<T>` + service methods + component state |
| **Binding** | Two-way XAML binding | Component parameters + EventCallbacks |
| **Data Persistence** | `InMemoryRepository` (static) | `IDataStore` interface (scoped/singleton) |
| **Async Operations** | Limited (Task-based) | Full async/await with Task<T> |
| **Caching** | Manual in repositories | In-memory cache in repositories + future IndexedDB |

---