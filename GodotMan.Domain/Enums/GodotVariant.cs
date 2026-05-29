namespace GodotMan.Domain.Enums;

/// <summary>
/// The two official distribution variants of the Godot Engine editor.
/// </summary>
public enum GodotVariant
{
    /// <summary>
    /// The standard build. Supports GDScript and C++ (GDExtension).
    /// Smaller download, no .NET runtime dependency.
    /// </summary>
    Standard,

    /// <summary>
    /// The Mono / .NET build. Adds full C# support via the .NET runtime.
    /// Requires a compatible .NET SDK to be installed separately.
    /// </summary>
    Mono
}
