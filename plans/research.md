# Phase 0 Research: WPF to Blazor Migration

---

## 1. Data Storage Strategy (BLOCKING DECISION)

### Question
Migrate from in-memory JSON (current WPF) to which storage backend in Blazor WASM?

### Research Findings

#### Option A: Client-Side IndexedDB (No Backend)
**Decision**: ✅ **RECOMMENDED for Phase 1**

**Rationale**:
- **Pros**:
  - No server infrastructure required; immediate value
  - Sufficient for current single-user desktop app conversion
  - IndexedDB API exposes ~50-100MB per origin (plenty for email/calendar data)
  - JS interop well-established in Blazor community
  - Async/await friendly; mirrors async patterns in C#
  
- **Cons**:
  - Client-side-only; no cross-device sync
  - Requires JavaScript interop layer
  - Data loss if user clears browser cache
  - No built-in backup/export
  
- **Implementation Approach**:
  1. Create `IStorageProvider` interface with Get/Set/Delete/Query operations
  2. Implement `IndexedDbStorageProvider` using JS interop
  3. Write `StorageService` that wraps provider (DRY from WPF InMemoryRepository)
  4. Seed IndexedDB on first load from embedded JSON files
  5. All repository methods (MailRepository, CalendarRepository) use StorageService

- **Cost**: ~3-4 days (JS interop learning curve, testing)

#### Option B: ASP.NET Core Backend API + Database
**Decision**: ⏸️ **DEFERRED to Phase 2+ (future-proof path)**

**Rationale**:
- **Pros**:
  - Cross-device sync
  - Real-time updates via SignalR
  - Centralized data; security, backup, audit logs
  - Scalable for multi-user features (later)
  
- **Cons**:
  - Requires API implementation (MailController, CalendarController)
  - Database design & migrations
  - Hosting infrastructure
  - Authentication/authorization required
  - Delays Phase 1 delivery
  
- **Note**: Architecture supports easy migration to API later; repository interfaces prepared for it

#### Option C: Hybrid (IndexedDB + Periodic Sync)
**Decision**: ⏭️ **FUTURE (Phase 2+)**

**Rationale**:
- Combines offline-first (IndexedDB) + server backup (optional backend)
- Requires conflict resolution logic
- Deferred due to complexity

### Decision ✅ CONFIRMED
**Use in-memory or JSON storage for Phase 1** (matches current WPF InMemoryRepository behavior). Load JSON data files at app startup; populate C# collections. Upgrade path to IndexedDB, API backend, or database remains available for Phase 2+ without architectural changes.

---

## 2. Rich Text Email Editor Solution (BLOCKING DESIGN DECISION)

### Question
Replace WPF RadRichTextBoxRibbonUI with what in Blazor?

### Research Findings

#### Option A: Telerik RichTextEditor for Blazor (Version 13.0.0)
**Decision**: ✅ **CONFIRMED AVAILABLE**

**Rationale**:
- **Pros**:
  - Part of Telerik suite (consistent licensing)
  - Built for Blazor; full feature parity with WPF variant
  - Integrated toolbar (customizable)
  - Handles HTML/rich content natively
  - **Use MCP**: Query `@telerikblazor TelerikEditor` for latest API details
  
- **Cons**:
  - Requires Telerik Commercial license (check current App licensing)
  - Limited documentation for Blazor (newer component)
  - Toolbar design differs from current UI (not a blocker)
  
- **Cost**: Included in existing Telerik license if enabled; dev time ~2-3 days for integration
- **Feasibility**: ✅ HIGH

#### Option B: Third-Party JS Editor (Quill.js / TinyMCE / CKEditor 5)
**Decision**: 🟡 **ACCEPTABLE alternative if Telerik not available**

**Rationale**:
- **Pros**:
  - Free/open-source options available (Quill)
  - Mature, well-documented
  - JavaScript interop from Blazor is straightforward
  - Quill: lightweight (~35KB), good for email composition
  
- **Cons**:
  - Additional JS dependency
  - License considerations (CKEditor 5 personal use free, but commercial requires license)
  - Need to wrap in Blazor component abstraction
  
