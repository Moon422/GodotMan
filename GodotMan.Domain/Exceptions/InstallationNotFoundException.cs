using System;
using GodotMan.Domain.Enums;

namespace GodotMan.Domain.Exceptions;

/// <summary>
/// Thrown when an operation targets a <see cref="GodotInstallation"/> that
/// does not exist in the local installation store.
/// </summary>
public sealed class InstallationNotFoundException : GodotManException
{
    public Guid? InstallationId { get; }
    public string? Version { get; }

    public InstallationNotFoundException(Guid id)
        : base($"No installation found with ID '{id}'.")
    {
        InstallationId = id;
    }

    public InstallationNotFoundException(string version, GodotVariant variant)
        : base($"No installation found for Godot {version} ({variant}).")
    {
        Version = version;
    }
}
