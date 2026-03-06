# Migration Guide: WPF Controls → Telerik Blazor Components

---

## Overview

This guide maps every Telerik WPF control used in the OutlookInspiredApp to its Blazor equivalent, with migration code examples where applicable.

---

## 1. Window & Layout Controls

### 1.1 RadRibbonWindow → Custom Ribbon Component

**WPF**:
```xaml
<telerik:RadRibbonWindow x:Class="TelerikOutlookInspiredApp.MainWindow"
                         Icon="Images\outlook.ico"
                         WindowState="Maximized"
                         Title="Outlook Inspired App">
    <Grid>
        <local:MainView/>
    </Grid>
</telerik:RadRibbonWindow>
```

**Blazor**:
```razor
<!-- App.razor (Root component) / or MainLayout.razor -->
<div class="ribbon-window">
    <RibbonComponent @ref="ribbon">
        <!-- Ribbon tabs and buttons go here -->
    </RibbonComponent>
    <MainView />
</div>

@code {
    private RibbonComponent ribbon;
}
```

**Custom RibbonComponent.razor**:
```razor
<div class="ribbon-container">
    <div class="ribbon-header">
        <img src="images/outlook.ico" alt="Logo" class="app-icon"/>
        <h1>Outlook Inspired App</h1>
    </div>
    <div class="ribbon-tabs">
        @foreach (var tab in Tabs)
        {
            <RibbonTabComponent Tab="@tab" IsActive="@(tab == ActiveTab)" 
                                  OnSelect="@(() => SelectTab(tab))"/>
        }
    </div>
</div>

<style>
    .ribbon-container {
        display: flex;
        flex-direction: column;
        background: #f5f5f5;
        border-bottom: 1px solid #ddd;
    }
    
    .ribbon-header {
        display: flex;
        align-items: center;
        padding: 8px 12px;
        gap: 12px;
    }
    
    .ribbon-tabs {
        display: flex;
        background: #fff;
        border-top: 1px solid #ddd;
    }
</style>

@code {
    [Parameter]
    public List<RibbonTab> Tabs { get; set; } = new();
    
    [Parameter]
    public EventCallback<RibbonTab> OnTabChanged { get; set; }
    
    private RibbonTab ActiveTab;
    
    private async Task SelectTab(RibbonTab tab)
    {
        ActiveTab = tab;
        await OnTabChanged.InvokeAsync(tab);
    }
}
```

**Cost**: Custom component required; ~4-5 days for full ribbon implementation

---

### 1.2 RadRibbonTab, RadRibbonGroup, RadRibbonButton → Custom Ribbon Components

**WPF**:
```xaml
<telerik:RadRibbonTab Header="Home">
    <telerik:RadRibbonGroup Header="New">
        <telerik:RadRibbonButton Text="New Email" 
                            LargeImage="..\Images\NewEmail.png" 
                            Size="Large" 
                            Command="{Binding NewMailCommand}"/>
    </telerik:RadRibbonGroup>
</telerik:RadRibbonTab>
```

**Blazor**:
```razor
<!-- RibbonTabComponent.razor -->
<div class="ribbon-tab @(IsActive ? "active" : "")">
    <div class="tab-header" @onclick="@(() => OnSelect.InvokeAsync())">
        @Tab.Header
    </div>
    @if (IsActive)
    {
        <div class="tab-content">
            @foreach (var group in Tab.Groups)
            {
                <RibbonGroupComponent Group="@group" />
            }
        </div>
    }
</div>

<!-- RibbonGroupComponent.razor -->
<div class="ribbon-group">
    <div class="group-header">@Group.Header</div>
    <div class="group-buttons">
        @foreach (var button in Group.Buttons)
        {
            <TelerikButton ThemeColor="@button.ThemeColor" 
                          Size="@button.Size"
                          OnClick="@(() => button.OnClick.InvokeAsync())">
                @if (!string.IsNullOrEmpty(button.ImagePath))
                {
                    <img src="@button.ImagePath" alt="@button.Text" class="button-icon"/>
                }
                <span>@button.Text</span>
            </TelerikButton>
        }
    </div>
</div>

<style>
    .ribbon-group {
        display: flex;
        flex-direction: column;
        align-items: center;
        border-right: 1px solid #ddd;
        padding: 8px 12px;
    }
    
    .group-header {
        font-size: 11px;
        color: #666;
        text-align: center;
        margin-bottom: 4px;
    }
    
    .group-buttons {
        display: flex;
        gap: 4px;
    }
</style>
```

