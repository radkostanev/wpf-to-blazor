# Implementation Plan: Telerik WPF to Blazor WASM Migration

## Summary

Migrate the OutlookInspiredApp from Telerik WPF desktop to Blazor WebAssembly, maintaining 100% functional and visual parity. The application is a rich Outlook-inspired mail/calendar client with 5 major views, 12+ Telerik components, and MVVM patterns. In Blazor, this becomes a service-based architecture with Razor components replacing XAML, and Telerik UI for Blazor components replacing WPF controls. Phase 1 research identified IndexedDB for Phase 1 storage (cross-device sync deferred to Phase 2+), confirmed Telerik Blazor licensing, and mapped all UI components.

## Technical Context

**Language/Version**: C# 12, .NET 8.0, Blazor WebAssembly (WASM)
**Primary Dependencies**: 
  - Telerik UI for Blazor 13.0.0 (40+ components)
  - System.Text.Json (data serialization)
  - Telerik.DataSource (standard grid/scheduler patterns)

**Storage**: IndexedDB (client-side, Phase 1) → ASP.NET Core API + SQL (Phase 2+)
**Testing**: xUnit unit tests + Playwright E2E tests
**Target Platform**: Modern browsers (Edge 90+, Chrome 90+, Firefox 88+, Safari 14+)
**Project Type**: Web SPA (Single Page Application) – desktop-class mail/calendar client
**Performance Goals**: 
  - Initial load: < 3 seconds
  - Grid/Scheduler: 60 FPS with 100k+ virtual items
  - Responsive interactions: < 200ms

**Constraints**: 
  - WASM bundle size: ≤ 32MB (consider lazy loading)
  - Data integrity: Match WPF InMemoryRepository behavior exactly
  - Keyboard shortcuts: Full Ribbon shortcut support

**Scale/Scope**: 
  - 5 major views (Mail, Calendar, People, Tasks, Notes)
  - 12+ Telerik components mapped
  - 2 custom components (Ribbon, Outlook Bar)
  - ~8,000 LOC expected (component + services)
  - Single-user desktop app pattern (multi-user deferred)

## Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Status**: ✅ PASS (with documented architectural choices)

**Verified Principles** (from project requirements):
- ✅ **Architecture-First**: Service-based with clear contracts (IDataStore, IMailRepository, IMailService)
- ✅ **Component Clarity**: UI/logic separation via Telerik + custom Razor components
- ✅ **Data-Driven**: JSON source; schema preserved exactly from WPF
- ✅ **Testable Design**: Services mockable; unit test coverage planned
- ✅ **Observability**: Component state logging; IndexedDB access traceable

**Complexity Justifications** (documented in research.md):
| Design Choice | Why Needed | Alternative Rejected |
|---------------|-----------|---------------------|
| IndexedDB (client-side) | Phase 1 rapid delivery; no backend needed | ASP.NET Core API adds 2-3 weeks; deferred to Phase 2 |
| Custom Ribbon component | No Telerik equivalent; core UI requirement | Use HTML buttons: lacks professional appearance/grouping |
| Custom Outlook Bar | No Telerik equivalent; navigation critical | Hamburger menu: loses Outlook identity |
| Service-based state (not MVVM) | Blazor patterns prefer services + cascading params | ViewModel injection: over-engineered for Blazor's design |

## Project Structure

### Documentation (this feature)

```text
├── plan.md                  # This file (implementation plan)
├── spec.md                  # ✅ Feature specification (Phase 0 input)
├── research.md              # ✅ Phase 0 research (10 decisions documented)
├── data-model.md            # ✅ Phase 1 design (entities, services, interfaces)
├── migration-guide.md       # ✅ Phase 1 (WPF→Blazor mapping with code)
├── quickstart.md            # ✅ Phase 1 (setup, build, debug instructions)
├── MCP-SERVERS.md           # ✅ Phase 1 (Telerik API reference guide)
```

### Source Code (repository root)

