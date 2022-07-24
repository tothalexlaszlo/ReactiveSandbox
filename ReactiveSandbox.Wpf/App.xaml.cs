using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveSandbox.Shared.Models;
using ReactiveSandbox.Shared.Services;
using ReactiveSandbox.Shared.ViewModels;
using ReactiveSandbox.Wpf;
using System.Windows;

namespace ReativeSandbox.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        Splat.ModeDetector.OverrideModeDetector(Mode.Test);

        _host = Host.CreateDefaultBuilder()
        .ConfigureServices((hostBuilderContext, services) =>
        {
            _ = services.AddSingleton<MainWindowViewModel>()
                .AddSingleton<MainWindow>()
                .AddSingleton<GeneratorService>()
                .AddSingleton<TrackService>()
                .Configure<AppOption>(hostBuilderContext.Configuration.GetSection(nameof(AppOption)));
        })
        .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}
