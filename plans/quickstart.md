# Quick Start: Blazor WASM Project Setup & Development Environment

---

## 1. Prerequisites

### Required
- **.NET 8.0 SDK** or later ([download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
- **Visual Studio 2022** (Community/Professional) or **Visual Studio Code** with C# Dev Kit
- **Node.js 18+** (optional, for build tools)
- **Git** for version control
- **Telerik UI for Blazor** NuGet package (requires active license)

### Verification

```bash
dotnet --version
# Expected: 8.0.x or higher

npm --version
# Expected: 9.x or higher (optional)

git --version
# Expected: 2.x or higher
```

---

## 2. Project Setup

### 2.1 Create Blazor WASM Project

```bash
# Navigate to the OutlookInspiredApp directory
cd c:\Dev\migration\OutlookInspiredApp

# Create new Blazor WebAssembly project
dotnet new blazorwasm -n OutlookInspiredApp.Blazor -au none --no-https

cd OutlookInspiredApp.Blazor
```

**Parameters Explained**:
- `-n`: Project name
- `-au none`: No authentication (single-user app)
- `--no-https`: Optional; set to false for HTTPS if deploying prod

### 2.2 Add Telerik NuGet Package

```bash
# Add Telerik UI for Blazor (version 13.0.0)
dotnet add package Telerik.UI.for.Blazor --version 13.0.0

# Note: Ensure you have Telerik NuGet source configured
# See: https://docs.telerik.com/blazor-ui/getting-started/what-you-need
```

**Configure Telerik NuGet Source** (if needed):

```bash
dotnet nuget add source "https://nuget.telerik.com/v3/index.json" \
  --name "telerik.com" \
  --username "your_telerik_email" \
  --password "your_telerik_password" \
  --store-password-in-clear-text

# Or use a .nugetconfig file for team sharing
```

### 2.3 Project File Setup (OutlookInspiredApp.Blazor.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <PublishTrimmed>true</PublishTrimmed>
    <InvariantGlobalization>false</InvariantGlobalization>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="8.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="8.0.0" PrivateAssets="all" />
    <PackageReference Include="System.Net.Http.Json" Version="8.0.0" />
    <PackageReference Include="Telerik.UI.for.Blazor" Version="13.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\OutlookInspiredApp.Repository\OutlookInspiredApp.Repository.csproj" />
  </ItemGroup>

</Project>
```

**Key Settings**:
- `PublishTrimmed="true"`: Reduces WASM bundle size (~20-30% smaller)
- `InvariantGlobalization="false"`: Enables globalization support for dates/times
- `ProjectReference`: Link to shared repository/models library

---

## 3. Directory Structure

Create the following folder structure:

```
OutlookInspiredApp.Blazor/
├── Components/                 # Razor components
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Pages/
│   │   ├── Mail/
│   │   │   ├── MailPage.razor
│   │   │   ├── EmailListPanel.razor
│   │   │   ├── EmailDetailView.razor
│   │   │   └── ComposeEmailModal.razor
│   │   ├── Calendar/
│   │   │   ├── CalendarPage.razor
│   │   │   └── AppointmentModal.razor
│   │   ├── People/
│   │   │   └── PeoplePage.razor
│   │   ├── Tasks/
│   │   │   └── TasksPage.razor
│   │   ├── Notes/
│   │   │   └── NotesPage.razor
│   │   └── Home.razor
│   └── Shared/
│       ├── OutlookBar.razor
│       ├── RibbonBar.razor
│       └── Dialogs/
│           └── ConfirmDialog.razor
├── Services/
│   ├── Interfaces/
│   │   ├── IMailService.cs
│   │   ├── ICalendarService.cs
│   │   ├── IMailRepository.cs
│   │   └── IDataStore.cs
│   ├── MailService.cs
│   ├── CalendarService.cs
│   ├── MailRepository.cs
│   ├── InMemoryDataStore.cs
│   └── AppearanceService.cs
├── Models/
│   ├── Email.cs
│   ├── Folder.cs
│   ├── Appointment.cs
│   ├── Category.cs
│   ├── TimeMarker.cs
│   └── Enums.cs
├── Styles/
│   ├── app.css
│   ├── ribbon.css
│   ├── outlook-bar.css
│   ├── mail.css
│   ├── calendar.css
│   └── theme.css
├── wwwroot/
│   ├── index.html
│   ├── css/
│   ├── images/
│   │   ├── outlook.ico
│   │   ├── categories/
│   │   ├── ribbon-icons/
│   │   └── section-icons/
│   ├── data/                   # JSON data files
│   │   ├── emails.json
│   │   ├── folders.json
│   │   ├── appointments.json
│   │   ├── categories.json
│   │   ├── resources.json
│   │   ├── time-markers.json
│   │   └── email-client.json
│   └── js/
│       └── interop.js
├── App.razor
├── Program.cs
├── appsettings.json
└── OutlookInspiredApp.Blazor.csproj
```

### Create Directories

```bash
cd OutlookInspiredApp.Blazor

mkdir -p Components/Layout
mkdir -p Components/Pages/{Mail,Calendar,People,Tasks,Notes}
mkdir -p Components/Shared/Dialogs
mkdir -p Services/Interfaces
mkdir -p Models
mkdir -p Styles
mkdir -p wwwroot/data
mkdir -p wwwroot/images/{categories,ribbon-icons,section-icons}
mkdir -p wwwroot/js
```

---

## 4. Core Configuration Files

### 4.1 Program.cs

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Telerik.Blazor.Services;
using OutlookInspiredApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add root component
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register Telerik services
builder.Services.AddTelerikBlazor();

// Register application services
builder.Services
    .AddScoped<IDataStore, InMemoryDataStore>()
    .AddScoped<IMailRepository, MailRepository>()
    .AddScoped<IMailService, MailService>()
    .AddScoped<ICalendarService, CalendarService>()
    .AddScoped<IAppearanceService, AppearanceService>();

var app = builder.Build();

// Initialize data store on startup
using (var scope = app.Services.CreateScope())
{
    var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
    await dataStore.InitializeAsync();
}

await app.RunAsync();
```

### 4.2 App.razor

```razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>Page not found</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```

### 4.3 appsettings.json

```json
{
  "ApiBaseUrl": "http://localhost:5000",
  "AppSettings": {
    "DefaultTheme": "Office2013",
    "EnableLogging": true,
    "ItemsPerPage": 50
  }
}
```

### 4.4 wwwroot/index.html

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Outlook Inspired App - Blazor</title>
    <base href="/" />
    <link rel="icon" href="images/outlook.ico" />
    <link rel="stylesheet" href="css/bootstrap.css" />
    <link rel="stylesheet" href="css/app.css" />
    <link rel="stylesheet" href="css/ribbon.css" />
    <link rel="stylesheet" href="css/outlook-bar.css" />
    <link rel="stylesheet" href="css/mail.css" />
    <link rel="stylesheet" href="css/calendar.css" />
    <link rel="stylesheet" href="css/theme.css" />
</head>
<body>
    <div id="app">Loading...</div>

    <div id="blazor-error-ui">
        An unhandled error has occurred.
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>

    <script src="_framework/blazor.web.js"></script>
    <script src="js/interop.js"></script>
</body>
</html>
```

---

## 5. JSON Data File Format

Place these files in `wwwroot/data/` directory. Copy from existing WPF app or create samples:

### 5.1 wwwroot/data/emails.json

```json
{
  "1": {
    "emailID": 1,
    "subject": "Welcome to Outlook Inspired App",
    "from": "system@example.com",
    "to": "user@example.com",
    "dateReceived": "2026-02-01T10:00:00Z",
    "body": "Welcome to the new Blazor-based Outlook App!",
    "htmlBody": "<p>Welcome to the new Blazor-based Outlook App!</p>",
    "status": 1,
    "folderID": 1,
    "isRead": true,
    "attachments": []
  }
}
```

### 5.2 wwwroot/data/folders.json

```json
[
  {"folderID": 1, "name": "Inbox", "parentFolderID": null, "unreadCount": 5},
  {"folderID": 2, "name": "Sent Items", "parentFolderID": null, "unreadCount": 0},
  {"folderID": 3, "name": "Deleted Items", "parentFolderID": null, "unreadCount": 0}
]
```

**Note**: Convert WPF JSON files to Blazor-compatible format (ensure numeric keys in emails.json are strings).

---

## 6. Build & Run

### Build Project

```bash
dotnet build
```

Expected output:
```
Build succeeded.
```

### Run in Development

```bash
dotnet watch run

# Or without watch mode:
dotnet run
```

Browser should open to `https://localhost:5001` (or HTTP localhost:5000)

### Build for Production

```bash
dotnet publish -c Release

# Output: bin/Release/net8.0/publish/wwwroot/
```

---

## 7. Debugging

### Visual Studio 2022
1. Set breakpoints in C# code
2. Press F5 to start debugging
3. Browser DevTools: F12 (inspect Razor output, debug JS interop)

### Visual Studio Code
1. Install "Debugger for C#" extension
2. Create `.vscode/launch.json`:
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Blazor WebAssembly",
            "type": "blazwasm",
            "request": "launch",
            "preLaunchTask": "build"
        }
    ]
}
```
3. Press F5 to debug

### Browser DevTools
- Open DevTools (F12)
- Application → Local Storage/Session Storage for debugging state
- Console for JS interop errors

---

## 8. Initial Minimal Component Implementation

### MainLayout.razor

```razor
@inherits LayoutComponentBase