```text
OutlookInspiredApp.Blazor/           [BEING SCAFFOLDED - Phase 2]
├── Program.cs                       # Service registration, Telerik init
├── App.razor                        # Root component, routing
├── index.html                       # WASM host, CSS/script refs
├── appsettings.json                 # Config (future API endpoint)
│
├── wwwroot/
│   ├── data/                        # JSON seed data
│   │   ├── emails.json              # Email data
│   │   ├── appointments.json        # Calendar data
│   │   └── folders.json             # Folder structure
│   ├── styles/
│   │   ├── theme.css                # Custom theme (OutlookInspired)
│   │   └── ribbon.css               # Custom Ribbon styles
│   └── index.html                   # Contains Telerik CSS/JS refs
│
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor         # Main app layout (Ribbon + Bar + Content)
│   │   ├── RibbonBar.razor          # Custom ribbon component
│   │   └── OutlookBar.razor         # Custom sidebar navigation
│   │
│   ├── Pages/
│   │   ├── MailPage.razor           # Mail view container
│   │   ├── CalendarPage.razor       # Calendar view container
│   │   ├── PeoplePage.razor         # Contacts stub
│   │   ├── TasksPage.razor          # Tasks stub
│   │   └── NotesPage.razor          # Notes stub
│   │
│   ├── Mail/
│   │   ├── EmailListPanel.razor     # TelerikGrid with emails
│   │   ├── EmailDetailView.razor    # Selected email display
│   │   └── ComposeModal.razor       # TelerikEditor compose form
│   │
│   ├── Calendar/
│   │   ├── SchedulerPanel.razor     # TelerikScheduler
│   │   └── AppointmentModal.razor   # Create/edit appointment
│   │
│   └── Shared/
│       ├── NavMenu.razor            # (not used; OutlookBar replaces)
│       └── MainLayout.razor         # (generated; overridden)
│
├── Services/
│   ├── Interfaces/
│   │   ├── IDataStore.cs            # Storage abstraction (IndexedDB→API)
│   │   ├── IMailRepository.cs       # Email business logic
│   │   ├── IMailService.cs          # Mail orchestration
│   │   ├── ICalendarRepository.cs   # Calendar data access
│   │   └── ICalendarService.cs      # Calendar orchestration
│   │
│   ├── Implementation/
│   │   ├── InMemoryDataStore.cs     # Phase 1: JSON + memory cache
│   │   ├── IndexedDbDataStore.cs    # Phase 1: JS interop wrapper
│   │   ├── MailRepository.cs        # Email data access
│   │   ├── MailService.cs           # Email orchestration
│   │   ├── CalendarRepository.cs    # Calendar data access
│   │   └── CalendarService.cs       # Calendar orchestration
│   │
│   └── Storage/
│       └── IndexedDbInterop.cs      # JS interop for IndexedDB
│
├── Models/
│   ├── Email.cs                     # Email entity
│   ├── Folder.cs                    # Folder entity
│   ├── Appointment.cs               # Calendar event entity
│   ├── Resource.cs                  # Calendar resource (attendee, room)
│   ├── Category.cs                  # Email/appointment category
│   ├── TimeMarker.cs                # Calendar time marker
│   ├── EmailStatus.cs               # Enum: Unread, Read, etc.
│   ├── FollowUpType.cs              # Enum: Follow-up types
│   └── RecurrenceType.cs            # Enum: Recurrence patterns
│
├── Styles/
│   └── app.css                      # Global styles (overrides Telerik defaults)
│
└── Properties/
    └── launchSettings.json          # Dev server config
```

**Structure Decision**: Web SPA (single-project). Blazor WASM runs client-side; no separate backend in Phase 1 (IndexedDB storage). Repository/Service layers prepared for future API migration. Custom Ribbon and Outlook Bar components handle UI that Telerik doesn't provide.

## Phase Progression

### Phase 0: Research & Unknowns Resolution ✅ COMPLETE

**Deliverables**: [research.md](research.md)  
**Timeline**: 6 hours (completed 2026-02-23)  
**Key Decisions**:

