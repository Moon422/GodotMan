using System;

namespace GodotMan.Domain.Entities;

/// <summary>
/// Immutable value object that captures the current state of an in-progress
/// file download. Passed through <see cref="IProgress{T}"/> callbacks so the
/// UI layer can update without depending on infrastructure types.
/// </summary>
public sealed class DownloadProgress
{
    /// <summary>Number of bytes received so far.</summary>
    public required long BytesReceived { get; init; }

    /// <summary>
    /// Total expected bytes, or null if the server did not provide
    /// a Content-Length header.
    /// </summary>
    public required long? TotalBytes { get; init; }

    /// <summary>
    /// Download completion as a value between 0.0 and 1.0,
    /// or null when total size is unknown.
    /// </summary>
    public double? Fraction =>
        TotalBytes is > 0
            ? Math.Clamp((double)BytesReceived / TotalBytes.Value, 0, 1)
            : null;

    /// <summary>
    /// Percentage string for display, e.g. "42 %" or "—" when unknown.
    /// </summary>
    public string PercentageText =>
        Fraction is { } f ? $"{f * 100:F0} %" : "—";

    /// <summary>Current download speed in bytes per second.</summary>
    public required double BytesPerSecond { get; init; }

    /// <summary>Human-readable speed, e.g. "4.2 MB/s".</summary>
    public string SpeedText =>
        BytesPerSecond switch
        {
            >= 1_048_576 => $"{BytesPerSecond / 1_048_576:F1} MB/s",
            >= 1_024 => $"{BytesPerSecond / 1_024:F1} KB/s",
            _ => $"{BytesPerSecond:F0} B/s"
        };

    /// <summary>Estimated time remaining, or null when speed/total is unknown.</summary>
    public TimeSpan? EstimatedTimeRemaining =>
        TotalBytes is { } total && BytesPerSecond > 0
            ? TimeSpan.FromSeconds((total - BytesReceived) / BytesPerSecond)
            : null;

    /// <summary>Friendly ETA string, e.g. "1m 24s" or "—".</summary>
    public string EtaText =>
        EstimatedTimeRemaining is { } eta
            ? eta.TotalMinutes >= 1
                ? $"{(int)eta.TotalMinutes}m {eta.Seconds}s"
                : $"{eta.Seconds}s"
            : "—";

    /// <summary>True once the download has completed (all bytes received).</summary>
    public bool IsComplete =>
        TotalBytes.HasValue && BytesReceived >= TotalBytes.Value;

    /// <summary>The asset being downloaded (filename only, for display).</summary>
    public required string AssetFileName { get; init; }

    public static DownloadProgress Completed(string assetFileName, long totalBytes) =>
        new()
        {
            AssetFileName = assetFileName,
            BytesReceived = totalBytes,
            TotalBytes = totalBytes,
            BytesPerSecond = 0
        };
}