**Cost**: Included in Ribbon component (~4-5 days total)

---

### 1.3 RadDocking → TelerikSplitter + CSS Layout

**WPF**:
```xaml
<telerik:RadDocking HasDocumentHost="False" Grid.Row="1">
    <telerik:RadSplitContainer>
        <telerik:RadPane Header="Calendar" CanUserClose="False">
            <telerik:RadOutlookBar ... />
        </telerik:RadPane>
        <telerik:RadPane Header="Appointments">
            <telerik:RadScheduleView ... />
        </telerik:RadPane>
    </telerik:RadSplitContainer>
</telerik:RadDocking>
```

**Blazor**:
```razor
<TelerikSplitter Orientation="@SplitterOrientation.Horizontal">
    <SplitterPanes>
        <SplitterPane Size="20%" Min="15%">
            <div class="leftpane">
                <OutlookBarComponent />
            </div>
        </SplitterPane>
        <SplitterPane Size="80%">
            <div class="rightpane">
                <TelerikScheduler @ref="scheduler" Data="@Appointments"
                                 AllowCreate="true" AllowDelete="true" AllowUpdate="true">
                </TelerikScheduler>
            </div>
        </SplitterPane>
    </SplitterPanes>
</TelerikSplitter>

@code {
    private TelerikScheduler scheduler;
    private List<AppointmentModel> Appointments { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        Appointments = await CalendarService.LoadAppointmentsAsync();
    }
}
```

**Cost**: ~2-3 days for splitter integration + CSS

---

## 2. Data Grid Controls

### 2.1 RadGridView → TelerikGrid

**WPF** (MailView):
```xaml
<!-- Assumed usage (not shown in initial inspection, but referenced in GridViewStyle.xaml) -->
<telerik:RadGridView ItemsSource="{Binding Emails}"
                     SelectedItem="{Binding SelectedEmail, Mode=TwoWay}"
                     RowStyleSelector="{StaticResource RowStyleSelector}">
    <telerik:GridViewDataColumn Header="From" DataMemberBinding="{Binding From}" Width="150"/>
    <telerik:GridViewDataColumn Header="Subject" DataMemberBinding="{Binding Subject}" Width="*"/>
    <telerik:GridViewDataColumn Header="Date" DataMemberBinding="{Binding DateReceived}" Width="120"/>
</telerik:RadGridView>
```

**Blazor**:
```razor
<TelerikGrid Data="@Emails"
            SelectedItems="@SelectedEmails"
            SelectionMode="GridSelectionMode.Single"
            OnRowClick="@OnEmailSelected"
            Resizable="true"
            Reorderable="true">
    <GridColumns>
        <GridColumn Field="@nameof(Email.From)" Title="From" Width="150px" />
        <GridColumn Field="@nameof(Email.Subject)" Title="Subject" Width="auto" />
        <GridColumn Field="@nameof(Email.DateReceived)" Title="Date" Width="120px" 
                   DisplayFormat="{0:g}" />
        <GridColumn Field="@nameof(Email.Status)" Title="Status" Width="80px" />
    </GridColumns>
</TelerikGrid>

@code {
    private List<Email> Emails { get; set; } = new();
    private IEnumerable<Email> SelectedEmails { get; set; } = new List<Email>();

    private async Task OnEmailSelected(GridRowClickEventArgs args)
    {
        var email = args.Item as Email;
        await MailService.SelectEmailAsync(email.EmailID);
    }

    protected override async Task OnInitializedAsync()
    {
        Emails = await MailService.GetCurrentEmailsAsync();
    }
}
```

**Key Differences**:
- **Selection**: WPF has `SelectedItem`; Blazor uses `SelectedItems` collection + `OnRowClick` event
- **Virtual Scrolling**: Built into TelerikGrid; configure via `RowHeight` and `Pageable="false"`
- **Styling**: Row styling via `RowTemplate` parameter instead of `RowStyleSelector`

