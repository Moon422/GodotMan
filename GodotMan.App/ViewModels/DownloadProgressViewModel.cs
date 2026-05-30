using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace GodotMan.App.ViewModels;

public partial class DownloadProgressViewModel : ViewModelBase
{
    private string _assetFileName = "";
    public string AssetFileName
    {
        get => _assetFileName;
        set => this.RaiseAndSetIfChanged(ref _assetFileName, value);
    }

    private long _bytesReceived;
    public long BytesReceived
    {
        get => _bytesReceived;
        set => this.RaiseAndSetIfChanged(ref _bytesReceived, value);
    }

    private long? _totalBytes;
    public long? TotalBytes
    {
        get => _totalBytes;
        set => this.RaiseAndSetIfChanged(ref _totalBytes, value);
    }

    private double _bytesPerSecond;
    public double BytesPerSecond
    {
        get => _bytesPerSecond;
        set => this.RaiseAndSetIfChanged(ref _bytesPerSecond, value);
    }

    private double? _fraction;
    public double? Fraction
    {
        get => _fraction;
        set => this.RaiseAndSetIfChanged(ref _fraction, value);
    }

    private string _percentageText = "—";
    public string PercentageText
    {
        get => _percentageText;
        set => this.RaiseAndSetIfChanged(ref _percentageText, value);
    }

    private string _speedText = "—";
    public string SpeedText
    {
        get => _speedText;
        set => this.RaiseAndSetIfChanged(ref _speedText, value);
    }

    private string _etaText = "—";
    public string EtaText
    {
        get => _etaText;
        set => this.RaiseAndSetIfChanged(ref _etaText, value);
    }

    private bool _isComplete;
    public bool IsComplete
    {
        get => _isComplete;
        set => this.RaiseAndSetIfChanged(ref _isComplete, value);
    }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    private bool _canCancel;
    public bool CanCancel
    {
        get => _canCancel;
        set => this.RaiseAndSetIfChanged(ref _canCancel, value);
    }

    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public ReactiveCommand<Unit, Unit> CompleteCommand { get; }

    public DownloadProgressViewModel()
    {
        CancelCommand = ReactiveCommand.Create(CancelDownload);
        CompleteCommand = ReactiveCommand.Create(CompleteDownload);

        // Update progress display when values change
        this.WhenAnyValue(x => x.BytesReceived, x => x.TotalBytes, x => x.BytesPerSecond)
            .Subscribe(_ => UpdateProgressText());
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
}
