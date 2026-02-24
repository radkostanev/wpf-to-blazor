using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OutlookInspiredApp.Blazor;
using OutlookInspiredApp.Blazor.Services.Interfaces;
using OutlookInspiredApp.Blazor.Services.Implementation;
using Telerik.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Telerik services
builder.Services.AddTelerikBlazor();

// Register application services
builder.Services.AddScoped<IDataStore, InMemoryDataStore>();
builder.Services.AddScoped<IMailRepository, MailRepository>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IThemeService, ThemeService>();

await builder.Build().RunAsync();