<div class="app-container">
    <OutlookBar />
    <RibbonBar SelectedSection="@SelectedSection" />
    
    <main class="main-content">
        @Body
    </main>
</div>

@code {
    private string SelectedSection = "Mail";
}
```

### OutlookBar.razor (MVP)

```razor
<nav class="outlook-bar">
    <div class="section-item @(IsMailActive ? "active" : "")" @onclick="@(() => SelectSection("Mail"))">
        <span class="icon">✉️</span>
        <span>Mail</span>
    </div>
    <div class="section-item @(IsCalendarActive ? "active" : "")" @onclick="@(() => SelectSection("Calendar"))">
        <span class="icon">📅</span>
        <span>Calendar</span>
    </div>
    <div class="section-item @(IsPeopleActive ? "active" : "")" @onclick="@(() => SelectSection("People"))">
        <span class="icon">👥</span>
        <span>People</span>
    </div>
</nav>

@code {
    [Parameter]
    public string SelectedSection { get; set; } = "Mail";
    
    [Parameter]
    public EventCallback<string> OnSectionSelected { get; set; }
    
    private bool IsMailActive => SelectedSection == "Mail";
    private bool IsCalendarActive => SelectedSection == "Calendar";
    private bool IsPeopleActive => SelectedSection == "People";
    
    private async Task SelectSection(string section)
    {
        await OnSectionSelected.InvokeAsync(section);
    }
}
```

### MailPage.razor

```razor
@page "/mail"
@inject IMailService MailService

