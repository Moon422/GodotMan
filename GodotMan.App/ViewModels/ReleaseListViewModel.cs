using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GodotMan.Domain.Enums;
using GodotMan.Services.DTOs;
using GodotMan.Services.Interfaces;
using ReactiveUI;

namespace GodotMan.App.ViewModels;

public partial class ReleaseListViewModel : ViewModelBase
{
    private readonly IReleaseService _releaseService;
    private readonly IInstallationService _installationService;

    public GodotVariant Variant { get; }

    [ObservableProperty]
    private ObservableCollection<ReleaseDto> releases = [];

    [ObservableProperty]
    private ReleaseDto? selectedRelease;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private bool includePreReleases;

    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<ReleaseDto, Unit> InstallCommand { get; }

    public ReleaseListViewModel()
        : this(null!, null!, GodotVariant.Standard)
    {
        // Designer support
    }

    public ReleaseListViewModel(
        IReleaseService releaseService,
        IInstallationService installationService,
        GodotVariant variant)
    {
        _releaseService = releaseService ?? throw new ArgumentNullException(nameof(releaseService));
        _installationService = installationService ?? throw new ArgumentNullException(nameof(installationService));
        Variant = variant;

        RefreshCommand = ReactiveCommand.CreateFromTask(RefreshReleases);
        InstallCommand = ReactiveCommand.CreateFromTask<ReleaseDto>(InstallRelease);

        // Auto-refresh when pre-release setting changes
        this.WhenAnyValue(x => x.IncludePreReleases)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .InvokeCommand(RefreshCommand);

        // Load data on activation
        this.Activator.Activated
            .Take(1)
            .InvokeCommand(RefreshCommand);
    }

    private async System.Threading.Tasks.Task RefreshReleases()
    {
        try
        {
            IsLoading = true;
            StatusMessage = $"Loading {Variant} releases...";

            var releases = await _releaseService.GetReleasesAsync(Variant, IncludePreReleases);
            Releases = new ObservableCollection<ReleaseDto>(releases);

            StatusMessage = $"Loaded {releases.Count} releases";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading releases: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async System.Threading.Tasks.Task InstallRelease(ReleaseDto? release)
    {
        if (release == null) return;

        try
        {
            StatusMessage = $"Starting installation of {release.Version}...";
            // TODO: Show asset selection dialog and download progress
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    public override string ToString() => $"Godot {Variant} Releases";

    public ViewModelActivator Activator { get; } = new();
}
