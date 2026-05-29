using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Entities;
using GodotMan.Domain.Interfaces;

namespace GodotMan.Services.Services;

public sealed class DownloadService : IDownloadService
{
    private readonly HttpClient _httpClient;

    public DownloadService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<long> DownloadAsync(
        string url,
        string destinationPath,
        IProgress<DownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Download URL must not be empty.", nameof(url));

        if (string.IsNullOrWhiteSpace(destinationPath))
            throw new ArgumentException("Destination path must not be empty.", nameof(destinationPath));

        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength;
        var buffer = new byte[81920];
        long totalRead = 0;
        var stopwatch = Stopwatch.StartNew();

        using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        try
        {
            await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, useAsync: true);
            int read;
            long lastReported = 0;
            var lastReportTime = DateTimeOffset.UtcNow;

            while ((read = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                totalRead += read;

                var now = DateTimeOffset.UtcNow;
                var elapsed = stopwatch.Elapsed.TotalSeconds;
                var bytesPerSecond = elapsed > 0 ? totalRead / elapsed : 0;

                if (progress is not null && (totalRead - lastReported >= 512 * 1024 || (now - lastReportTime).TotalMilliseconds >= 500 || (totalBytes.HasValue && totalRead == totalBytes.Value)))
                {
                    lastReported = totalRead;
                    lastReportTime = now;
                    progress.Report(new DownloadProgress
                    {
                        AssetFileName = Path.GetFileName(destinationPath),
                        BytesReceived = totalRead,
                        TotalBytes = totalBytes,
                        BytesPerSecond = bytesPerSecond
                    });
                }
            }
        }
        catch (OperationCanceledException)
        {
            try { if (File.Exists(destinationPath)) File.Delete(destinationPath); } catch { }
            throw;
        }

        progress?.Report(DownloadProgress.Completed(Path.GetFileName(destinationPath), totalRead));
        return totalRead;
    }
}
