using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Entities;
using GodotMan.Domain.Interfaces;
using GodotMan.Domain.Enums;
using GodotMan.Services.DTOs;
using GodotMan.Services.Interfaces;
using GodotMan.Services.Mappers;

namespace GodotMan.Services.Services;

public sealed class ReleaseService : IReleaseService
{
    private readonly IGodotReleaseRepository _releaseRepository;

    public ReleaseService(IGodotReleaseRepository releaseRepository)
    {
        _releaseRepository = releaseRepository ?? throw new ArgumentNullException(nameof(releaseRepository));
    }

    public async Task<IReadOnlyList<ReleaseDto>> GetReleasesAsync(
        GodotVariant variant,
        bool includePreReleases = false,
        CancellationToken cancellationToken = default)
    {
        var releases = await _releaseRepository.GetReleasesAsync(variant, includePreReleases, cancellationToken);
        return releases.Select(ReleaseMapper.ToDto).ToList();
    }

    public async Task<ReleaseDto?> GetLatestStableReleaseAsync(
        GodotVariant variant,
        CancellationToken cancellationToken = default)
    {
        var release = await _releaseRepository.GetLatestStableReleaseAsync(variant, cancellationToken);
        return release is null ? null : ReleaseMapper.ToDto(release);
    }

    public async Task<ReleaseDto?> GetReleaseByVersionAsync(
        string version,
        GodotVariant variant,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Version must not be empty.", nameof(version));
        }

        var release = await _releaseRepository.GetReleaseByVersionAsync(version, variant, cancellationToken);
        return release is null ? null : ReleaseMapper.ToDto(release);
    }
}
