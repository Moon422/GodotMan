using System;
using System.IO;
using Avalonia;
using GodotMan.App.DependencyInjection;
using GodotMan.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI.Avalonia;

namespace GodotMan.App;

sealed class Program
{
    public static IHost? Host { get; private set; }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Create and build the DI host
        Host = CreateHostBuilder(args).Build();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Microsoft
            .Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices(
                (context, services) =>
                {
                    // Compute app data path
                    var appDataPath = Path.Join(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "Normitech",
                        "GodotMan"
                    );

                    services.AddApplicationServices();

                    // Register infrastructure services (repositories, download service, extractors)
                    services.AddInfrastructureServices(appDataPath);

                    // Register presentation services (ViewModels, Views)
                    services.AddPresentationServices();
                }
            );

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
