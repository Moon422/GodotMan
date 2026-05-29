using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Interfaces;
using GodotMan.Domain.Enums;
using GodotMan.Services.DTOs;
using GodotMan.Services.Interfaces;
using GodotMan.Services.Mappers;

namespace GodotMan.Services.Services;

public sealed class InstallationService : IInstallationService
{
    private readonly IInstallationRepository _installationRepository;

    public InstallationService(IInstallationRepository installationRepository)
    {
        _installationRepository = installationRepository ?? throw new ArgumentNullException(nameof(installationRepository));
    }

    public async Task<IReadOnlyList<InstallationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var installations = await _installationRepository.GetAllAsync(cancellationToken);
        return installations.Select(InstallationMapper.ToDto).ToList();
    }

    public async Task<InstallationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var installation = await _installationRepository.GetByIdAsync(id, cancellationToken);
        return installation is null ? null : InstallationMapper.ToDto(installation);
    }

    public async Task<InstallationDto?> FindAsync(string version, GodotVariant variant, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Version must not be empty.", nameof(version));
        }

        var installation = await _installationRepository.FindAsync(version, variant, cancellationToken);
        return installation is null ? null : InstallationMapper.ToDto(installation);
    }

    public async Task<InstallationDto?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        var installation = await _installationRepository.GetDefaultAsync(cancellationToken);
        return installation is null ? null : InstallationMapper.ToDto(installation);
    }

    public Task AddAsync(InstallationDto installation, CancellationToken cancellationToken = default)
    {
        if (installation is null)
        {
            throw new ArgumentNullException(nameof(installation));
        }

        return _installationRepository.AddAsync(InstallationMapper.ToEntity(installation), cancellationToken);
    }

    public Task UpdateAsync(InstallationDto installation, CancellationToken cancellationToken = default)
    {
        if (installation is null)
        {
            throw new ArgumentNullException(nameof(installation));
        }

        return _installationRepository.UpdateAsync(InstallationMapper.ToEntity(installation), cancellationToken);
    }

    public Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _installationRepository.RemoveAsync(id, cancellationToken);
    }
}