<div class="mail-container">
    <div class="folder-panel">
        <h3>Folders</h3>
        @if (Folders != null)
        {
            <ul>
                @foreach (var folder in Folders)
                {
                    <li @onclick="@(() => OnFolderSelected(folder.FolderID))" 
                        class="@(folder.FolderID == SelectedFolderID ? "active" : "")">
                        @folder.Name <span class="unread">(@folder.UnreadCount)</span>
                    </li>
                }
            </ul>
        }
    </div>
    
    <div class="email-panel">
        <h3>Messages</h3>
        @if (Emails != null && Emails.Count > 0)
        {
            <div class="email-list">
                @foreach (var email in Emails)
                {
                    <div class="email-item @(email.EmailID == SelectedEmailID ? "selected" : "")"
                         @onclick="@(() => OnEmailSelected(email.EmailID))">
                        <div class="sender">@email.From</div>
                        <div class="subject">@email.Subject</div>
                        <div class="date">@email.DateReceived.ToString("g")</div>
                    </div>
                }
            </div>
        }
    </div>
    
    <div class="detail-panel">
        @if (SelectedEmail != null)
        {
            <h3>@SelectedEmail.Subject</h3>
            <p><strong>From:</strong> @SelectedEmail.From</p>
            <p><strong>To:</strong> @SelectedEmail.To</p>
            <div class="body">@SelectedEmail.Body</div>
        }
        else
        {
            <p>Select an email to view</p>
        }
    </div>
</div>

@code {
    private List<Folder> Folders;
    private List<Email> Emails;
    private Email SelectedEmail;
    private int SelectedFolderID = 1;
    private int SelectedEmailID;

    protected override async Task OnInitializedAsync()
    {
        await MailService.LoadFoldersAsync();
        Folders = MailService.Folders;
        
        await OnFolderSelected(1);
    }

    private async Task OnFolderSelected(int folderID)
    {
        SelectedFolderID = folderID;
        await MailService.LoadEmailsByFolderAsync(folderID);
        Emails = MailService.CurrentEmails;
    }

    private async Task OnEmailSelected(int emailID)
    {
        SelectedEmailID = emailID;
        await MailService.SelectEmailAsync(emailID);
        SelectedEmail = MailService.SelectedEmail;
    }
}
```

---

---

**Phase 1 Total Estimate**: 3-4 weeks

---

## 9. Troubleshooting

### Telerik Components Not Found

```xml
<!-- Ensure in _Imports.razor -->
@using Telerik.Blazor
@using Telerik.Blazor.Components
```

### WASM Bundle Too Large

```bash
# Profile bundle size
dotnet publish -c Release
dotnet tool install --global dotnet-try

# Use tree-shaking and trimming
<PublishTrimmed>true</PublishTrimmed>
```

### Styling Issues

- Ensure Telerik CSS is linked in `index.html`
- Check cascade order: Telerik → App → Component-specific
- Use browser DevTools to inspect computed styles

---

## 10. Resources

- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Telerik UI for Blazor Getting Started](https://docs.telerik.com/blazor-ui/getting-started/what-you-need)
- [Telerik Blazor Sample Apps](https://github.com/telerik/blazor-ui-examples)
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/) (for future auth)
