namespace GodotMan.Domain.Interfaces;

/// <summary>
/// Retrieves Godot Engine release information from an external source
/// (e.g. the GitHub API via Octokit.NET).
/// Implementations live in the Infrastructure layer.
/// </summary>
public interface IGodotReleaseRepository
{
    /// <summary>
    /// Returns all releases available for the given <paramref name="variant"/>,
    /// ordered from newest to oldest.
    /// </summary>
    /// <param name="variant">Standard or Mono.</param>
    /// <param name="includePreReleases">
    /// When true, beta/RC/dev releases are included; otherwise only stable
    /// releases are returned.
    /// </param>
    /// <param name="cancellationToken">Propagates cancellation from the caller.</param>
    Task<IReadOnlyList<GodotRelease>> GetReleasesAsync(
        GodotVariant variant,
        bool includePreReleases = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the single latest stable release for the given <paramref name="variant"/>.
    /// Returns null if no stable release is found.
    /// </summary>
    Task<GodotRelease?> GetLatestStableReleaseAsync(
        GodotVariant variant,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the release matching the exact <paramref name="version"/> string
    /// (e.g. "4.3-stable"), or null if not found.
    /// </summary>
    Task<GodotRelease?> GetReleaseByVersionAsync(
        string version,
        GodotVariant variant,
        CancellationToken cancellationToken = default);
}