**Cost**: ~2-3 days (handles virtualization automatically)

---

## 3. Navigation & Selection Controls

### 3.1 RadOutlookBar → Custom Outlook Bar Component

**WPF**:
```xaml
<telerik:RadOutlookBar x:Name="OutlookBar"
                       IsContentPreserved="True"
                       SelectedIndex="0"
                       MinimizedWidth="43"
                       Width="245"
                       ItemsSource="{Binding OutlookSections}"
                       SelectedItem="{Binding SelectedOutlookSection, Mode=TwoWay}">
    <telerik:EventToCommandBehavior.EventBindings>
        <telerik:EventBinding Command="{Binding OutlookBarSelectionChangedCommand}" 
                             EventName="SelectionChanged" />
    </telerik:EventToCommandBehavior.EventBindings>
</telerik:RadOutlookBar>
```

**Blazor** (Custom Component):
```razor
<!-- OutlookBarComponent.razor -->
<div class="outlook-bar @(IsMinimized ? "minimized" : "")">
    @if (!IsMinimized)
    {
        @foreach (var section in Sections)
        {
            <div class="outlook-section @(section == SelectedSection ? "active" : "")" 
                 @onclick="@(() => SelectSection(section))">
                <div class="section-icon">
                    <img src="@section.IconPath" alt="@section.Name"/>
                </div>
                <div class="section-label">@section.Name</div>
            </div>
        }
        <button class="minimize-button" @onclick="@(() => IsMinimized = true)">
            ◄ Minimize
        </button>
    }
    else
    {
        <div class="minimize-toggle" @onclick="@(() => IsMinimized = false)">
            ►
        </div>
    }
</div>

<style>
    .outlook-bar {
        width: 245px;
        background: #f5f5f5;
        border-right: 1px solid #ddd;
        display: flex;
        flex-direction: column;
        transition: width 0.3s ease;
    }
    
    .outlook-bar.minimized {
        width: 43px;
    }
    
    .outlook-section {
        padding: 12px;
        cursor: pointer;
        border-bottom: 1px solid #ddd;
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 8px;
    }
    
    .outlook-section.active {
        background: #0078d4;
        color: white;
    }
    
    .section-icon {
        width: 32px;
        height: 32px;
    }
    
    .section-label {
        font-size: 12px;
        text-align: center;
    }
</style>

@code {
    [Parameter]
    public List<OutlookSection> Sections { get; set; } = new();
    
    [Parameter]
    public OutlookSection SelectedSection { get; set; }
    
    [Parameter]
    public EventCallback<OutlookSection> OnSectionSelected { get; set; }
    
    private bool IsMinimized { get; set; }
    
    private async Task SelectSection(OutlookSection section)
    {
        SelectedSection = section;
        await OnSectionSelected.InvokeAsync(section);
    }
}
```

**Key Differences**:
- WPF uses data binding + commands → Blazor uses component parameters + EventCallback
- Minimize/expand animation handled via CSS transitions
- Section icons loaded from image paths (same as WPF)

**Cost**: ~2-3 days (custom component)

---

### 3.2 RadListBox → TelerikListBox

**WPF**:
```xaml
<telerik:RadListBox ItemsSource="{Binding TimeMarkers}"
                   SelectedItem="{Binding SelectedTimeMarker, Mode=TwoWay}"
                   ItemTemplate="{StaticResource TimeMarkerItemTemplate}"/>
```

**Blazor**:
```razor
<TelerikListBox Data="@TimeMarkers"
               @bind-SelectedItem="@SelectedTimeMarker"
               ListBoxHeight="300px"
               ItemTemplate="@TimeMarkerTemplate"
               TItem="TimeMarker" 
               TValue="int">
    <ListBoxItem>
        <div class="marker-item">
            <span class="marker-color" style="background: @context.AssociatedColor"></span>
            <span>@context.Name</span>
        </div>
    </ListBoxItem>
</TelerikListBox>

@code {
    private List<TimeMarker> TimeMarkers { get; set; } = new();
    private TimeMarker SelectedTimeMarker { get; set; }
}
```

