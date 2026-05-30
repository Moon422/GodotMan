using GodotMan.App.ViewModels;
using GodotMan.App.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GodotMan.App.DependencyInjection;

/// <summary>
/// Extension methods to register presentation layer services (ViewModels and Views).
/// </summary>
public static class PresentationServiceExtensions
{
    /// <summary>
    /// Registers all ViewModels and Views with the service collection.
    /// </summary>
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        // Register ViewModels as transient - each request gets a new instance
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ReleaseListViewModel>();
        services.AddTransient<InstalledViewModel>();
        services.AddTransient<DownloadProgressViewModel>();

        // Register Views as transient
        services.AddTransient<MainWindow>();
        services.AddTransient<ReleaseListView>();
        services.AddTransient<InstalledView>();
        services.AddTransient<DownloadProgressView>();

        return services;
    }
}
