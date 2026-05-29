namespace GodotMan.Domain.Enums;

/// <summary>
/// The stability tier of a Godot release, derived from the version tag suffix.
/// </summary>
public enum ReleaseStability
{
    /// <summary>Production-ready release (tag suffix: "stable").</summary>
    Stable,

    /// <summary>Release candidate — feature-frozen, final testing (suffix: "rc1", "rc2", …).</summary>
    ReleaseCandidate,

    /// <summary>Beta — feature-complete but not yet stable (suffix: "beta1", …).</summary>
    Beta,

    /// <summary>Alpha / development snapshot — may be unstable (suffix: "alpha1", "dev1", …).</summary>
    Dev
}