**Cost**: ~1 day (direct Telerik component)

---

### 3.3 RadComboBox → TelerikDropDownList / TelerikComboBox

**WPF**:
```xaml
<telerik:RadComboBox ItemsSource="{Binding TimeMarkers}"
                    SelectedItem="{Binding SelectedTimeMarker, Mode=TwoWay}"
                    ClearSelectionButtonVisibility="Visible"
                    ItemTemplate="{StaticResource TimeMarkerComboBoxItemTemplate}"/>
```

**Blazor**:
```razor
<TelerikComboBox Data="@TimeMarkers"
                @bind-Value="@SelectedTimeMarkerID"
                Clearable="true"
                ItemTemplate="@TimeMarkerTemplate"
                PopupHeight="250px"
                TItem="TimeMarker"
                TValue="int">
    <ItemTemplate>
        <div class="combo-item">
            <span class="marker-color" style="background: @context.AssociatedColor"></span>
            <span>@context.Name</span>
        </div>
    </ItemTemplate>
</TelerikComboBox>

@code {
    private List<TimeMarker> TimeMarkers { get; set; } = new();
    private int? SelectedTimeMarkerID { get; set; }
}
```

**Cost**: ~1 day (direct Telerik component)

---

## 4. Calendar Controls

### 4.1 RadScheduleView → TelerikScheduler

**WPF**:
```xaml
<telerik:RadScheduleView x:Name="CalendarScheduleView"
                        ActiveViewDefinitionIndex="{Binding ActiveViewDefinitionIndex, Mode=TwoWay}"
                        AppointmentsSource="{Binding Appointments}"
                        ResourceTypesSource="{Binding ResourceTypes}"
                        SelectedAppointment="{Binding SelectedAppointment, Mode=TwoWay}">
    <telerik:RadScheduleView.ViewDefinitions>
        <telerik:DayViewDefinition MinTimeRulerExtent="625"/>
        <telerik:WeekViewDefinition MinTimeRulerExtent="625"/>
        <telerik:MonthViewDefinition/>
        <telerik:TimelineViewDefinition MinTimeRulerExtent="625"/>
    </telerik:RadScheduleView.ViewDefinitions>
</telerik:RadScheduleView>
```

**Blazor**:
```razor
<TelerikScheduler @ref="SchedulerRef"
                 Data="@Appointments"
                 @bind-SelectedAppointment="@SelectedAppointment"
                 @bind-CurrentView="@CurrentView"
                 Height="100%"
                 AllowCreate="true"
                 AllowDelete="true" 
                 AllowUpdate="true"
                 AllowDragDrop="true"
                 OnCreate="@OnCreateAppointment"
                 OnUpdate="@OnUpdateAppointment"
                 OnDelete="@OnDeleteAppointment"
                 OnEventDoubleClick="@OnEditAppointment">
    <SchedulerViews>
        <SchedulerDayView />
        <SchedulerWeekView />
        <SchedulerMonthView />
        <SchedulerTimelineView />
    </SchedulerViews>
</TelerikScheduler>

@code {
    private TelerikScheduler SchedulerRef { get; set; }
    private List<AppointmentModel> Appointments { get; set; } = new();
    private AppointmentModel SelectedAppointment { get; set; }
    private SchedulerView CurrentView { get; set; } = SchedulerView.Month;

    private async Task OnCreateAppointment(SchedulerCreateEventArgs args)
    {
        var newAppointment = args.Item as AppointmentModel;
        await CalendarService.CreateAppointmentAsync(newAppointment);
        Appointments.Add(newAppointment);
    }

    private async Task OnUpdateAppointment(SchedulerUpdateEventArgs args)
    {
        var updatedAppointment = args.Item as AppointmentModel;
        await CalendarService.UpdateAppointmentAsync(updatedAppointment);
    }

    private async Task OnDeleteAppointment(SchedulerDeleteEventArgs args)
    {
        var appointment = args.Item as AppointmentModel;
        await CalendarService.DeleteAppointmentAsync(appointment.AppointmentID);
        Appointments.Remove(appointment);
    }

    protected override async Task OnInitializedAsync()
    {
        Appointments = await CalendarService.LoadAppointmentsAsync();
    }
}
```

