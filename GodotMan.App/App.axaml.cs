using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using GodotMan.App.ViewModels;
using GodotMan.App.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GodotMan.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Resolve MainWindow and its ViewModel from the DI container
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
}
