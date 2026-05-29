using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Exceptions;
using GodotMan.Domain.Interfaces;

namespace GodotMan.Infrastructure.FileSystem;

public sealed class ArchiveExtractor : IArchiveExtractor
{
    public async Task<string> ExtractAsync(
        string archivePath,
        string destinationDirectory,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(archivePath))
        {
            throw new ArgumentException("Archive path must not be empty.", nameof(archivePath));
        }

        if (string.IsNullOrWhiteSpace(destinationDirectory))
        {
            throw new ArgumentException("Destination directory must not be empty.", nameof(destinationDirectory));
        }

        if (!File.Exists(archivePath))
        {
            throw new FileNotFoundException("Archive file not found.", archivePath);
        }

        Directory.CreateDirectory(destinationDirectory);

        try
        {
            await Task.Run(() => ZipFile.ExtractToDirectory(archivePath, destinationDirectory, overwriteFiles: true), cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            var executable = FindExecutable(destinationDirectory);
            if (executable is null)
            {
                throw new InvalidOperationException("Could not locate the Godot executable after extraction.");
            }

            return executable;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex) when (ex is IOException || ex is InvalidDataException || ex is UnauthorizedAccessException)
        {
            throw new GodotManException("Archive extraction failed.", ex);
        }
    }

    private static string? FindExecutable(string rootDirectory)
    {
        var candidates = Directory.EnumerateFiles(rootDirectory, "*", SearchOption.AllDirectories)
            .Where(file =>
            {
                var fileName = Path.GetFileName(file);
                return string.Equals(fileName, "Godot.exe", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(fileName, "Godot", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(fileName, "godot", StringComparison.OrdinalIgnoreCase);
            })
            .OrderBy(path => path.Length)
            .FirstOrDefault();

        return candidates;
    }
}