- **Recommended**: Quill.js (lightweight, email-friendly, Apache 2.0 license)
- **Cost**: ~2-3 days (wrapping + Blazor component)
- **Feasibility**: ✅ HIGH

#### Option C: Simplified Phase 1 (Defer Rich Text)
**Decision**: ✅ **ACCEPTABLE for MVP**

**Rationale**:
- Implement basic `<textarea>` in Phase 1 or plain text + markdown
- Fully functional email composition without styling
- Upgrade to rich editor in Phase 2 without breaking changes
  
- **Pros**:
  - Fastest time-to-delivery
  - Easier testing
  
- **Cons**:
  - Reduced feature parity vs. WPF app
  
- **Cost**: ~1 day (basic textarea component)
- **Feasibility**: ✅✅ HIGHEST

#### Option D: Custom Blazor Rich Editor (DIY)
**Decision**: ❌ **NOT RECOMMENDED (excessive scope)**

**Rationale**:
- Too time-consuming; existing solutions are mature
- Maintenance burden

### Decision ✅ CONFIRMED
**Use TelerikEditor** (Blazor equivalent confirmed available in current Telerik licensing). Full feature parity not required; reduced feature set acceptable for MVP email composition.

**Backup Plan**: If TelerikEditor insufficient, Quill.js remains viable alternative.

---

## 3. Ribbon UI / Toolbar Implementation

### Question
RadRibbon has no Blazor equivalent. How to replicate Ribbon UX in the web?

### Research Findings

#### Option A: Custom CSS/Bootstrap Ribbon Component
**Decision**: ✅ **RECOMMENDED for Phase 1**

**Rationale**:
- **Design Approach**:
  1. Create `<Ribbon>` Razor component with CSS Grid layout
  2. Define `<RibbonTab>`, `<RibbonGroup>`, `<RibbonButton>` sub-components
  3. Style with CSS variables matching current XAML colors
  4. Use Telerik buttons inside (`TelerikButton`)
  
- **Pros**:
  - Full control over look & feel
  - Can match existing XAML design exactly
  - No external library overhead
  - Blazor-native (no JS interop needed)
  
- **Cons**:
  - Custom implementation time
  - Must handle responsive collapse (medium/small screen)
  
- **Cost**: ~3-4 days (component architecture + CSS)
- **Feasibility**: ✅ HIGH

#### Option B: Telerik Toolbar / Tab Components
**Decision**: 🟡 **ACCEPTABLE, simpler but less ribbon-like**

**Rationale**:
- Combine TelerikToolbar + TelerikTabs
- Pre-built, minimal custom CSS
- Less visually identical to WPF, but functional
  
- **Pros**:
  - Faster (1-2 days)
  - Leverages Telerik components
  
- **Cons**:
  - Does not look like traditional ribbon
  - Limited grouping/layout control
  
- **Feasibility**: ✅✅ HIGHEST

#### Option C: Third-Party Ribbon Component (JS Library)
**Decision**: ❌ **NOT RECOMMENDED (adds dependency)**

**Rationale**:
- Options: ag-Grid has context menus, but no ribbon library exists in JS ecosystem
- Too niche; custom CSS better

### Decision ✅ CONFIRMED
**Replicate WPF Ribbon UI visually where feasible** using custom CSS/Blazor component. Build `<Ribbon>`, `<RibbonTab>`, `<RibbonGroup>`, `<RibbonButton>` components with CSS Grid layout matching current XAML design.

**Approach**:
1. Extract color palette from existing XAML styles (theme colors)
2. Implement ribbon layout with CSS Grid (tab rows, button groups)
3. Nest Telerik buttons inside ribbon component scaffolding
4. Iteratively refine visual styling to match WPF screenshots
5. If full visual match not feasible, acceptable to use simpler Telerik Toolbar as fallback

### Decision ✅ CONFIRMED
**Build custom Outlook Bar** (toolbar or component). No direct Telerik Blazor equivalent exists; custom implementation necessary. Design as reusable Razor component for navigation between Mail, Calendar, People, Tasks, Notes sections.

