using System;

namespace GodotMan.Domain.Entities;

/// <summary>
/// Represents a Godot Engine installation that is present on the local machine.
/// </summary>
public sealed class GodotInstallation
{
    /// <summary>Unique identifier (generated on install, persisted in local store).</summary>
    public required Guid Id { get; init; }

    /// <summary>Full version string matching the originating <see cref="GodotRelease"/>.</summary>
    public required string Version { get; init; }

    /// <summary>Parsed semantic version for sorting.</summary>
    public required Version SemanticVersion { get; init; }

    /// <summary>Standard or Mono variant.</summary>
    public required GodotVariant Variant { get; init; }

    /// <summary>Absolute path to the installation directory.</summary>
    public required string InstallPath { get; init; }

    /// <summary>
    /// Absolute path to the Godot executable within <see cref="InstallPath"/>.
    /// </summary>
    public required string ExecutablePath { get; init; }

    /// <summary>Current lifecycle status of this installation.</summary>
    public required InstallationStatus Status { get; init; }

    /// <summary>UTC timestamp when this version was installed.</summary>
    public required DateTimeOffset InstalledAt { get; init; }

    /// <summary>UTC timestamp of the most recent launch, or null if never launched.</summary>
    public DateTimeOffset? LastLaunchedAt { get; init; }

    /// <summary>
    /// Whether this installation is pinned as the system-wide default,
    /// used when launching projects that don't specify a version.
    /// </summary>
    public bool IsDefault { get; init; }

    public override string ToString() => $"Godot {Version} ({Variant}) @ {InstallPath}";
}
