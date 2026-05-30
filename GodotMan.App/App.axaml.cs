using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GodotMan.App.ViewModels;
using GodotMan.App.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace GodotMan.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Disable Avalonia's built-in DataAnnotations validation plugin.
        // Without this, any ViewModel that also uses CommunityToolkit or ReactiveUI
        // validation will fire twice, producing duplicate error messages.
        // Must be called before the first window is shown.
        DisableAvaloniaDataAnnotationValidation();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var serviceProvider = Program.Host?.Services;
            if (serviceProvider != null)
            {
                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                var viewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
                mainWindow.DataContext = viewModel;
                desktop.MainWindow = mainWindow;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Find and remove the DataAnnotations validator from the binding pipeline.
        // The binding pipeline runs validators in order; the built-in one is
        // DataAnnotationsValidationPlugin. Leaving it active alongside ReactiveUI
        // causes duplicate validation errors on bound properties.
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators
                          .OfType<DataAnnotationsValidationPlugin>()
                          .ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