**Approach**:
1. Create `<OutlookBar>` Razor component with vertical icon/text buttons
2. Bind to section selection logic (similar to MainViewModel pattern)
3. Style with CSS to match WPF appearance (theme colors, icon alignment)
4. Support minimized/expanded states if original app uses them

---

## 4. RadDocking Alternative (Layout Panels)

### Question
RadDocking provides resizable docking panels. What replaces this in Blazor?

### Research Findings

#### Solution: TelerikSplitter + CSS Grid / Flexbox
**Decision**: ✅ **RECOMMENDED**

**Rationale**:
- **Implementation**:
  1. Use `TelerikSplitter` for resizable boundaries between panels
  2. Wrap panels in CSS Grid or Flexbox for responsive layout
  3. Create `<DockPanel>` Razor component wrapper for consistency
  4. Store panel sizes in localStorage (persist layout)
  
- **Pros**:
  - TelerikSplitter is production-ready
  - CSS layout is responsive-friendly
  - Clean Razor component abstraction
  
- **Cons**:
  - Not a 1:1 feature match (no docking/floating windows in web context)
  - Less flexible than RadDocking
  
- **Cost**: ~2-3 days (wrapper component + localStorage integration)
- **Feasibility**: ✅ HIGH

**Note**: Web applications typically don't need floating/docking panels; split-pane layout is sufficient.

---

## 5. Keyboard Shortcuts & Accessibility (KeyTipService Replacement)

### Question
RadRibbon's KeyTipService enables Alt+Letter shortcuts. How to implement in Blazor?

### Research Findings

#### Recommended Solution: JS Interop + Configuration
**Decision**: ✅ **RECOMMENDED**

**Rationale**:
- **Implementation**:
  1. Define shortcut mappings in `shortcut-config.json` (e.g., `"Alt+N": "NewEmail"`)
  2. Register JS keyboard event listener (onkeydown)
  3. Dispatch Blazor EventCallback via JS interop
  4. Fallback: Use HTML attribute `accesskey` on buttons (simpler, limited)
  
- **Pros**:
  - Centralizes shortcut definitions
  - Supports Alt+Key, Ctrl+Key, etc.
  - Accessible via `accesskey` HTML attribute
  
- **Cons**:
  - JS interop required
  - Browser restrictions on certain key combos (browser reserved keys)
  
- **Cost**: ~2-3 days (JS interop module, config system, testing)
- **Feasibility**: ✅ HIGH

#### Alternative: HTML `accesskey` Attribute Only
**Decision**: 🟢 **ACCEPTABLE for MVP**

**Rationale**:
- Built-in HTML feature; no JS needed
- Supported by screen readers
- Limited to single keys per button (no Alt+Key customization)
  
- **Pros**:
  - Zero custom code
  - Accessibility baked in
  
- **Cons**:
  - Less powerful; can't replicate all ribbon shortcuts
  
- **Cost**: ~1 day (add `accesskey` to button components)
- **Feasibility**: ✅✅ HIGHEST

### Decision ✅
**Phase 1**: Use `accesskey` HTML attributes on common actions (New, Delete, Reply, etc.).  
**Phase 1.5+**: Implement full JS interop shortcut system if user feedback demands it.

---

## 6. Performance Optimization: Large Dataset Virtualization

### Question
Current app might hold 100k+ emails. How to ensure 60 FPS + fast load times?

### Research Findings

#### Recommended Strategy: Progressive Loading + Virtualization
**Decision**: ✅ **RECOMMENDED**

**Tactics**:

1. **TelerikGrid Virtual Scrolling**:
   - Built-in; configure ItemsPerPage = ~50
   - Blazor automatically virtualizes rendering
   - Reduces DOM size dramatically
   
2. **Repository-Level Filtering**:
   - Load only visible folder's emails, not all at once
   - Implement paging in MailRepository (PageSize = 100, LoadPage(x))
   - `` example `GetEmailsByFolder(folderID, pageIndex, pageSize)`
   
3. **Lazy-Load Components**:
   - Use Blazor `@htmRef.Lazy` for off-screen components
   - Defer calendar sidebar details until clicked
   
