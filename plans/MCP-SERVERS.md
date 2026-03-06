# MCP Server Resources & Commands Reference

## Available MCP Servers

### 1. Telerik WPF Assistant
**Command Aliases**:
- `@telerik`
- `@telerikwpf`
- `/telerik`
- `/ask_telerik`

**Use For**:
- Understanding existing WPF control behavior and properties
- Reference RadGridView, RadScheduleView, RadRibbonView features
- Validating component mappings (WPF → Blazor)
- Styling and theming in WPF context

**Example Queries**:
- "What are the key properties of RadScheduleView?"
- "How does RadOutlookBar handle selection events?"
- "What styling options exist for RadGridView rows?"
- "Document RadRibbonTab structure and binding"

---

### 2. Telerik Blazor Assistant
**Command Aliases**:
- `@telerikblazor`
- `@ask_telerik` (may also trigger this)
- `/telerikblazor`
- `/help_telerik` (may also trigger this)

**Use For**:
- Blazor component API documentation
- Data binding patterns in Blazor
- Event handling (OnCreate, OnUpdate, OnDelete, etc.)
- Styling and CSS customization
- Responsive design with Telerik Blazor components

**Example Queries**:
- "How do I configure TelerikGrid for virtual scrolling?"
- "What APIs does TelerikScheduler expose for appointments?"
- "How to customize TelerikEditor toolbar?"
- "TelerikComboBox data binding example"
- "Column templating in TelerikGrid"

---

## Component Mapping Reference

### Grid Components

| WPF | Blazor | Reference | Status |
|-----|--------|-----------|--------|
| RadGridView | TelerikGrid | @telerikblazor TelerikGrid | ✅ Available |
| GridViewColumn | GridColumn | @telerikblazor GridColumn | ✅ Available |
| GridViewRow | (implicit via data) | (CSS styling) | ✅ Available |
| RadListBox | TelerikListBox | @telerikblazor TelerikListBox | ✅ Available |

**Query Examples**:
```
@telerikblazor TelerikGrid virtual scrolling configuration
@telerik RadGridView row virtualization
@telerikblazor GridColumn templates and formatting
```

---

### Calendar Components

| WPF | Blazor | Reference | Status |
|-----|--------|-----------|--------|
| RadScheduleView | TelerikScheduler | @telerikblazor TelerikScheduler | ✅ Available |
| DayViewDefinition | SchedulerDayView | @telerikblazor SchedulerDayView | ✅ Available |
| WeekViewDefinition | SchedulerWeekView | @telerikblazor SchedulerWeekView | ✅ Available |
| MonthViewDefinition | SchedulerMonthView | @telerikblazor SchedulerMonthView | ✅ Available |
| TimelineViewDefinition | SchedulerTimelineView | @telerikblazor SchedulerTimelineView | ✅ Available |

**Query Examples**:
```
@telerikblazor TelerikScheduler appointment CRUD events
@telerik RadScheduleView resource handling
@telerikblazor SchedulerView switching and configuration
```

---

### Dropdown & Selection Components

| WPF | Blazor | Reference | Status |
|-----|--------|-----------|--------|
| RadComboBox | TelerikComboBox | @telerikblazor TelerikComboBox | ✅ Available |
| RadDropDownList | TelerikDropDownList | @telerikblazor TelerikDropDownList | ✅ Available |
| RadContextMenu | TelerikContextMenu | @telerikblazor TelerikContextMenu | ✅ Available |

**Query Examples**:
```
@telerikblazor TelerikComboBox filtering and searching
@telerik RadComboBox data binding modes
@telerikblazor TelerikContextMenu positioning and visibility
```

---

### Editor Components

| WPF | Blazor | Reference | Status |
|-----|--------|-----------|--------|
| RadRichTextBoxRibbonUI | TelerikEditor | @telerikblazor TelerikEditor | ✅ Available |
| RadSlider | TelerikSlider | @telerikblazor TelerikSlider | ✅ Available |

**Query Examples**:
```
@telerikblazor TelerikEditor toolbar configuration
@telerikblazor TelerikEditor HTML content handling
@telerik RadRichTextBoxRibbonUI toolbar buttons and formatting
```

---

### Layout Components

