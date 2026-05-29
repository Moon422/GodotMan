using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Entities;
using GodotMan.Domain.Exceptions;
using GodotMan.Domain.Interfaces;

namespace GodotMan.Infrastructure.Http;

public sealed class FileDownloader : IDownloadService
{
    private readonly HttpClient _httpClient;

    public FileDownloader(HttpClient? httpClient = null)
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
        {
            throw new ArgumentException("URL must not be empty.", nameof(url));
        }

        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            throw new ArgumentException("Destination path must not be empty.", nameof(destinationPath));
        }

        var directory = Path.GetDirectoryName(destinationPath);
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Destination directory does not exist: {directory}");
        }

        try
        {
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new DownloadException(url, (int)response.StatusCode);
            }

            var contentLength = response.Content.Headers.ContentLength;
            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await using var fileStream = new FileStream(
                destinationPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true);

            var buffer = new byte[81920];
            long totalBytesRead = 0;
            var stopwatch = Stopwatch.StartNew();
            var lastReport = TimeSpan.Zero;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var bytesRead = await responseStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    break;
                }

                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;

                var elapsed = stopwatch.Elapsed;
                if (progress is not null && elapsed - lastReport >= TimeSpan.FromMilliseconds(500))
                {
                    progress.Report(new DownloadProgress
                    {
                        AssetFileName = Path.GetFileName(destinationPath),
                        BytesReceived = totalBytesRead,
                        TotalBytes = contentLength,
                        BytesPerSecond = totalBytesRead / Math.Max(1.0, elapsed.TotalSeconds)
                    });

                    lastReport = elapsed;
                }
            }

            progress?.Report(DownloadProgress.Completed(Path.GetFileName(destinationPath), totalBytesRead));
            return totalBytesRead;
        }
        catch (OperationCanceledException)
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            throw;
        }
        catch (DownloadException)
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            throw;
        }
        catch (Exception ex)
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            throw new DownloadException(url, "The download failed.", ex);
        }
    }
}
