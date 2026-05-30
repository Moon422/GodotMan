using GodotMan.App.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace GodotMan.App.Views;

public partial class DownloadProgressView : ReactiveUserControl<DownloadProgressViewModel>
{
    public DownloadProgressView()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            _ = disposables;
        });
    }
}
