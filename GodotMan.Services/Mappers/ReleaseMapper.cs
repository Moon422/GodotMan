using GodotMan.Domain.Entities;
using GodotMan.Services.DTOs;

namespace GodotMan.Services.Mappers;

internal static class ReleaseMapper
{
    public static ReleaseDto ToDto(GodotRelease release) =>
        new()
        {
            Version = release.Version,
            SemanticVersion = release.SemanticVersion,
            Stability = release.Stability,
            Variant = release.Variant,
            Assets = release.Assets,
            ReleasePageUrl = release.ReleasePageUrl,
            ReleaseNotes = release.ReleaseNotes,
            PublishedAt = release.PublishedAt,
            IsLatestStable = release.IsLatestStable,
        };
}
