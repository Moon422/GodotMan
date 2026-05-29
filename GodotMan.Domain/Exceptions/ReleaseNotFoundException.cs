using System;
using GodotMan.Domain.Enums;

namespace GodotMan.Domain.Exceptions;

/// <summary>
/// Thrown when a requested Godot release cannot be found on GitHub
/// or does not match the expected variant/version combination.
/// </summary>
public sealed class ReleaseNotFoundException : GodotManException
{
    public string RequestedVersion { get; }
    public GodotVariant RequestedVariant { get; }

    public ReleaseNotFoundException(string version, GodotVariant variant)
        : base($"Godot release '{version}' ({variant}) was not found.")
    {
        RequestedVersion = version;
        RequestedVariant = variant;
    }

    public ReleaseNotFoundException(string version, GodotVariant variant, Exception innerException)
        : base($"Godot release '{version}' ({variant}) was not found.", innerException)
    {
        RequestedVersion = version;
        RequestedVariant = variant;
    }
}
