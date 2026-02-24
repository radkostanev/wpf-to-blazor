using Microsoft.JSInterop;
using OutlookInspiredApp.Blazor.Services.Interfaces;

namespace OutlookInspiredApp.Blazor.Services.Implementation;

public class ThemeService : IThemeService
{
    private readonly IJSRuntime _js;
    private string _currentTheme = "kendo-theme-default";

    public string CurrentTheme => _currentTheme;

    public event Action? StateChanged;

    public IReadOnlyList<ThemeOption> AvailableThemes { get; } = new List<ThemeOption>
    {
        new("kendo-theme-default",   "Default",   "#0072C6"),
        new("kendo-theme-bootstrap", "Bootstrap", "#7952B3"),
        new("kendo-theme-material",  "Material",  "#009688"),
        new("kendo-theme-fluent",    "Fluent",    "#0078D4"),
    };

    public ThemeService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var stored = await _js.InvokeAsync<string?>("localStorage.getItem", "telerik-theme");
            if (!string.IsNullOrWhiteSpace(stored))
                _currentTheme = stored;
        }
        catch { /* ignore during pre-render or if localStorage is unavailable */ }
    }

    public async Task SetThemeAsync(string themeName)
    {
        _currentTheme = themeName;
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "telerik-theme", themeName);
        }
        catch { }
        StateChanged?.Invoke();
    }
}
