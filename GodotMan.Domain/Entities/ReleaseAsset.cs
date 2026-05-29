using GodotMan.Domain.Enums;

namespace GodotMan.Domain.Entities;

/// <summary>
/// A single downloadable file attached to a <see cref="GodotRelease"/>,
/// e.g. "Godot_v4.3-stable_mono_win64.exe.zip".
/// </summary>
public sealed class ReleaseAsset
{
    /// <summary>Original filename as it appears on GitHub.</summary>
    public required string FileName { get; init; }

    /// <summary>Direct download URL for this asset.</summary>
    public required string DownloadUrl { get; init; }

    /// <summary>File size in bytes. Used for progress reporting.</summary>
    public required long SizeBytes { get; init; }

    /// <summary>Target operating system for this asset.</summary>
    public required TargetPlatform Platform { get; init; }

    /// <summary>Target CPU architecture for this asset.</summary>
    public required TargetArchitecture Architecture { get; init; }

    /// <summary>
    /// True when this asset is the server/headless export template rather
    /// than an editor build.
    /// </summary>
    public bool IsExportTemplate { get; init; }

    /// <summary>Human-readable size, e.g. "98.4 MB".</summary>
    public string FormattedSize =>
        SizeBytes switch
        {
            >= 1_073_741_824 => $"{SizeBytes / 1_073_741_824.0:F1} GB",
            >= 1_048_576 => $"{SizeBytes / 1_048_576.0:F1} MB",
            >= 1_024 => $"{SizeBytes / 1_024.0:F1} KB",
            _ => $"{SizeBytes} B",
        };

    public override string ToString() => FileName;
}
