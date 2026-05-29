using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Enums;
using GodotMan.Services.DTOs;

namespace GodotMan.Services.Interfaces;

public interface IInstallationService
{
    Task<IReadOnlyList<InstallationDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<InstallationDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<InstallationDto?> FindAsync(
        string version,
        GodotVariant variant,
        CancellationToken cancellationToken = default);

    Task<InstallationDto?> GetDefaultAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        InstallationDto installation,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        InstallationDto installation,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
