# Feature Spec: Telerik WPF to Blazor WASM Migration

---

## Executive Summary

Migrate the OutlookInspiredApp from a Telerik WPF desktop application to a modern Blazor WebAssembly single-page application while preserving all business logic, data models, and UI patterns. The application will transition from MVVM/XAML-based UI to Razor component-based UI, leveraging Telerik UI for Blazor for component compatibility.

## Requirements

### Functional Requirements

1. **Feature Parity**: Maintain 100% functional equivalence with the existing WPF application
   - Mail view with email list, compose, and detail panels
   - Calendar view with schedule management (Day, Week, WorkWeek, Month, Timeline views)
   - People/Contacts view (stub in current app, prepare for expansion)
   - Tasks view (stub in current app, prepare for expansion)
   - Notes view (stub in current app, prepare for expansion)

2. **UI/UX Preservation**:
   - Maintain the Outlook-inspired ribbon UI paradigm
   - Preserve the outlook bar navigation pattern
   - Keep the multi-view architecture with dynamic view switching
   - Support the same theming/appearance system

3. **Data Persistence**:
   - Current: JSON files loaded into ObservableCollections via InMemoryRepository
   - Target: Migrate to browser-based storage (IndexedDB recommended) or optional backend API
   - Maintain current data schema without modification

4. **Component Mapping**: 
   - Replace WPF Telerik controls with Telerik UI for Blazor equivalents
   - Maintain custom XAML styles converted to CSS/Razor styles
   - Support responsive design for browser deployment

### Non-Functional Requirements

1. **Performance**:
   - Initial load time < 3 seconds (with HTTP/2 & compression)
   - Smooth 60 FPS UI interactions
   - Support ~100k+ email items with virtual scrolling

2. **Browser Compatibility**:
   - Modern browsers: Edge 90+, Chrome 90+, Firefox 88+, Safari 14+
   - WASM 32MB single-file size limit (consider lazy loading or module splitting)

3. **Accessibility**:
   - WCAG 2.1 Level AA compliance
   - Keyboard navigation support (Ribbon shortcuts)

## Current Application Architecture

### WPF Technology Stack
- **Framework**: .NET Framework 4.5+ / .NET Core 3.1+
- **UI Framework**: Telerik WPF Controls (2023.x)
- **Architecture Pattern**: MVVM with ViewModelBase
- **Data Layer**: Repository pattern with in-memory JSON-based storage
- **Key Dependencies**:
  - Telerik.Windows.Controls
  - Telerik.Windows.Controls.RichTextBox
  - Telerik.Windows.Controls.RichTextBoxUI
  - System.Collections.ObjectModel (for ObservableCollection)

### Current Telerik WPF Components in Use
| WPF Component | Usage | Blazor Equivalent |
|---|---|---|
| RadRibbonWindow | Main application window with ribbon UI | Custom Ribbon component |
| RadRibbonTab, RadRibbonGroup, RadRibbonButton | Ribbon UI in Mail and Calendar views | TelerikRibbonTab, TelerikRibbonGroup, TelerikButton |
| RadOutlookBar | Navigation sidebar | TelerikOutlookBar |
| RadScheduleView | Calendar appointment management | TelerikScheduler |
| DayViewDefinition, WeekViewDefinition, MonthViewDefinition, TimelineViewDefinition | Multiple calendar views | SchedulerView definitions |
| RadDocking | Docking panels for layout management | TelerikSplitter + custom layout |
| RadGridView | Data grid for email list (implied) | TelerikGrid |
| RadListBox | Category/dropdown lists | TelerikListBox |
| RadComboBox | Time marker and category selection | TelerikDropDownList / TelerikComboBox |
| RadContextMenu | Right-click menus | TelerikContextMenu |
| RadRichTextBoxRibbonUI | Rich text editor with integrated ribbon | TelerikEditor / Custom implementation |
| RadSlider | Time ruler extent control | TelerikSlider |

### Current Project Structure

