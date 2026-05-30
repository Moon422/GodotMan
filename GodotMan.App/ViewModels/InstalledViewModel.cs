using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using GodotMan.Domain.Enums;
using GodotMan.Services.DTOs;
using GodotMan.Services.Interfaces;
using ReactiveUI;

namespace GodotMan.App.ViewModels;

public partial class InstalledViewModel : ViewModelBase
{
    private readonly IInstallationService _installationService;
    private readonly IReleaseService _releaseService;

    private ObservableCollection<InstallationDto> _installations = [];
    public ObservableCollection<InstallationDto> Installations
    {
        get => _installations;
        set => this.RaiseAndSetIfChanged(ref _installations, value);
    }

    private InstallationDto? _selectedInstallation;
    public InstallationDto? SelectedInstallation
    {
        get => _selectedInstallation;
        set => this.RaiseAndSetIfChanged(ref _selectedInstallation, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    private bool _showOnlyInstalled;
    public bool ShowOnlyInstalled
    {
        get => _showOnlyInstalled;
        set => this.RaiseAndSetIfChanged(ref _showOnlyInstalled, value);
    }

    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<InstallationDto, Unit> LaunchCommand { get; }
    public ReactiveCommand<InstallationDto, Unit> UninstallCommand { get; }
    public ReactiveCommand<InstallationDto, Unit> SetDefaultCommand { get; }

    public InstalledViewModel()
        : this(null!, null!)
    {
        // Designer support
    }

    public InstalledViewModel(
        IReleaseService releaseService,
        IInstallationService installationService)
    {
        _releaseService = releaseService ?? throw new ArgumentNullException(nameof(releaseService));
        _installationService = installationService ?? throw new ArgumentNullException(nameof(installationService));

        RefreshCommand = ReactiveCommand.CreateFromTask(RefreshInstallations);
        LaunchCommand = ReactiveCommand.CreateFromTask<InstallationDto>(LaunchInstallation);
        UninstallCommand = ReactiveCommand.CreateFromTask<InstallationDto>(UninstallInstallation);
        SetDefaultCommand = ReactiveCommand.CreateFromTask<InstallationDto>(SetDefaultInstallation);

        // Load data on activation
        this.Activator.Activated
            .Take(1)
            .InvokeCommand(RefreshCommand);
    }

    private async System.Threading.Tasks.Task RefreshInstallations()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading installations...";

            var installations = await _installationService.GetAllAsync();

            if (ShowOnlyInstalled)
            {
                Installations = new ObservableCollection<InstallationDto>(
                    installations.Where(i => i.Status == InstallationStatus.Installed)
                );
            }
            else
            {
                Installations = new ObservableCollection<InstallationDto>(installations);
            }

            StatusMessage = $"Loaded {installations.Count} installations";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading installations: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async System.Threading.Tasks.Task LaunchInstallation(InstallationDto? installation)
    {
        if (installation == null) return;

        try
        {
            StatusMessage = $"Launching {installation.Version}...";
            // TODO: Implement launch functionality
            StatusMessage = $"Launched {installation.Version}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error launching: {ex.Message}";
        }
    }

    private async System.Threading.Tasks.Task UninstallInstallation(InstallationDto? installation)
    {
        if (installation == null) return;

        try
        {
            StatusMessage = $"Uninstalling {installation.Version}...";
            await _installationService.RemoveAsync(installation.Id);
            await RefreshInstallations();
            StatusMessage = $"Uninstalled {installation.Version}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error uninstalling: {ex.Message}";
        }
    }

    private async System.Threading.Tasks.Task SetDefaultInstallation(InstallationDto? installation)
    {
        if (installation == null) return;

        try
        {
            StatusMessage = $"Setting {installation.Version} as default...";
            // TODO: Implement set default functionality
            await RefreshInstallations();
            StatusMessage = $"Set {installation.Version} as default";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error setting default: {ex.Message}";
        }
    }

    public override string ToString() => "Installed Godot Versions";

}