| # | Unknown | Decision | Research Details |
|---|---------|----------|------------------|
| 1 | Data storage | IndexedDB (Phase 1) → ASP.NET API (Phase 2) | [Research § Data Storage](research.md#1-data-storage-strategy-blocking-decision) |
| 2 | Telerik licensing | ✅ Confirmed available | [Research § Telerik Licensing](research.md#2-telerik-ui-for-blazor-licensing-blocking-decision) |
| 3 | Component mapping | 12+ mapped; 2 custom required | [Research § Component Mapping](research.md#3-component-mapping) |
| 4 | Rich text editor | TelerikEditor for Blazor selected | [Research § Rich Text Editor](research.md#4-rich-text-editor) |
| 5 | Ribbon UI | Custom Razor component (no Telerik equiv) | [Research § Ribbon UI](research.md#5-ribbon-ui-pattern) |
| 6 | Outlook Bar | Custom Razor component (no Telerik equiv) | [Research § Outlook Bar](research.md#6-outlook-bar-navigation) |
| 7 | Authentication | Not required (Phase 1 single-user) | [Research § Authentication](research.md#7-authentication--authorization) |
| 8 | State management | Service-based (IMailService, ICalendarService) | [Research § State Management](research.md#8-state-management-architecture) |
| 9 | Styling strategy | CSS + Telerik theming (no XAML) | [Research § Styling Strategy](research.md#9-styling-and-theming) |
| 10 | Testing framework | xUnit + Playwright E2E | [Research § Testing Framework](research.md#10-testing-framework) |

**Gate Status**: ✅ PASS – All decisions documented in research.md; user confirmed blocking items

### Phase 1: Design & Contracts ✅ COMPLETE

**Deliverables**: 
- [data-model.md](data-model.md) - Data entities, storage contracts, service interfaces
- [migration-guide.md](migration-guide.md) - WPF→Blazor mapping, code examples, effort estimates
- [quickstart.md](quickstart.md) - Project setup, build, debug instructions
- [MCP-SERVERS.md](MCP-SERVERS.md) - Telerik API reference guide
- Service interface contracts (signatures defined in data-model.md)

**Timeline**: 8 hours (completed 2026-02-23)  
**Key Artifacts**:

1. **Data Models** (8 entities preserved from WPF):
   - Email (14 properties)
   - Folder (tree structure with SubFolders)
   - Appointment (full calendar event)
   - Resource, Category, TimeMarker (supporting entities)
   - Enums: EmailStatus, FollowUpType, RecurrenceType

2. **Service Layer** (MVVM→Service-Based transition):
   - `IDataStore` (8 methods) – Storage abstraction layer
   - `IMailRepository` (9 methods) – Email business logic
   - `IMailService` (6 methods) – Mail orchestration
   - `ICalendarRepository` (8 methods) – Calendar data access
   - `ICalendarService` (7 methods) – Calendar orchestration

3. **Component Mapping** (80% Telerik coverage):
   - Telerik Grid, Scheduler, Editor, Combobox, ListBox, ContextMenu, Slider, Splitter (8 controls)
   - Custom components: RibbonBar (4-5 days), OutlookBar (2-3 days)
   - Effort estimate: 40-50 days total development

4. **Documentation**:
   - Component-by-component WPF→Blazor code examples
   - Service layer signatures with descriptions
   - JSON data format specifications
   - Build/run/debug commands

**Gate Status**: ✅ PASS (re-evaluated) – All design decisions validated; architectural conflicts resolved

**Check (Re-evaluation)**:
- ✅ Service-based architecture meets clarity & testability requirements
- ✅ Component mapping covers 80%+ of UI (2 custom components justified for Outlook identity)
- ✅ Data layer abstracted; IndexedDB implementation swappable for API later
- ✅ Testing strategy defined (unit + E2E)
- ⚠️ IndexedDB limits Phase 1 to single-user; multi-user deferred (does not violate design principles)

### Phase 2: Implementation 🚀 READY

**Timeline**: 4-6 weeks (estimated)  
**Order**: 
1. Project scaffolding + Telerik integration (1-2 days)
2. Custom components (Ribbon + Outlook Bar) (7-9 days) – **highest priority**
3. Data layer (InMemoryDataStore + JSON loading) (2-3 days)
4. Service implementations (3-4 days)
5. Mail page + components (3-4 days)
6. Calendar page + components (4-5 days)
7. Styling & theming (4-6 days)
8. Unit + E2E testing (4-6 days)
9. Performance optimization & polishing (2-3 days)

**Success Criteria**:
- [ ] Blazor WASM project builds without warnings
- [ ] Ribbon & Outlook Bar render with correct layout
- [ ] Mail page displays email list with virtual scrolling
- [ ] Email detail view loads selected email
- [ ] Compose modal works with TelerikEditor
- [ ] Calendar page displays TelerikScheduler with 5 views
- [ ] Appointments can be created, edited, deleted
- [ ] Data persisted to IndexedDB
- [ ] All UI styled to match WPF appearance
- [ ] Unit tests cover all services (>80% coverage)
- [ ] E2E tests cover critical user flows

**Blocking Items**: None; all Phase 0-1 decisions made and tested

## Complexity & Risk Tracking

| Risk | Impact | Mitigation |
|------|--------|-----------|
| **Large Dataset Handling** | Medium | Implement virtual scrolling, pagination, lazy loading early |
| **Real-time Sync** | Medium | Prototype SignalR if needed; start offline-first then add server |
| **Styling/UX Parity** | Medium | Use Telerik themes as base; create comprehensive CSS library |
| **Performance (WASM size)** | Low-Medium | Tree-shake unused Telerik components; consider split bundles |
| **Data Migration** | Low | JSON format remains compatible; create conversion utilities |
| **Testing Infrastructure** | Low | Use standard Blazor testing patterns (xUnit + Bunit) |

## Detailed Migration Timeline

### Weeks 1-2: Research & Analysis (Phase 0) ✅ COMPLETE
- ✅ Analyze Telerik UI for Blazor component parity with WPF components
- ✅ Evaluate data storage options (IndexedDB selected for Phase 1)
- ✅ Document styling migration path (XAML → CSS/Bootstrap)
- ✅ Create component mapping reference guide (12+ WPF controls to Blazor equivalents)
- ✅ Decide on state management pattern (Service-based architecture selected)

### Weeks 3-4: Foundation & Infrastructure (Phase 1) ✅ COMPLETE
- ✅ Design service architecture and dependency injection contracts
- ✅ Define data entities and storage interfaces
- ✅ Create component mapping with code examples
- ✅ Document setup and build instructions
- ✅ Prepare Telerik API reference guide

### Weeks 5-8: Core Components (Phase 2) 🚀 IN PROGRESS
- [ ] Create new Blazor WASM project structure
- [ ] Set up Telerik UI for Blazor NuGet packages and license
- [ ] Create custom components (Ribbon + Outlook Bar)
- [ ] Implement data layer (InMemoryDataStore + JSON loading)
- [ ] Build Mail module (email list, detail view, folder hierarchy)
- [ ] Build Calendar module (month/week views, appointment management)
- [ ] Build People module (contact list, detail view)
- [ ] Build Tasks module (task list, status management)
- [ ] Build Notes module (note editor, organization)

### Weeks 9-10: Advanced Features (Phase 3)
- [ ] Search functionality
- [ ] Categories/tagging system
- [ ] Flags and follow-ups
- [ ] Appearance/theming system
- [ ] Settings and preferences

### Weeks 11-12: Polish & Optimization (Phase 4)
- [ ] Performance optimization (virtualization, lazy loading)
- [ ] Cross-browser testing
- [ ] Accessibility audit (WCAG 2.1)
- [ ] E2E testing with Playwright
- [ ] Documentation and deployment setup

## Resources

- **MCP Servers** (enabled for live documentation):
  - `@telerik` – Telerik WPF reference (understand architecture)
  - `@telerikblazor` – Telerik Blazor Blazor API (implement components)

- **External References**:
  - [Telerik UI for Blazor Docs](https://docs.telerik.com/blazor-ui/introduction)
  - [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
  - [IndexedDB API](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API)

- **Related Docs** (this project):
  - [spec.md](spec.md) – Feature requirements
  - [research.md](research.md) – Decision rationale
  - [data-model.md](data-model.md) – Service contracts
  - [migration-guide.md](migration-guide.md) – Code examples
  - [quickstart.md](quickstart.md) – Setup & build
