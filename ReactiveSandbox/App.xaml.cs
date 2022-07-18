using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveSandbox.Models;
using ReactiveSandbox.Services;
using ReactiveSandbox.ViewModels;
using System.Windows;

namespace ReactiveSandbox;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
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

    private void OnApplicationStartup(object sender, StartupEventArgs e)
    {
        _host.Start();
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void OnApplicationExit(object sender, ExitEventArgs e)
    {
        _host.StopAsync().Wait();
        _host.Dispose();
    }
}
