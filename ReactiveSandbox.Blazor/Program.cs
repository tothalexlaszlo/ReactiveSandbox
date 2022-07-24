using DynamicData;
using MudBlazor.Services;
using ReactiveSandbox.Shared;
using ReactiveSandbox.Shared.Models;
using ReactiveSandbox.Shared.Services;
using ReactiveSandbox.Shared.ViewModels;

Splat.ModeDetector.OverrideModeDetector(Mode.Run);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

_ = builder.Services.AddSingleton<MainWindowViewModel>()
                .AddSingleton<GeneratorService>()
                .AddSingleton<TrackService>()
                .Configure<AppOption>(builder.Configuration.GetSection(nameof(AppOption)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