**Key Differences**:
- View definitions are components not parameters
- Appointment editing is event-driven (OnCreate, OnUpdate, OnDelete)
- No ResourceTypes visible setting in initial version; can be added in Phase 2

**Cost**: ~3-4 days (complex component with multiple views)

---

## 5. Rich Text & Editor Controls

### 5.1 RadRichTextBoxRibbonUI → TelerikEditor (or Simplified)

**WPF**:
```xaml
<telerik:RadRichTextBoxRibbonUI x:Name="richTextBoxRibbonUI"
                               CollapseThresholdSize="0 0"
                               Title="Email Composition"
                               MinimizeButtonVisibility="Visible">
    <!-- Ribbon content omitted for brevity -->
</telerik:RadRichTextBoxRibbonUI>
```

**Blazor Option 1: TelerikEditor (Full-Featured)**:
```razor
<TelerikEditor @bind-Value="@EmailBody"
              Height="400px"
              Tools="@EditorTools"
              EditMode="@EditorEditMode.Html">
</TelerikEditor>

@code {
    private string EmailBody { get; set; }

    private List<EditorToolConfig> EditorTools = new List<EditorToolConfig>()
    {
        new EditorToolConfig() { Name = EditorToolName.Bold },
        new EditorToolConfig() { Name = EditorToolName.Italic },
        new EditorToolConfig() { Name = EditorToolName.Underline },
        new EditorToolConfig() { Name = EditorToolName.FontSize },
        new EditorToolConfig() { Name = EditorToolName.FontName },
        new EditorToolConfig() { Name = EditorToolName.CreateLink },
        new EditorToolConfig() { Name = EditorToolName.Unlink },
        new EditorToolConfig() { Name = EditorToolName.InsertImage },
    };
}
```

**Blazor Option 2: Simplified TextArea (MVP)**:
```razor
<textarea @bind="@EmailBody" class="compose-textarea" rows="15"></textarea>

<style>
    .compose-textarea {
        width: 100%;
        padding: 12px;
        border: 1px solid #ddd;
        border-radius: 4px;
        font-family: Segoe UI, sans-serif;
        font-size: 14px;
    }
</style>

@code {
    private string EmailBody { get; set; }
}
```

**Cost**: 
- Option 1 (TelerikEditor): ~2 days
- Option 2 (Textarea): ~1 day

**Recommendation**: Start with Option 2 for MVP; upgrade to TelerikEditor in Phase 1.5

---

## 6. Menu & Command Controls

### 6.1 RadContextMenu → TelerikContextMenu

**WPF**:
```xaml
<telerik:RadContextMenu BorderThickness="0" IconColumnWidth="25">
    <telerik:RadMenuItem Header="Red Category" Command="{Binding OpenDialogCommand}" 
                        CommandParameter="Red Category">
        <telerik:RadMenuItem.Icon>
            <Image Source="..\Images\red_category.png" Width="16" />
        </telerik:RadMenuItem.Icon>
    </telerik:RadMenuItem>
    <!-- More menu items... -->
</telerik:RadContextMenu>
```

**Blazor**:
```razor
<TelerikContextMenu Selector=".email-row">
    <ContextMenuItems>
        <ContextMenuItem @onclick="@(() => OnCategorySelected("Red"))">
            <img src="images/red_category.png" alt="Red" width="16"/>
            <span>Red Category</span>
        </ContextMenuItem>
        <ContextMenuItem @onclick="@(() => OnCategorySelected("Blue"))">
            <img src="images/blue_category.png" alt="Blue" width="16"/>
            <span>Blue Category</span>
        </ContextMenuItem>
        <ContextMenuSeparator />
        <ContextMenuItem @onclick="@OnDeleteEmail">
            <span>Delete</span>
        </ContextMenuItem>
    </ContextMenuItems>
</TelerikContextMenu>

@code {
    private async Task OnCategorySelected(string categoryName)
    {
        // Handle category selection
    }

    private async Task OnDeleteEmail()
    {
        // Handle deletion
    }
}
```

**Cost**: ~1-2 days (direct Telerik component)

