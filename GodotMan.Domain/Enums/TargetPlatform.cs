namespace GodotMan.Domain.Enums;


/// <summary>
/// The operating system a <see cref="ReleaseAsset"/> targets.
/// </summary>
public enum TargetPlatform
{
    /// <summary>Microsoft Windows (32-bit or 64-bit).</summary>
    Windows,

    /// <summary>macOS (Universal / arm64 / x86_64).</summary>
    MacOS,

    /// <summary>Linux / X11 desktop.</summary>
    Linux,

    /// <summary>
    /// Web / HTML5 export template — not an editor build.
    /// </summary>
    Web,

    /// <summary>
    /// Android export template — not an editor build.
    /// </summary>
    Android,

    /// <summary>Asset type not recognised (e.g. checksums, source archives).</summary>
    Unknown
}
