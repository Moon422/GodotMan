using System;
using GodotMan.Domain.Enums;

namespace GodotMan.Services.DTOs;

public sealed class InstallationDto
{
    public required Guid Id { get; init; }
    public required string Version { get; init; }
    public required Version SemanticVersion { get; init; }
    public required GodotVariant Variant { get; init; }
    public required string InstallPath { get; init; }
    public required string ExecutablePath { get; init; }
    public required InstallationStatus Status { get; init; }
    public required DateTimeOffset InstalledAt { get; init; }
    public DateTimeOffset? LastLaunchedAt { get; init; }
    public bool IsDefault { get; init; }
}
