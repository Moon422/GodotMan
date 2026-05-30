using GodotMan.App.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace GodotMan.App.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposables => { });
    }
}
