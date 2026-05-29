using System;

namespace GodotMan.Domain.Exceptions;

/// <summary>
/// Thrown when a file download fails due to a network error, HTTP error,
/// or unexpected interruption.
/// </summary>
public sealed class DownloadException : GodotManException
{
    /// <summary>URL that was being downloaded when the error occurred.</summary>
    public string Url { get; }

    /// <summary>HTTP status code, if the failure was an HTTP-level error.</summary>
    public int? HttpStatusCode { get; }

    public DownloadException(string url, string reason)
        : base($"Download failed for '{url}': {reason}")
    {
        Url = url;
    }

    public DownloadException(string url, int httpStatusCode)
        : base($"Download failed for '{url}': HTTP {httpStatusCode}.")
    {
        Url = url;
        HttpStatusCode = httpStatusCode;
    }

    public DownloadException(string url, string reason, Exception innerException)
        : base($"Download failed for '{url}': {reason}", innerException)
    {
        Url = url;
    }
}
