namespace GodotMan.Domain.Interfaces;

/// <summary>
/// Extracts downloaded Godot archives (ZIP) to a target directory.
/// Implementations live in the Infrastructure layer.
/// </summary>
public interface IArchiveExtractor
{
    /// <summary>
    /// Extracts the archive at <paramref name="archivePath"/> into
    /// <paramref name="destinationDirectory"/>.
    /// </summary>
    /// <param name="archivePath">Full path to the downloaded .zip file.</param>
    /// <param name="destinationDirectory">
    /// Directory into which the archive contents are extracted.
    /// Created automatically if it does not exist.
    /// </param>
    /// <param name="cancellationToken">Propagates cancellation from the caller.</param>
    /// <returns>
    /// The full path to the extracted Godot executable, resolved after extraction.
    /// </returns>
    Task<string> ExtractAsync(
        string archivePath,
        string destinationDirectory,
        CancellationToken cancellationToken = default);
}
