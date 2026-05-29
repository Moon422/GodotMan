using GodotMan.Domain.Entities;
using GodotMan.Services.DTOs;

namespace GodotMan.Services.Mappers;

internal static class InstallationMapper
{
    public static InstallationDto ToDto(GodotInstallation installation) => new()
    {
        Id = installation.Id,
        Version = installation.Version,
        SemanticVersion = installation.SemanticVersion,
        Variant = installation.Variant,
        InstallPath = installation.InstallPath,
        ExecutablePath = installation.ExecutablePath,
        Status = installation.Status,
        InstalledAt = installation.InstalledAt,
        LastLaunchedAt = installation.LastLaunchedAt,
        IsDefault = installation.IsDefault
    };

    public static GodotInstallation ToEntity(InstallationDto dto) => new()
    {
        Id = dto.Id,
        Version = dto.Version,
        SemanticVersion = dto.SemanticVersion,
        Variant = dto.Variant,
        InstallPath = dto.InstallPath,
        ExecutablePath = dto.ExecutablePath,
        Status = dto.Status,
        InstalledAt = dto.InstalledAt,
        LastLaunchedAt = dto.LastLaunchedAt,
        IsDefault = dto.IsDefault
    };
}
