using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using GodotMan.Domain.Enums;
using GodotMan.Services.Interfaces;
using ReactiveUI;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GodotMan.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IReleaseService _releaseService;
    private readonly IInstallationService _installationService;

    [ObservableProperty]
    private ObservableCollection<ReleaseListViewModel> tabs = [];

    [ObservableProperty]
    private ViewModelBase? selectedTab;

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private bool isLoading;

    public ReactiveCommand<Unit, Unit> LoadCommand { get; }
    public ReactiveCommand<ViewModelBase, Unit> SelectTabCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToReleasesCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToInstalledCommand { get; }

    public MainWindowViewModel()
        : this(null!, null!)
    {
        // Designer support
    }

    public MainWindowViewModel(
        IReleaseService releaseService,
        IInstallationService installationService)
    {
        _releaseService = releaseService ?? throw new ArgumentNullException(nameof(releaseService));
        _installationService = installationService ?? throw new ArgumentNullException(nameof(installationService));

        // Create commands
        LoadCommand = ReactiveCommand.CreateFromTask(LoadInitialData);
        SelectTabCommand = ReactiveCommand.Create<ViewModelBase>(SelectTab);
        NavigateToReleasesCommand = ReactiveCommand.Create(NavigateToReleases);
        NavigateToInstalledCommand = ReactiveCommand.Create(NavigateToInstalled);

        // Load data on activation
        this.Activator.Activated
            .Take(1)
            .InvokeCommand(LoadCommand);
    }

    private async System.Threading.Tasks.Task LoadInitialData()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading releases...";

            // Create tabs for Standard and Mono variants
            var standardTab = new ReleaseListViewModel(_releaseService, _installationService, GodotVariant.Standard);
            var monoTab = new ReleaseListViewModel(_releaseService, _installationService, GodotVariant.Mono);

            Tabs = new ObservableCollection<ReleaseListViewModel> { standardTab, monoTab };
            SelectedTab = standardTab;

            StatusMessage = "Ready";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading data: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SelectTab(ViewModelBase? tab)
    {
        if (tab != null)
        {
            SelectedTab = tab;
        }
    }

    private void NavigateToReleases()
    {
        var standardTab = Tabs.FirstOrDefault();
        if (standardTab != null)
        {
            SelectedTab = standardTab;
        }
    }

    private void NavigateToInstalled()
    {
        var installedTab = SelectedTab as InstalledViewModel;
        if (installedTab == null)
        {
            installedTab = new InstalledViewModel(_releaseService, _installationService);
            SelectedTab = installedTab;
        }
        else
        {
            SelectedTab = installedTab;
        }
    }

    public ViewModelActivator Activator { get; } = new();
}
