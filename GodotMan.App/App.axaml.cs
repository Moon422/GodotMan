using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
