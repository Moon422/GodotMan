using GodotMan.App.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace GodotMan.App.Views;

public partial class InstalledView : ReactiveUserControl<InstalledViewModel>
{
    public InstalledView()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            _ = disposables;
        });
    }
}