| WPF | Blazor | Reference | Status |
|-----|--------|-----------|--------|
| RadDocking | TelerikSplitter | @telerikblazor TelerikSplitter | ✅ Available |
| RadOutlookBar | Custom Component | (Custom build required) | 🏗️ Custom |
| RadRibbonWindow | Custom Component | (Custom build required) | 🏗️ Custom |

**Query Examples**:
```
@telerikblazor TelerikSplitter resizing and layout
@telerik RadDocking pane management
@telerik RadOutlookBar section definition and styling
```

---

## Recommended Query Workflow for Phase 2

### Week 1: Grid & List Components
1. Query `@telerikblazor TelerikGrid virtual scrolling`
2. Query `@telerik RadGridView row styling and row selection`
3. Query `@telerikblazor GridColumn template examples`
4. Query `@telerikblazor TelerikListBox item template`

### Week 2: Calendar Components
1. Query `@telerikblazor TelerikScheduler complete API`
2. Query `@telerik RadScheduleView appointment properties`
3. Query `@telerikblazor SchedulerView switching`
4. Query `@telerikblazor TelerikScheduler drag and drop`

### Week 3: Dropdowns & Menus
1. Query `@telerikblazor TelerikComboBox binding and events`
2. Query `@telerik RadComboBox templates and customization`
3. Query `@telerikblazor TelerikContextMenu implementation`

### Week 4-5: Editors & Advanced
1. Query `@telerikblazor TelerikEditor toolbar tools`
2. Query `@telerik RadRichTextBoxRibbonUI ribbon buttons`
3. Query `@telerikblazor TelerikSplitter panes and resizing`

---

## Common Usage Patterns

### Data Binding
```
@telerikblazor TelerikGrid @bind-SelectedItems
@telerik RadGridView SelectedItem binding and INotifyPropertyChanged
@telerikblazor GridColumn bound data formatting
```

### Event Handling
```
@telerikblazor TelerikScheduler OnCreate delete update events
@telerik RadScheduleView appointment selection and creation events
@telerikblazor TelerikGrid OnRowClick row selection events
```

### Custom Styling
```
@telerikblazor component CSS classes and custom styling
@telerik XAML style selectors and basedOn resources
@telerikblazor theme variables and CSS variables
```

### Performance & Virtualization
```
@telerikblazor virtual scrolling performance optimization
@telerik RadGridView ScrollMode deferred loading
@telerikblazor TelerikGrid RowHeight and virtualization
```

---

## Quick Command Examples

### Get TelerikGrid Complete Documentation
```
@telerikblazor TelerikGrid complete API reference properties events parameters
```

### Compare WPF RadScheduleView with Blazor TelerikScheduler
```
@telerik RadScheduleView architecture and features
@telerikblazor TelerikScheduler appointment handling and view definitions
```

### Styling Migration (XAML → CSS)
```
@telerik style selectors for RadGridView rows and cells
@telerikblazor CSS classes and customization points
```

### Event Mapping (WPF → Blazor)
```
@telerik RadOutlookBar selection changed command
@telerikblazor equivalent Blazor event patterns and callbacks
```

---

## Notes for Next Conversation

**Context Preservation**:
- Telerik version confirmed: **13.0.0**
- Both WPF and Blazor MCP servers enabled and verified
- All spec files updated with latest version and MCP references
- Ready to build Phase 2 components with live documentation queries

**When Starting Phase 2**:
1. Bookmark this file for quick MCP server reference
2. Use `@telerikblazor` to get current API docs while building
3. Use `@telerik` to understand WPF behavior when mapping
4. Query specific components as needed during implementation

**Expected Queries per Component**:
- TelerikGrid: 3-5 queries (columns, events, virtual scrolling, templates)
- TelerikScheduler: 4-6 queries (views, events, resources, styling)
- Custom Ribbon: 2-3 queries (WPF ribbon structure, design reference)
- Custom Outlook Bar: 2-3 queries (WPF outlook bar behavior, styling)

---

## Telerik Documentation Online

For quick reference between conversations:
- **WPF**: https://docs.telerik.com/wpf/
- **Blazor**: https://docs.telerik.com/blazor-ui/

But prefer MCP servers for accuracy since they're always current with your actual packages.
