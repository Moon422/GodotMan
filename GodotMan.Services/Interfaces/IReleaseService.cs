using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Enums;
using GodotMan.Services.DTOs;

namespace GodotMan.Services.Interfaces;

public interface IReleaseService
{
    Task<IReadOnlyList<ReleaseDto>> GetReleasesAsync(
        GodotVariant variant,
        bool includePreReleases = false,
        CancellationToken cancellationToken = default);

    Task<ReleaseDto?> GetLatestStableReleaseAsync(
        GodotVariant variant,
        CancellationToken cancellationToken = default);

    Task<ReleaseDto?> GetReleaseByVersionAsync(
        string version,
        GodotVariant variant,
        CancellationToken cancellationToken = default);
}
