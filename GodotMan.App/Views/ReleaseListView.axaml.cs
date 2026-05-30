using GodotMan.App.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace GodotMan.App.Views;

public partial class ReleaseListView : ReactiveUserControl<ReleaseListViewModel>
{
    public ReleaseListView()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            _ = disposables;
        });
    }
}