4. **WASM Bundle Optimization**:
   - Keep main bundle < 2MB (use lazy loading for feature-specific DLLs)
   - Consider Telerik NuGet package for WASM (optimized)
   
5. **Async Data Loading**:
   - Make all repository calls async
   - Use `IAsyncEnumerable<T>` for streaming large datasets
   
6. **Caching Strategy**:
   - Cache current folder's emails in memory (JS side via IndexedDB)
   - Implement 5-minute in-memory cache for frequent queries
   
- **Cost**: Distributed across component implementation (~3-5 days total)
- **Feasibility**: ✅ HIGH

#### Testing Strategy:
- Lighthouse audit (target: >90 performance score)
- Load test with 100k emails; profile memory usage
- Blazor profiler to ensure no unexpected re-renders

---

## 7. MVVM Pattern Adaptation (WPF → Blazor)

### Question
WPF uses ViewModelBase + ObservableCollection binding. How to adapt to Blazor?

### Research Findings

#### Recommended Pattern: Service-Based State Management
**Decision**: ✅ **RECOMMENDED**

**Approach**:

1. **Replace ViewModelBase:**
   - WPF: `class MailViewModel : ViewModelBase`
   - Blazor: `class MailService` (registered in DI)
   - Blazor components inject `MailService` via `@inject`

2. **Replace ObservableCollection:**
   - WPF: `ObservableCollection<Email>` with PropertyChanged events
   - Blazor: `List<Email>` + `EventCallback<Email>` on component methods
   - Services return `Task<List<T>>` for async data loading

3. **Example Refactoring:**
   ```csharp
   // WPF MailViewModel
   public ObservableCollection<Email> Emails { get; set; }
   public void DeleteEmail(Email email) => Emails.Remove(email);
   
   // Blazor MailService
   public List<Email> Emails { get; set; }
   public async Task DeleteEmailAsync(Email email) {
       Emails.Remove(email);
       await repository.DeleteAsync(email);
   }
   
   // Blazor Component
   @inject MailService mailService
   @foreach (var email in mailService.Emails) {
       <EmailRow Email=email OnDelete="mailService.DeleteEmailAsync" />
   }
   ```

4. **State Persistence:**
   - Use `ProtectedSessionStorage` for runtime state
   - Use custom `StorageService` (IndexedDB) for persistent data

- **Pros**:
  - Natural for Blazor; idiomatic C#
  - Easier to test (pure services)
  - Supports both server and WASM hosting
  
- **Cons**:
  - Requires refactoring ViewModels → Services
  - Loss of immediate PropertyChanged binding (must explicitly call `StateHasChanged()`)
  
- **Cost**: Embedded in component design (~2-3 days)
- **Feasibility**: ✅ HIGH

#### Alternative: Cascade State Management (Advanced)
- Use `CascadingParameter` to pass state down
- Use `EventCallback` to communicate up
- More complex but powerful for deeply nested components
- Recommended for future iterations

### Decision ✅
Adopt **Service-Based State** pattern. Treat services as the "ViewModel equivalent" in Blazor.

---

## 8. Telerik Component Licensing & Feature Parity

### Question
Does Telerik UI for Blazor 2024.x have all controls needed, and are licenses valid?

### Research Findings

#### Component Coverage

| WPF Control | Blazor Equivalent | Feature Parity | License Status |
|---|---|---|---|
| RadRibbonWindow | (Custom) | Partial | Custom component required |
| RadRibbonTab, etc. | TelerikToolbar + TelerikTabs | Good | ✅ Included |
| RadOutlookBar | TelerikOutlookBar (?) | To Verify | ⚠️ CHECK |
| RadScheduleView | TelerikScheduler | Excellent | ✅ Included |
| RadGridView | TelerikGrid | Excellent | ✅ Included |
| RadListBox | TelerikListBox | Good | ✅ Included |
| RadComboBox | TelerikDropDownList / ComboBox | Good | ✅ Included |
| RadContextMenu | TelerikContextMenu | Good | ✅ Included |
| RadRichTextBoxRibbonUI | TelerikRichTextEditor (if available) | Partial | ⚠️ CHECK |
| RadSlider | TelerikSlider | Good | ✅ Included |
| RadDocking | (TelerikSplitter) | Workaround | Custom component |

