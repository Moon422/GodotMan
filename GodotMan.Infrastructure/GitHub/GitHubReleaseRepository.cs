using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Entities;
using GodotMan.Domain.Enums;
using GodotMan.Domain.Exceptions;
using GodotMan.Domain.Interfaces;
using Octokit;
using OctokitRelease = Octokit.Release;

namespace GodotMan.Infrastructure.GitHub;

public sealed class GitHubReleaseRepository : IGodotReleaseRepository
{
    private readonly GitHubClient _client;
    private readonly string _owner;
    private readonly string _repository;

    public GitHubReleaseRepository(string owner, string repository, GitHubClient? client = null)
    {
        _owner = !string.IsNullOrWhiteSpace(owner)
            ? owner
            : throw new ArgumentException("GitHub owner must not be empty.", nameof(owner));
        _repository = !string.IsNullOrWhiteSpace(repository)
            ? repository
            : throw new ArgumentException(
                "GitHub repository must not be empty.",
                nameof(repository)
            );
        _client = client ?? new GitHubClient(new ProductHeaderValue("GodotMan"));
    }

    public async Task<IReadOnlyList<GodotRelease>> GetReleasesAsync(
        GodotVariant variant,
        bool includePreReleases = false,
        CancellationToken cancellationToken = default
    )
    {
        var allReleases = await LoadAllReleasesAsync(cancellationToken).ConfigureAwait(false);
        var parsedReleases = allReleases
            .Select(release => TryParseRelease(release, variant))
            .Where(release => release is not null)
            .Cast<GodotRelease>()
            .ToList();

        if (!includePreReleases)
        {
            parsedReleases = parsedReleases
                .Where(release => release.Stability == ReleaseStability.Stable)
                .ToList();
        }

        parsedReleases = MarkLatestStable(parsedReleases);
        return parsedReleases.OrderByDescending(r => r.SemanticVersion).ToList();
    }

    public async Task<GodotRelease?> GetLatestStableReleaseAsync(
        GodotVariant variant,
        CancellationToken cancellationToken = default
    )
    {
        var releases = await GetReleasesAsync(variant, includePreReleases: true, cancellationToken)
            .ConfigureAwait(false);
        return releases
            .Where(release => release.Stability == ReleaseStability.Stable)
            .OrderByDescending(release => release.SemanticVersion)
            .FirstOrDefault();
    }

    public async Task<GodotRelease?> GetReleaseByVersionAsync(
        string version,
        GodotVariant variant,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Version must not be empty.", nameof(version));
        }

        var releases = await GetReleasesAsync(variant, includePreReleases: true, cancellationToken)
            .ConfigureAwait(false);
        return releases.FirstOrDefault(release =>
            string.Equals(release.Version, version, StringComparison.OrdinalIgnoreCase)
        );
    }

    private async Task<List<OctokitRelease>> LoadAllReleasesAsync(
        CancellationToken cancellationToken
    )
    {
        try
        {
            var options = new ApiOptions { PageSize = 100 };
            var releases = new List<OctokitRelease>();
            int pageNum = 1;

            while (true)
            {
                options.StartPage = pageNum;
                var page = await _client
                    .Repository.Release.GetAll(_owner, _repository, options)
                    .ConfigureAwait(false);

                if (page.Count == 0)
                {
                    break;
                }

                releases.AddRange(page);
                if (page.Count < options.PageSize)
                {
                    break;
                }

                pageNum++;
            }

            return releases;
        }
        catch (Exception ex)
        {
            throw new GodotManException("Failed to load GitHub releases.", ex);
        }
    }

    private static GodotRelease? TryParseRelease(OctokitRelease release, GodotVariant variant)
    {
        if (release is null)
        {
            return null;
        }

        // Filter assets that match the requested variant before parsing
        var variantFilter = variant == GodotVariant.Mono ? "_mono_" : "_";
        var assets = release
            .Assets.Where(asset =>
            {
                var lower = asset.Name?.ToLowerInvariant() ?? "";
                // Mono assets have "_mono_" in filename, standard do not
                if (variant == GodotVariant.Mono)
                {
                    return lower.Contains("_mono_", StringComparison.Ordinal);
                }
                else
                {
                    return !lower.Contains("_mono_", StringComparison.Ordinal);
                }
            })
            .Select(asset =>
            {
                GitHubAssetParser.TryParseAsset(asset, release.TagName, out var parsed);
                return parsed;
            })
            .Where(parsed => parsed is not null)
            .Cast<Domain.Entities.ReleaseAsset>()
            .ToList();

        if (!assets.Any())
        {
            return null;
        }

        var version = release.TagName ?? string.Empty;
        if (string.IsNullOrWhiteSpace(version))
        {
            return null;
        }

        if (!TryParseSemanticVersion(version, out var semanticVersion))
        {
            return null;
        }

        return new GodotRelease
        {
            Version = version,
            SemanticVersion = semanticVersion,
            Stability = ParseStability(version),
            Variant = variant,
            Assets = assets,
            ReleasePageUrl = release.HtmlUrl ?? string.Empty,
            ReleaseNotes = release.Body,
            PublishedAt = release.PublishedAt.GetValueOrDefault(DateTimeOffset.MinValue),
            IsLatestStable = false,
        };
    }

    private static List<GodotRelease> MarkLatestStable(List<GodotRelease> releases)
    {
        var latestStableVersion = releases
            .Where(release => release.Stability == ReleaseStability.Stable)
            .OrderByDescending(release => release.SemanticVersion)
            .FirstOrDefault()
            ?.Version;

        return releases
            .Select(release => new GodotRelease
            {
                Version = release.Version,
                SemanticVersion = release.SemanticVersion,
                Stability = release.Stability,
                Variant = release.Variant,
                Assets = release.Assets,
                ReleasePageUrl = release.ReleasePageUrl,
                ReleaseNotes = release.ReleaseNotes,
                PublishedAt = release.PublishedAt,
                IsLatestStable =
                    latestStableVersion is not null
                    && string.Equals(
                        release.Version,
                        latestStableVersion,
                        StringComparison.OrdinalIgnoreCase
                    ),
            })
            .ToList();
    }

    private static bool TryParseSemanticVersion(string version, out Version versionValue)
    {
        versionValue = new Version(0, 0);
        var index = version.IndexOf('-');
        var baseVersion = index >= 0 ? version[..index] : version;

        return Version.TryParse(baseVersion, out versionValue);
    }

    private static ReleaseStability ParseStability(string version)
    {
        var lower = version.ToLowerInvariant();
        return lower.Contains("-stable", StringComparison.Ordinal) ? ReleaseStability.Stable
            : lower.Contains("-rc", StringComparison.Ordinal) ? ReleaseStability.ReleaseCandidate
            : lower.Contains("-beta", StringComparison.Ordinal) ? ReleaseStability.Beta
            : lower.Contains("-alpha", StringComparison.Ordinal)
            || lower.Contains("-dev", StringComparison.Ordinal)
                ? ReleaseStability.Dev
            : ReleaseStability.Dev;
    }
}
