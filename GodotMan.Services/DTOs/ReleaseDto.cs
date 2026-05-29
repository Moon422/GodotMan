using System;
using System.Collections.Generic;
using GodotMan.Domain.Entities;
using GodotMan.Domain.Enums;

namespace GodotMan.Services.DTOs;

public sealed class ReleaseDto
{
    public required string Version { get; init; }
    public required Version SemanticVersion { get; init; }
    public required ReleaseStability Stability { get; init; }
    public required GodotVariant Variant { get; init; }
    public required IReadOnlyList<ReleaseAsset> Assets { get; init; }
    public required string ReleasePageUrl { get; init; }
    public string? ReleaseNotes { get; init; }
    public required DateTimeOffset PublishedAt { get; init; }
    public bool IsLatestStable { get; init; }
}
