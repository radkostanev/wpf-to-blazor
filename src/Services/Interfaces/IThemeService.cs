namespace OutlookInspiredApp.Blazor.Services.Interfaces;

public record ThemeOption(string Id, string Label, string Swatch);

public interface IThemeService
{
    string CurrentTheme { get; }
    event Action? StateChanged;
    IReadOnlyList<ThemeOption> AvailableThemes { get; }
    Task InitializeAsync();
    Task SetThemeAsync(string themeName);
}
