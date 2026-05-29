namespace GodotMan.Domain.Enums;

/// <summary>
/// Lifecycle state of a <see cref="GodotInstallation"/> on the local machine.
/// </summary>
public enum InstallationStatus
{
    /// <summary>
    /// The release is available on GitHub but has not been downloaded.
    /// </summary>
    Available,

    /// <summary>
    /// The asset is currently being downloaded from GitHub.
    /// </summary>
    Downloading,

    /// <summary>
    /// The download completed and the archive is being extracted.
    /// </summary>
    Extracting,

    /// <summary>
    /// The engine is fully installed and ready to launch.
    /// </summary>
    Installed,

    /// <summary>
    /// The installation could not be verified — executable missing or corrupted.
    /// </summary>
    Broken,

    /// <summary>
    /// The installation is in the process of being removed.
    /// </summary>
    Uninstalling
}
