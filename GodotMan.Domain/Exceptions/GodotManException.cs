using System;

namespace GodotMan.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-level errors in GodotMan.
/// Prefer a specific subclass when one exists.
/// </summary>
public class GodotManException : Exception
{
    public GodotManException(string message)
        : base(message) { }

    public GodotManException(string message, Exception innerException)
        : base(message, innerException) { }
}
