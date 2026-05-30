using GodotMan.App.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace GodotMan.App.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            // View-side reactive subscriptions go here.
            // The empty block is still required — it's what signals Avalonia
            // to participate in the ReactiveUI activation lifecycle, which in
            // turn fires ViewModel.Activator.Activated.
            _ = disposables; // suppress unused-variable warning
        });
    }
}
