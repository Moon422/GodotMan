using System;
using System.Collections.Generic;
using GodotMan.Domain.Enums;

namespace GodotMan.Domain.Entities;

/// <summary>
/// Represents a published Godot Engine release fetched from GitHub.
/// </summary>
public sealed class GodotRelease
{
    /// <summary>Full semantic version string, e.g. "4.3-stable".</summary>
    public required string Version { get; init; }

    /// <summary>Parsed major.minor.patch numeric version for sorting/comparison.</summary>
    public required Version SemanticVersion { get; init; }

    /// <summary>Whether this is a stable, beta, rc, or dev release.</summary>
    public required ReleaseStability Stability { get; init; }

    /// <summary>Variant: Standard (GDScript/C++) or Mono (.NET/C#).</summary>
    public required GodotVariant Variant { get; init; }

    /// <summary>All downloadable assets bundled with this release.</summary>
    public required IReadOnlyList<ReleaseAsset> Assets { get; init; }

    /// <summary>GitHub release page URL.</summary>
    public required string ReleasePageUrl { get; init; }

    /// <summary>Release notes / changelog body from GitHub.</summary>
    public string? ReleaseNotes { get; init; }

    /// <summary>UTC timestamp of when this release was published on GitHub.</summary>
    public required DateTimeOffset PublishedAt { get; init; }

    /// <summary>True when this is the most recent stable release of this variant.</summary>
    public bool IsLatestStable { get; init; }

    public override string ToString() => $"Godot {Version} ({Variant})";
}
