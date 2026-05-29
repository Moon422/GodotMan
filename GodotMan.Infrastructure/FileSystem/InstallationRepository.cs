using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using GodotMan.Domain.Entities;
using GodotMan.Domain.Enums;
using GodotMan.Domain.Exceptions;
using GodotMan.Domain.Interfaces;

namespace GodotMan.Infrastructure.FileSystem;

public sealed class InstallationRepository : IInstallationRepository
{
    private readonly string _storePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public InstallationRepository(string storePath)
    {
        if (string.IsNullOrWhiteSpace(storePath))
        {
            throw new ArgumentException("Store path must not be empty.", nameof(storePath));
        }

        _storePath = Path.GetFullPath(storePath);
        var directory = Path.GetDirectoryName(_storePath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new ArgumentException("Store path must contain a directory.", nameof(storePath));
        }

        Directory.CreateDirectory(directory);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        _jsonOptions.Converters.Add(new VersionJsonConverter());
    }

    public static string GetDefaultStorePath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(path, "GodotMan", "installations.json");
    }

    public async Task<IReadOnlyList<GodotInstallation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var installations = await LoadAsync(cancellationToken).ConfigureAwait(false);
        return installations.OrderByDescending(i => i.SemanticVersion).ToList();
    }

    public async Task<GodotInstallation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var installations = await GetAllAsync(cancellationToken).ConfigureAwait(false);
        return installations.FirstOrDefault(i => i.Id == id);
    }

    public async Task<GodotInstallation?> FindAsync(string version, GodotVariant variant, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Version must not be empty.", nameof(version));
        }

        var installations = await GetAllAsync(cancellationToken).ConfigureAwait(false);
        return installations.FirstOrDefault(i => string.Equals(i.Version, version, StringComparison.OrdinalIgnoreCase) && i.Variant == variant);
    }

    public async Task<GodotInstallation?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        var installations = await GetAllAsync(cancellationToken).ConfigureAwait(false);
        return installations.FirstOrDefault(i => i.IsDefault);
    }

    public async Task AddAsync(GodotInstallation installation, CancellationToken cancellationToken = default)
    {
        if (installation is null)
        {
            throw new ArgumentNullException(nameof(installation));
        }

        var installations = await LoadAsync(cancellationToken).ConfigureAwait(false);
        if (installations.Any(i => i.Id == installation.Id))
        {
            throw new InvalidOperationException($"Installation with ID '{installation.Id}' already exists.");
        }

        installations.Add(installation);
        await SaveAsync(installations, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateAsync(GodotInstallation installation, CancellationToken cancellationToken = default)
    {
        if (installation is null)
        {
            throw new ArgumentNullException(nameof(installation));
        }

        var installations = await LoadAsync(cancellationToken).ConfigureAwait(false);
        var index = installations.FindIndex(i => i.Id == installation.Id);
        if (index < 0)
        {
            throw new InvalidOperationException($"Installation with ID '{installation.Id}' does not exist.");
        }

        installations[index] = installation;
        await SaveAsync(installations, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var installations = await LoadAsync(cancellationToken).ConfigureAwait(false);
        var removed = installations.RemoveAll(i => i.Id == id) > 0;
        if (!removed)
        {
            throw new InvalidOperationException($"Installation with ID '{id}' does not exist.");
        }

        await SaveAsync(installations, cancellationToken).ConfigureAwait(false);
    }

    private async Task<List<GodotInstallation>> LoadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_storePath))
        {
            return new List<GodotInstallation>();
        }

        try
        {
            await using var stream = File.Open(_storePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var installations = await JsonSerializer.DeserializeAsync<List<GodotInstallation>>(stream, _jsonOptions, cancellationToken).ConfigureAwait(false);
            return installations ?? new List<GodotInstallation>();
        }
        catch (JsonException ex)
        {
            throw new GodotManException("Failed to read installation store.", ex);
        }
    }

    private async Task SaveAsync(List<GodotInstallation> installations, CancellationToken cancellationToken)
    {
        await using var stream = File.Open(_storePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, installations, _jsonOptions, cancellationToken).ConfigureAwait(false);
        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}

internal sealed class VersionJsonConverter : JsonConverter<Version>
{
    public override Version? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            return null;
        }

        var value = reader.GetString();
        return value is null ? null : Version.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
