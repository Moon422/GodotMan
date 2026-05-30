using System;
using System.Reactive;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;

namespace GodotMan.App.ViewModels;

public partial class DownloadProgressViewModel : ViewModelBase
{
    [ObservableProperty]
    private string assetFileName = "";

    [ObservableProperty]
    private long bytesReceived;

    [ObservableProperty]
    private long? totalBytes;

    [ObservableProperty]
    private double bytesPerSecond;

    [ObservableProperty]
    private double? fraction;

    [ObservableProperty]
    private string percentageText = "—";

    [ObservableProperty]
    private string speedText = "—";

    [ObservableProperty]
    private string etaText = "—";

    [ObservableProperty]
    private bool isComplete;

    [ObservableProperty]
    private string statusMessage = "Ready to download";

    [ObservableProperty]
    private bool canCancel;

    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public ReactiveCommand<Unit, Unit> CompleteCommand { get; }

    public DownloadProgressViewModel()
    {
        CancelCommand = ReactiveCommand.Create(CancelDownload);
        CompleteCommand = ReactiveCommand.Create(CompleteDownload);

        // Update progress display when values change
        this.WhenAnyValue(
            x => x.BytesReceived,
            x => x.TotalBytes,
            x => x.BytesPerSecond
        ).Subscribe(_ => UpdateProgressText());
    }

    private void UpdateProgressText()
    {
        // Calculate fraction
        if (TotalBytes.HasValue && TotalBytes.Value > 0)
        {
            Fraction = (double)BytesReceived / TotalBytes.Value;
            PercentageText = $"{(Fraction.Value * 100):F1}%";
        }
        else
        {
            Fraction = null;
            PercentageText = "—";
        }

        // Format speed
        if (BytesPerSecond > 0)
        {
            SpeedText = FormatBytes((long)BytesPerSecond) + "/s";
        }
        else
        {
            SpeedText = "—";
        }

        // Calculate ETA
        if (TotalBytes.HasValue && TotalBytes.Value > 0 && BytesPerSecond > 0)
        {
            var remainingBytes = TotalBytes.Value - BytesReceived;
            var secondsRemaining = remainingBytes / BytesPerSecond;
            EtaText = FormatTimeSpan(TimeSpan.FromSeconds(secondsRemaining));
        }
        else
        {
            EtaText = "—";
        }
    }

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double len = bytes;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    private string FormatTimeSpan(TimeSpan time)
    {
        if (time.TotalHours > 1)
        {
            return $"{(int)time.TotalHours}h {time.Minutes}m";
        }
        else if (time.TotalMinutes > 1)
        {
            return $"{time.Minutes}m {time.Seconds}s";
        }
        else
        {
            return $"{time.Seconds}s";
        }
    }

    private void CancelDownload()
    {
        StatusMessage = "Cancelling download...";
        CanCancel = false;
    }

    private void CompleteDownload()
    {
        IsComplete = true;
        StatusMessage = "Download completed successfully";
        CanCancel = false;
    }

    public ViewModelActivator Activator { get; } = new();
}