```
OutlookInspiredApp/
├── OutlookInspiredApp.Client/          # WPF UI layer
│   ├── App.xaml / App.xaml.cs          # Application entry point
│   ├── MainWindow.xaml / .cs           # RadRibbonWindow wrapper
│   ├── ViewModels/                     # MVVM ViewModels
│   │   ├── MainViewModel.cs
│   │   ├── MailViewModel.cs
│   │   ├── CalendarViewModel.cs
│   │   ├── SettingsViewModel.cs
│   │   └── [Others].Commands.cs
│   ├── Views/                          # View UserControls
│   │   ├── MainView.xaml
│   │   ├── MailView.xaml (in ViewsResources/)
│   │   ├── CalendarView.xaml
│   │   └── SettingsView.xaml
│   ├── ViewsResources/                 # Shared XAML resources & templates
│   │   ├── MailView.xaml
│   │   ├── CalendarView.xaml
│   │   ├── MailViewResources.xaml
│   │   └── CalendarViewResource.xaml
│   ├── Styles/                         # XAML style dictionaries
│   │   ├── CommonStyles.xaml
│   │   ├── GridViewStyle.xaml
│   │   ├── OutlookBarStyle.xaml
│   │   ├── ScheduleViewStyle.xaml
│   │   ├── CalendarStyle.xaml
│   │   └── [Others]
│   ├── Data/                           # JSON data files
│   │   ├── Email.json
│   │   ├── Folder.json
│   │   ├── Appointment.json
│   │   └── [Others]
│   ├── Models/                         # Data models
│   │   ├── OutlookSection.cs
│   │   ├── EmailStatus.cs
│   │   └── [Others]
│   ├── Appearance/                     # Theme & appearance management
│   │   ├── AppearanceManager.cs
│   │   ├── ThemePalette.cs
│   │   └── [Others]
│   ├── Analytics/, Helpers/, Images/, etc.
│   └── Properties/ (project config)
│
├── OutlookInspiredApp.Repository/      # Data access layer
│   ├── Repositories/
│   │   ├── MailRepository.cs
│   │   ├── CalendarRepository.cs
│   │   ├── InMemoryRepository.cs      # Static data store
│   │   └── [Others]
│   ├── Models/ (shared data models)
│   ├── Service/ (DTOs and service interfaces)
│   └── Core/ (business logic)
```

### Data Models Overview
- **Email**: EmailID, Subject, From, To, Body, Attachments, Status, Category, FolderID
- **Folder**: FolderID, Name, ParentFolderID, UnreadCount
- **Appointment**: AppointmentID, Subject, Start, End, Resources, Category, TimeMarker
- **EmailClient**: EmailAddress, Name, DisplayName
- **Category**: CategoryID, CategoryName, CategoryBrush
- **Resource** (Calendar): ResourceID, ResourceName, ResourceType
- **TimeMarker** (Calendar): TimeMarkerID, Name, AssociatedColor

### MVVM Implementation Details
- **Base Class**: Telerik.Windows.Controls.ViewModelBase
- **Property Notification**: OnPropertyChanged("PropertyName") or lambda-based
- **Commands**: RelayCommand pattern (likely custom or from Telerik.Windows.Controls)
- **Data Binding**: Two-way binding with INotifyPropertyChanged

### Current Limitations & Desktop-Only Features
1. **File System Access**: JSON files loaded from local filesystem (will need web-based migration)
2. **Rich Text Editing**: RadRichTextBoxRibbonUI with embedded ribbon (requires custom Blazor implementation or third-party editor)
3. **Desktop Ribbon UI**: RadRibbon controls won't directly map to Blazor (design decision needed)
4. **Styling & Theming**: XAML-based styling needs conversion to CSS/Blazor components
5. **Email Composition**: NewEmailWindow complex XAML (may need simplification for web)
6. **Keyboard Shortcuts**: Telerik KeyTipService (XAML-specific) needs reimplementation

## Migration Scope & Scale

| Metric | Value |
|--------|-------|
| Main Views | 5 (Mail, Calendar, People, Tasks, Notes) |
| Telerik Components | ~12-15 major control types |
| ViewModels | 4 main + N utilities |
| Repositories | 3 main + shared |
| Custom Styles | ~15 XAML style dictionaries |
| Data Models | ~8-10 core entities |
| Estimated Code Lines | ~3000-5000 (UI + logic) |

## Success Criteria

1. ✅ **Functional Equivalence**: All major views and commands work identically to WPF version
2. ✅ **Data Integrity**: No data loss during migration; JSON schemas preserved
3. ✅ **Performance**: Page load < 3s, interactions at 60 FPS
4. ✅ **Browser Support**: Pass on Edge, Chrome, Firefox (modern versions)
5. ✅ **Code Quality**: Unit tests for repository & business logic; E2E tests for UI flows
6. ✅ **Documentation**: Migration guide documenting WPF↔Blazor component mapping

## Out of Scope (Future Iterations)

- Backend API (can be added later if needed)
- Advanced email features (e.g., full POP3/IMAP support)
- Advanced scheduling features (recurrence rules, complex grouping)
- Notifications and background sync (SignalR integration)
- Offline-first PWA features
- Mobile UI optimization (initially desktop-focused)