---

## 7. Other Controls

### 7.1 RadSlider → TelerikSlider

**WPF**:
```xaml
<telerik:RadSlider Minimum="625" Maximum="5000" Value="{Binding ActiveViewDefinition.MinTimeRulerExtent, Mode=TwoWay}"/>
```

**Blazor**:
```razor
<TelerikSlider @bind-Value="@MinTimeRulerExtent"
              Min="625"
              Max="5000"
              SmallStep="250"
              LargeStep="1250"
              Width="160px">
</TelerikSlider>

@code {
    private decimal MinTimeRulerExtent { get; set; } = 625;
}
```

**Cost**: ~1 day (direct Telerik component)

---

## 8. Control Migration Summary

| WPF Control | Blazor Equivalent | Effort | Status |
|---|---|---|---|
| RadRibbonWindow | Custom Ribbon Component | 4-5 days | Custom required |
| RadRibbonTab/Group/Button | Custom Ribbon Components | (included above) | Custom required |
| RadOutlookBar | Custom Outlook Bar Component | 2-3 days | Custom required |
| RadGridView | TelerikGrid | 2-3 days | ✅ Telerik component |
| RadListBox | TelerikListBox | 1 day | ✅ Telerik component |
| RadComboBox | TelerikComboBox | 1 day | ✅ Telerik component |
| RadContextMenu | TelerikContextMenu | 1-2 days | ✅ Telerik component |
| RadScheduleView | TelerikScheduler | 3-4 days | ✅ Telerik component |
| RadRichTextBoxRibbonUI | TelerikEditor or TextArea | 1-2 days | ✅ TelerikEditor available |
| RadSlider | TelerikSlider | 1 day | ✅ Telerik component |
| RadDocking | TelerikSplitter + CSS | 2-3 days | ✅ Telerik + custom CSS |

**Total Custom Component Development**: 6-8 days (Ribbon + Outlook Bar)  
**Total Telerik Component Integration**: 13-19 days (grid, scheduler, editors, sliders, menus)  
**Total Phase 1 Estimate**: 3-4 weeks

---

## 9. MVVM Pattern Migration

### 9.1 ViewModel → Service + Component State

**WPF MailViewModel**:
```csharp
public partial class MailViewModel : ViewModelBase
{
    private ObservableCollection<Email> emails;
    private Email selectedEmail;
    
    public ObservableCollection<Email> Emails
    {
        get { return emails; }
        set { if (emails != value) { emails = value; OnPropertyChanged(); } }
    }
    
    public Email SelectedEmail
    {
        get { return selectedEmail; }
        set { if (selectedEmail != value) { selectedEmail = value; OnPropertyChanged(); } }
    }
    
    public void LoadEmails()
    {
        Emails = new ObservableCollection<Email>(MailRepository.GetEmailsForFolder(folderId));
    }
}
```

**Blazor MailService**:
```csharp
public class MailService : IMailService
{
    public List<Email> Emails { get; private set; } = new();
    public Email SelectedEmail { get; private set; }
    
    public async Task LoadEmailsAsync(int folderID)
    {
        Emails = await _repository.GetEmailsByFolderAsync(folderID);
    }
    
    public async Task SelectEmailAsync(int emailID)
    {
        SelectedEmail = await _repository.GetEmailAsync(emailID);
    }
}
```

**Blazor MailComponent**:
```razor
@inject IMailService MailService
@implements IAsyncDisposable

<EmailListPanel Emails="@MailService.Emails" OnSelectEmail="@OnSelectEmail" />
<EmailDetailPanel Email="@MailService.SelectedEmail" />

@code {
    protected override async Task OnInitializedAsync()
    {
        await MailService.LoadEmailsAsync(1); // Inbox folder
    }
    
    private async Task OnSelectEmail(Email email)
    {
        await MailService.SelectEmailAsync(email.EmailID);
        StateHasChanged(); // Notify component of state change
    }
}
```

**Key Differences**:
  - No PropertyChanged events needed
  - Explicit `StateHasChanged()` when service state changes (or use component refresh pattern)
  - Services injected via `@inject` directive
  - Cascading parameters for deep component trees

---