namespace GodotMan.Domain.Enums;

/// <summary>
/// CPU architecture targeted by a <see cref="ReleaseAsset"/>.
/// </summary>
public enum TargetArchitecture
{
    /// <summary>64-bit x86 (most common desktop target).</summary>
    X64,

    /// <summary>32-bit x86.</summary>
    X86,

    /// <summary>64-bit ARM (Apple Silicon, ARM Linux).</summary>
    Arm64,

    /// <summary>32-bit ARM.</summary>
    Arm32,

    /// <summary>
    /// Universal / fat binary containing multiple architectures (macOS).
    /// </summary>
    Universal,

    /// <summary>Architecture not determined or not applicable.</summary>
    Unknown,
}