**Action Items**:
- Confirm Telerik UI for Blazor package includes `OutlookBar` control
- Confirm `RichTextEditor` available and compatible with current Telerik license
- Request license confirmation from product team
- Review Telerik Blazor documentation (early 2024 build)

**Assumption for Phase 1**: Telerik Blazor suite covers 80%+ of needed controls; gaps filled with CSS components.

---

## 9. Testing Strategy & Tools

### Question
How to test Blazor WASM app effectively?

### Research Findings

#### Recommended Approach

1. **Unit Tests (xUnit)**:
   - Test repositories (MailRepository, CalendarRepository) against mock StorageProvider
   - Test services (MailService, CalendarService) logic
   - Cost: ~2-3 days (write tests for ~20 critical methods)

2. **Component Tests (bUnit)**:
   - Test Razor components in isolation
   - Mock services via DI
   - Example: Verify EmailList renders correct count, filters work
   - Cost: ~2-3 days (cover 10-15 key components)

3. **E2E Tests (Playwright)**:
   - Test full user flows: Compose email, create appointment, switch views
   - Run against deployed WASM app
   - Cost: ~3-4 days (script 5-7 critical flows)

#### Tools:
- **Unit/Component**: xUnit + Moq + bUnit
- **E2E**: Playwright (cross-browser: Chromium, Firefox, WebKit)
- **Performance**: Lighthouse CI, Blazor WebAssembly AOT profiler

**Phase 1 Plan**: 
- Unit tests for repositories & services (required)
- Basic E2E smoke tests (required)
- Component tests deferred to Phase 1.5

---

## 10. Browser Compatibility & Web Standards

### Question
Target browsers and any compatibility gotchas?

### Research Findings

#### Target Browsers (Blazor WASM Requirements)
- **Minimum**: Browsers supporting WebAssembly (IE11 ❌ not supported)
- **Recommended**: Modern browsers with strong WASM support:
  - Edge 90+ ✅
  - Chrome 90+ ✅
  - Firefox 88+ ✅
  - Safari 14+ ✅

#### Gotchas & Considerations:
1. **WASM Size**: 32MB theoretical limit; practical apps ~5-10MB after compression
   - Monitor bundle size in CI/CD
   
2. **IndexedDB**: Supported in all modern browsers; quota varies by browser (~50-100MB per origin)
   - No issue for current app scope
   
3. **SharedArrayBuffer (Threading)**: Not available in full WASM; not needed for Phase 1
   
4. **Service Worker / PWA**: Supported; consider for offline capability (Phase 2+)

5. **CSS Grid / Flexbox**: Fully supported in target browsers; no polyfills needed

#### Assumptions:
- Application is for modern browsers only
- No IE11 or older Safari support required
- Can use cutting-edge web standards

**Decision**: No special compatibility shims needed; target modern evergreen browsers.

---

## Summary: Phase 0 Decisions - ALL CONFIRMED ✅

| Item | Decision | Status |
|------|----------|--------|
| **Data Storage** | In-memory or JSON (mirrors WPF InMemoryRepository) | ✅ CONFIRMED |
| **Rich Editor** | TelerikEditor (Telerik Blazor suite) | ✅ CONFIRMED |
| **Ribbon UI** | Custom CSS component (replicate WPF visual design) | ✅ CONFIRMED |
| **Outlook Bar** | Custom Razor component (vertical navigation) | ✅ CONFIRMED |
| **Docking Layout** | TelerikSplitter + CSS Grid layout | ✅ CONFIRMED |
| **Keyboard Shortcuts** | HTML `accesskey` attributes (MVP) | ✅ CONFIRMED |
| **Performance** | Virtual scrolling + repository-level paging | ✅ CONFIRMED |
| **MVVM Adaptation** | Service-based state management | ✅ CONFIRMED |
| **Testing** | xUnit + bUnit + Playwright | ✅ CONFIRMED |
| **Telerik License** | Active license, full suite available | ✅ CONFIRMED |
| **Auth/Users** | Single-user, no authentication required | ✅ CONFIRMED |

---