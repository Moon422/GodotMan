using System;
using System.IO;
using Avalonia;
using ReactiveUI.Avalonia;
using GodotMan.App.DependencyInjection;
using GodotMan.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GodotMan.App;

sealed class Program
{
    public static IHost? Host { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        Host = CreateHostBuilder(args).Build();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var appDataPath = Path.Join(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Normitech",
                    "GodotMan"
                );

                services.AddApplicationServices();
                services.AddInfrastructureServices(appDataPath);
                services.AddPresentationServices();
            });

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(_ => { });
}
