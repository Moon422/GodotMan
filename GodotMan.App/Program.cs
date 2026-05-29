using System;
using System.IO;
using Avalonia;
using GodotMan.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GodotMan.App;

class Program
{
    public static IHost? Host { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        Host = CreateHostBuilder(args).Build();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Microsoft
            .Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices(
                (context, services) =>
                {
                    services.AddInfrastructureServices(
                        Path.Join(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.LocalApplicationData
                            ),
                            "Normitech",
                            "GodotMan"
                        )
                    );
                }
            );

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
