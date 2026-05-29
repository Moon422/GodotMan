using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Entities;
using GodotMan.Domain.Enums;

namespace GodotMan.Domain.Interfaces;

/// <summary>
/// Persists and retrieves <see cref="GodotInstallation"/> records on the local machine.
/// Implementations live in the Infrastructure layer (e.g. JSON file store).
/// </summary>
public interface IInstallationRepository
{
    /// <summary>
    /// Returns all known installations, regardless of status, ordered by
    /// semantic version descending.
    /// </summary>
    Task<IReadOnlyList<GodotInstallation>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single installation by its <paramref name="id"/>,
    /// or null if not found.
    /// </summary>
    Task<GodotInstallation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds an installed version matching the given <paramref name="version"/>
    /// string and <paramref name="variant"/>, or null if not installed.
    /// </summary>
    Task<GodotInstallation?> FindAsync(
        string version,
        GodotVariant variant,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the installation marked as the system-wide default,
    /// or null if no default has been set.
    /// </summary>
    Task<GodotInstallation?> GetDefaultAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Persists a new installation record.</summary>
    Task AddAsync(
        GodotInstallation installation,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing installation record (e.g. status change, last launched).
    /// Throws <see cref="InvalidOperationException"/> if the record does not exist.
    /// </summary>
    Task UpdateAsync(
        GodotInstallation installation,
        CancellationToken cancellationToken = default);

    /// <summary>Removes an installation record by its <paramref name="id"/>.</summary>
    Task RemoveAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
