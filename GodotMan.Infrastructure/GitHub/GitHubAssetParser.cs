using System;
using System.Collections.Generic;
using System.Linq;
using GodotMan.Domain.Enums;
using OctokitAsset = Octokit.ReleaseAsset;
using DomainAsset = GodotMan.Domain.Entities.ReleaseAsset;

namespace GodotMan.Infrastructure.GitHub;

internal static class GitHubAssetParser
{
    public static bool TryParseAsset(OctokitAsset githubAsset, string releaseVersion, out DomainAsset? asset)
    {
        asset = null;

        if (githubAsset is null || string.IsNullOrWhiteSpace(githubAsset.Name))
        {
            return false;
        }

        var fileName = githubAsset.Name;
        var lowerName = fileName.ToLowerInvariant();

        // Skip non-editor assets
        if (lowerName.Contains("export_templates") || lowerName.EndsWith(".sha256") || lowerName.Contains("checksum"))
        {
            return false;
        }

        if (!lowerName.StartsWith("godot_v", StringComparison.Ordinal))
        {
            return false;
        }

        var trimmedName = RemoveArchiveExtension(lowerName);
        if (!TrySplitOnce(trimmedName, '_', out _, out var suffix) || string.IsNullOrWhiteSpace(suffix))
        {
            return false;
        }

        // Detect variant
        var variant = suffix.StartsWith("mono_", StringComparison.Ordinal)
            ? GodotVariant.Mono
            : GodotVariant.Standard;

        if (variant == GodotVariant.Mono)
        {
            suffix = suffix["mono_".Length..];
        }

        // Normalize underscores (handle _._)
        suffix = suffix.Replace("_._", ".");
        var tokens = suffix.Split(new[] { '_', '.' }, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0)
        {
            return false;
        }

        if (!TryParsePlatform(tokens, out var platform, out var architecture))
        {
            return false;
        }

        var downloadUrl = githubAsset.BrowserDownloadUrl ?? githubAsset.Url;
        if (string.IsNullOrWhiteSpace(downloadUrl))
        {
            return false;
        }

        asset = new DomainAsset
        {
            FileName = fileName,
            DownloadUrl = downloadUrl,
            SizeBytes = githubAsset.Size,
            Platform = platform,
            Architecture = architecture,
            IsExportTemplate = false
        };

        return true;
    }

    private static string RemoveArchiveExtension(string fileName)
    {
        foreach (var extension in new[] { ".zip", ".tpz", ".tar.gz", ".gz" })
        {
            if (fileName.EndsWith(extension, StringComparison.Ordinal))
            {
                return fileName[..^extension.Length];
            }
        }

        return fileName;
    }

    private static bool TrySplitOnce(string value, char separator, out string first, out string remainder)
    {
        var index = value.IndexOf(separator);
        if (index < 0)
        {
            first = value;
            remainder = string.Empty;
            return false;
        }

        first = value[..index];
        remainder = value[(index + 1)..];
        return true;
    }

    private static bool TryParsePlatform(string[] tokens, out TargetPlatform platform, out TargetArchitecture architecture)
    {
        platform = TargetPlatform.Unknown;
        architecture = TargetArchitecture.Unknown;

        if (tokens.Length == 0)
        {
            return false;
        }

        var first = tokens[0];

        // Windows
        if (first.StartsWith("win", StringComparison.Ordinal))
        {
            platform = TargetPlatform.Windows;
            if (!TryCoerceWindowsArchitecture(first, out architecture))
            {
                architecture = ParseArchitecture(tokens.Skip(1));
            }
            return true;
        }

        // macOS
        if (first.StartsWith("macos", StringComparison.Ordinal) || first.StartsWith("osx", StringComparison.Ordinal))
        {
            platform = TargetPlatform.MacOS;
            architecture = ParseArchitecture(tokens.Skip(1));
            return true;
        }

        // Linux
        if (first.StartsWith("linux", StringComparison.Ordinal) || first.StartsWith("x11", StringComparison.Ordinal))
        {
            platform = TargetPlatform.Linux;
            architecture = ParseArchitecture(tokens.Skip(1).Prepend(first));
            return true;
        }

        // Web
        if (first.StartsWith("web", StringComparison.Ordinal))
        {
            platform = TargetPlatform.Web;
            architecture = ParseArchitecture(tokens.Skip(1));
            return true;
        }

        // Android
        if (first.StartsWith("android", StringComparison.Ordinal))
        {
            platform = TargetPlatform.Android;
            architecture = ParseArchitecture(tokens.Skip(1));
            return true;
        }

        return false;
    }

    private static bool TryCoerceWindowsArchitecture(string token, out TargetArchitecture architecture)
    {
        architecture = TargetArchitecture.Unknown;

        if (token.Contains("64", StringComparison.Ordinal))
        {
            architecture = TargetArchitecture.X64;
            return true;
        }

        if (token.Contains("32", StringComparison.Ordinal))
        {
            architecture = TargetArchitecture.X86;
            return true;
        }

        return false;
    }

    private static TargetArchitecture ParseArchitecture(IEnumerable<string> tokens)
    {
        foreach (var token in tokens)
        {
            var normalized = token.Replace("-", "_", StringComparison.Ordinal);

            if (normalized.Contains("x86_64", StringComparison.Ordinal))
            {
                return TargetArchitecture.X64;
            }

            if (normalized.Contains("x64", StringComparison.Ordinal) && !normalized.Contains("x86_64", StringComparison.Ordinal))
            {
                return TargetArchitecture.X64;
            }

            if (normalized.Contains("x86_32", StringComparison.Ordinal)
                || (normalized.Contains("x86", StringComparison.Ordinal) && normalized.Contains("32", StringComparison.Ordinal))
                || normalized.Equals("32", StringComparison.Ordinal))
            {
                return TargetArchitecture.X86;
            }

            if (normalized.Contains("arm64", StringComparison.Ordinal))
            {
                return TargetArchitecture.Arm64;
            }

            if (normalized.Contains("arm32", StringComparison.Ordinal)
                || (normalized.Contains("arm", StringComparison.Ordinal) && normalized.Contains("32", StringComparison.Ordinal)))
            {
                return TargetArchitecture.Arm32;
            }

            if (normalized.Contains("universal", StringComparison.Ordinal))
            {
                return TargetArchitecture.Universal;
            }
        }

        return TargetArchitecture.Unknown;
    }
}
