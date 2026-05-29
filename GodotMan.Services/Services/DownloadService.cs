using System;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Entities;
using GodotMan.Domain.Interfaces;

namespace GodotMan.Services.Services;

public sealed class DownloadService
{
    private readonly IDownloadService _downloadService;

    public DownloadService(IDownloadService downloadService)
    {
        _downloadService = downloadService ?? throw new ArgumentNullException(nameof(downloadService));
    }

    public Task<long> DownloadAsync(
        string url,
        string destinationPath,
        IProgress<DownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Download URL must not be empty.", nameof(url));
        }

        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            throw new ArgumentException("Destination path must not be empty.", nameof(destinationPath));
        }

        return _downloadService.DownloadAsync(url, destinationPath, progress, cancellationToken);
    }
}
