namespace GodotMan.Domain.Interfaces;

/// <summary>
/// Downloads files from the internet and reports progress.
/// Implementations live in the Infrastructure layer (HttpClient-based).
/// </summary>
public interface IDownloadService
{
    /// <summary>
    /// Downloads the file at <paramref name="url"/> to <paramref name="destinationPath"/>,
    /// reporting progress via <paramref name="progress"/>.
    /// </summary>
    /// <param name="url">Direct URL to the file (e.g. a GitHub asset download URL).</param>
    /// <param name="destinationPath">
    /// Full path to the local file that will be written.
    /// The directory must already exist.
    /// </param>
    /// <param name="progress">
    /// Optional progress callback. Receives <see cref="DownloadProgress"/> updates
    /// at a reasonable frequency (e.g. every 0.5 s or every 512 KB).
    /// </param>
    /// <param name="cancellationToken">
    /// Allows the caller to cancel the download; the partially written file
    /// will be deleted on cancellation.
    /// </param>
    /// <returns>Total bytes written.</returns>
    Task<long> DownloadAsync(
        string url,
        string destinationPath,
        IProgress<DownloadProgress>? progress = null,
        CancellationToken cancellationToken = default);
}
