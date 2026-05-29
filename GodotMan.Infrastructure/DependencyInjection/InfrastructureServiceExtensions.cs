using System;
using GodotMan.Domain.Interfaces;
using GodotMan.Infrastructure.FileSystem;
using GodotMan.Infrastructure.GitHub;
using GodotMan.Infrastructure.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GodotMan.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string installationStorePath,
        string gitHubOwner = "godotengine",
        string gitHubRepository = "godot")
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (string.IsNullOrWhiteSpace(installationStorePath))
        {
            throw new ArgumentException("Installation store path must not be empty.", nameof(installationStorePath));
        }

        services.AddSingleton<IGodotReleaseRepository>(_ => new GitHubReleaseRepository(gitHubOwner, gitHubRepository));
        services.AddSingleton<IInstallationRepository>(_ => new InstallationRepository(installationStorePath));
        services.AddSingleton<IDownloadService, FileDownloader>();
        services.AddSingleton<IArchiveExtractor, ArchiveExtractor>();

        return services;
    }
}
