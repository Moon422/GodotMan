# AGENT.md — GodotMan

> **Purpose:** This file is the single authoritative reference for any LLM agent
> working on the GodotMan codebase. Read it entirely before writing or modifying
> any code. Update it whenever you add a layer, change a contract, or make an
> architectural decision.

---

## 1. Project overview

**GodotMan** ("Godot Manager") is a cross-platform desktop GUI application that
lets developers download, install, launch, and manage multiple versions of the
[Godot Engine](https://godotengine.org/), including both the **Standard**
(GDScript/C++) and **Mono** (.NET/C#) variants.

| Attribute        | Value                                                                  |
| ---------------- | ---------------------------------------------------------------------- |
| UI framework     | [Avalonia UI](https://avaloniaui.net/) (cross-platform WPF-style XAML) |
| Target framework | .NET 10                                                                |
| GitHub client    | [Octokit.NET](https://github.com/octokit/octokit.net)                  |
| Architecture     | Onion Architecture (domain-centric, DI throughout)                     |
| Language         | C# 14, nullable enabled, implicit usings disabled                      |
| Solution file    | `GodotMan.sln` (root)                                                  |

---

## 2. Architecture — onion layers

Dependencies flow **inward only**. Outer layers reference inner layers; inner
layers never reference outer layers.

```
┌─────────────────────────────────────────┐
│           Presentation (Avalonia)        │  ← References Application
│  ┌───────────────────────────────────┐  │
│  │         Infrastructure            │  │  ← References Application + Domain
│  │  ┌─────────────────────────────┐ │  │
│  │  │       Application           │ │  │  ← References Domain only
│  │  │  ┌───────────────────────┐ │ │  │
│  │  │  │       Domain          │ │ │  │  ← No external dependencies
│  │  │  └───────────────────────┘ │ │  │
│  │  └─────────────────────────────┘ │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

### Layer responsibilities

| Layer              | Project                   | Responsibility                                                                                                 |
| ------------------ | ------------------------- | -------------------------------------------------------------------------------------------------------------- |
| **Domain**         | `GodotMan.Domain`         | Entities, enums, interface contracts, domain exceptions. Zero NuGet deps.                                      |
| **Application**    | `GodotMan.Application`    | Use-case orchestration, DTOs, mappers, service interfaces. References Domain only.                             |
| **Infrastructure** | `GodotMan.Infrastructure` | Octokit.NET GitHub calls, HttpClient downloads, ZIP extraction, JSON file store. Implements Domain interfaces. |
| **Presentation**   | `GodotMan.Presentation`   | Avalonia views, ViewModels (MVVM + ReactiveUI), DI composition root.                                           |

---

## 3. Repository & folder structure

```
GodotMan/
├── GodotMan.sln
├── AGENT.md
├── GodotMan.Domain/
│   ├── GodotMan.Domain.csproj
│   ├── Entities/
│   │   ├── GodotRelease.cs
│   │   ├── GodotInstallation.cs
│   │   ├── ReleaseAsset.cs
│   │   └── DownloadProgress.cs
│   ├── Enums/
│   │   ├── GodotVariant.cs
│   │   ├── InstallationStatus.cs
│   │   ├── ReleaseStability.cs
│   │   ├── TargetPlatform.cs
│   │   └── TargetArchitecture.cs
│   ├── Interfaces/
│   │   ├── IGodotReleaseRepository.cs
│   │   ├── IInstallationRepository.cs
│   │   ├── IDownloadService.cs
│   │   └── IArchiveExtractor.cs
│   └── Exceptions/
│       ├── GodotManException.cs
│       ├── ReleaseNotFoundException.cs
│       ├── InstallationNotFoundException.cs
│       └── DownloadException.cs
│
├── GodotMan.Application/              ← NOT YET IMPLEMENTED
│   ├── GodotMan.Application.csproj
│   ├── Services/
│   │   ├── ReleaseService.cs
│   │   ├── InstallationService.cs
│   │   └── DownloadService.cs
│   ├── DTOs/
│   │   ├── ReleaseDto.cs
│   │   └── InstallationDto.cs
│   ├── Mappers/
│   │   └── ReleaseMapper.cs
│   └── Interfaces/
│       ├── IReleaseService.cs
│       └── IInstallationService.cs
│
├── GodotMan.Infrastructure/           ← NOT YET IMPLEMENTED
│   ├── GodotMan.Infrastructure.csproj
│   ├── GitHub/
│   │   ├── GitHubReleaseRepository.cs
│   │   └── GitHubAssetParser.cs
│   ├── FileSystem/
│   │   ├── InstallationRepository.cs
│   │   └── ArchiveExtractor.cs
│   ├── Http/
│   │   └── FileDownloader.cs
│   └── DependencyInjection/
│       └── InfrastructureServiceExtensions.cs
│
└── GodotMan.Presentation/             ← NOT YET IMPLEMENTED
    ├── GodotMan.Presentation.csproj
    ├── App.axaml / App.axaml.cs
    ├── Program.cs
    ├── Assets/
    ├── Views/
    │   ├── MainWindow.axaml
    │   ├── ReleaseListView.axaml
    │   ├── InstalledView.axaml
    │   └── DownloadProgressView.axaml
    ├── ViewModels/
    │   ├── MainWindowViewModel.cs
    │   ├── ReleaseListViewModel.cs
    │   ├── InstalledViewModel.cs
    │   └── DownloadProgressViewModel.cs
    ├── Controls/
    │   ├── ReleaseCard.axaml
    │   └── VariantToggle.axaml
    └── DependencyInjection/
        └── PresentationServiceExtensions.cs

tests/
├── GodotMan.Domain.Tests/
├── GodotMan.Application.Tests/
└── GodotMan.Infrastructure.Tests/
```

> **Status legend:** Folders marked `← NOT YET IMPLEMENTED` exist in the
> planned structure but have no source files yet. Implement them in layer order:
> Domain → Application → Infrastructure → Presentation.

---

## 4. Domain layer — complete reference

The Domain layer is **fully implemented**. Do not modify existing types without
updating this document. All types live in `src/GodotMan.Domain/`.

### 4.1 Entities

#### `GodotRelease`

Represents a single published release fetched from the GitHub API.

| Property          | Type                          | Notes                                |
| ----------------- | ----------------------------- | ------------------------------------ |
| `Version`         | `string`                      | Full tag string, e.g. `"4.3-stable"` |
| `SemanticVersion` | `System.Version`              | Parsed numeric version for sorting   |
| `Stability`       | `ReleaseStability`            | Stable / RC / Beta / Dev             |
| `Variant`         | `GodotVariant`                | Standard or Mono                     |
| `Assets`          | `IReadOnlyList<ReleaseAsset>` | All downloadable files               |
| `ReleasePageUrl`  | `string`                      | GitHub release HTML URL              |
| `ReleaseNotes`    | `string?`                     | Markdown body from GitHub (nullable) |
| `PublishedAt`     | `DateTimeOffset`              | UTC publish timestamp                |
| `IsLatestStable`  | `bool`                        | Set by repository after fetching     |

All properties use `required` + `init` — construct with object initializer.
`ToString()` → `"Godot 4.3-stable (Mono)"`.

---

#### `ReleaseAsset`

A single downloadable file within a `GodotRelease`.

| Property           | Type                 | Notes                                             |
| ------------------ | -------------------- | ------------------------------------------------- |
| `FileName`         | `string`             | Original GitHub filename                          |
| `DownloadUrl`      | `string`             | Direct download URL                               |
| `SizeBytes`        | `long`               | Raw byte count                                    |
| `Platform`         | `TargetPlatform`     | Windows / macOS / Linux / Web / Android / Unknown |
| `Architecture`     | `TargetArchitecture` | X64 / X86 / Arm64 / Arm32 / Universal / Unknown   |
| `IsExportTemplate` | `bool`               | True for headless/server templates                |
| `FormattedSize`    | `string` (computed)  | Human-readable: `"98.4 MB"`                       |

`ToString()` → `FileName`.

---

#### `GodotInstallation`

A Godot installation present on disk, persisted in the local store.

| Property          | Type                 | Notes                                    |
| ----------------- | -------------------- | ---------------------------------------- |
| `Id`              | `Guid`               | Generated on install, used as stable key |
| `Version`         | `string`             | Matches `GodotRelease.Version`           |
| `SemanticVersion` | `System.Version`     | For sorting                              |
| `Variant`         | `GodotVariant`       | Standard or Mono                         |
| `InstallPath`     | `string`             | Absolute directory path                  |
| `ExecutablePath`  | `string`             | Absolute path to the `.exe` / binary     |
| `Status`          | `InstallationStatus` | See enum below                           |
| `InstalledAt`     | `DateTimeOffset`     | UTC install timestamp                    |
| `LastLaunchedAt`  | `DateTimeOffset?`    | Null until first launch                  |
| `IsDefault`       | `bool`               | System-wide default flag                 |

Because the record is immutable (`init`), status transitions produce a new
instance with `with { Status = ... }`.

---

#### `DownloadProgress`

Immutable value object passed through `IProgress<DownloadProgress>` callbacks.
The UI layer data-binds to the computed properties directly.

| Member                       | Type                 | Notes                                   |
| ---------------------------- | -------------------- | --------------------------------------- |
| `BytesReceived`              | `long`               | Bytes downloaded so far                 |
| `TotalBytes`                 | `long?`              | Null when `Content-Length` is absent    |
| `BytesPerSecond`             | `double`             | Current transfer speed                  |
| `AssetFileName`              | `string`             | Display filename only                   |
| `Fraction`                   | `double?` (computed) | 0.0–1.0, null if total unknown          |
| `PercentageText`             | `string` (computed)  | `"42 %"` or `"—"`                       |
| `SpeedText`                  | `string` (computed)  | `"4.2 MB/s"`                            |
| `EtaText`                    | `string` (computed)  | `"1m 24s"` or `"—"`                     |
| `IsComplete`                 | `bool` (computed)    | True when `BytesReceived >= TotalBytes` |
| `Completed(fileName, total)` | static factory       | Returns a terminal progress snapshot    |

---

### 4.2 Enums

#### `GodotVariant`

```
Standard   // GDScript + C++ (GDExtension), no .NET dependency
Mono       // Adds full C# support via .NET runtime
```

#### `InstallationStatus`

```
Available     // On GitHub, not downloaded
Downloading   // Transfer in progress
Extracting    // Archive being unpacked
Installed     // Ready to launch
Broken        // Executable missing / corrupted
Uninstalling  // Removal in progress
```

#### `ReleaseStability`

```
Stable             // suffix: "stable"
ReleaseCandidate   // suffix: "rc1", "rc2", …
Beta               // suffix: "beta1", …
Dev                // suffix: "alpha1", "dev1", …
```

Derived by `GitHubAssetParser` from the version tag string.

#### `TargetPlatform`

```
Windows | macOS | Linux | Web | Android | Unknown
```

#### `TargetArchitecture`

```
X64 | X86 | Arm64 | Arm32 | Universal | Unknown
```

---

### 4.3 Interfaces

#### `IGodotReleaseRepository`

**Implemented by:** `Infrastructure.GitHub.GitHubReleaseRepository` (via Octokit.NET)

```csharp
Task<IReadOnlyList<GodotRelease>> GetReleasesAsync(
    GodotVariant variant,
    bool includePreReleases = false,
    CancellationToken cancellationToken = default);

Task<GodotRelease?> GetLatestStableReleaseAsync(
    GodotVariant variant,
    CancellationToken cancellationToken = default);

Task<GodotRelease?> GetReleaseByVersionAsync(
    string version,
    GodotVariant variant,
    CancellationToken cancellationToken = default);
```

Results are ordered newest → oldest. `IsLatestStable` is set by the repository
on the matching entry before returning.

---

#### `IInstallationRepository`

**Implemented by:** `Infrastructure.FileSystem.InstallationRepository` (JSON file store)

```csharp
Task<IReadOnlyList<GodotInstallation>> GetAllAsync(CancellationToken ct = default);
Task<GodotInstallation?> GetByIdAsync(Guid id, CancellationToken ct = default);
Task<GodotInstallation?> FindAsync(string version, GodotVariant variant, CancellationToken ct = default);
Task<GodotInstallation?> GetDefaultAsync(CancellationToken ct = default);
Task AddAsync(GodotInstallation installation, CancellationToken ct = default);
Task UpdateAsync(GodotInstallation installation, CancellationToken ct = default);
Task RemoveAsync(Guid id, CancellationToken ct = default);
```

`UpdateAsync` throws `InvalidOperationException` (not `InstallationNotFoundException`)
when the record does not exist — the caller is responsible for confirming existence first.

---

#### `IDownloadService`

**Implemented by:** `Infrastructure.Http.FileDownloader`

```csharp
Task<long> DownloadAsync(
    string url,
    string destinationPath,
    IProgress<DownloadProgress>? progress = null,
    CancellationToken cancellationToken = default);
```

- The destination directory must exist before calling.
- On cancellation, the partial file is deleted.
- Throws `DownloadException` on HTTP errors or network failures.
- Returns total bytes written.

---

#### `IArchiveExtractor`

**Implemented by:** `Infrastructure.FileSystem.ArchiveExtractor`

```csharp
Task<string> ExtractAsync(
    string archivePath,
    string destinationDirectory,
    CancellationToken cancellationToken = default);
```

Returns the absolute path to the extracted Godot executable.
Creates `destinationDirectory` if it does not exist.

---

### 4.4 Exceptions

| Exception                       | Inherits            | When thrown                                                               |
| ------------------------------- | ------------------- | ------------------------------------------------------------------------- |
| `GodotManException`             | `Exception`         | Base — use a subclass when possible                                       |
| `ReleaseNotFoundException`      | `GodotManException` | GitHub release not found; carries `RequestedVersion` + `RequestedVariant` |
| `InstallationNotFoundException` | `GodotManException` | Local record not found; carries `InstallationId` or `Version`             |
| `DownloadException`             | `GodotManException` | Network / HTTP failure; carries `Url` + optional `HttpStatusCode`         |

---

## 5. Conventions & coding rules

### Naming

- **Interfaces** prefixed with `I`: `IGodotReleaseRepository`, `IDownloadService`.
- **Implementations** named after what they do + suffix: `GitHubReleaseRepository`, `FileDownloader`.
- **DTOs** suffixed `Dto`: `ReleaseDto`, `InstallationDto`.
- **ViewModels** suffixed `ViewModel`: `ReleaseListViewModel`.
- **Views** suffixed `View` (AXAML): `ReleaseListView.axaml`.

### Entity construction

All domain entities use `required` properties with `init` setters. Construct
with object initializers. Mutate with `with` expressions to produce new
immutable copies:

```csharp
var updated = existing with { Status = InstallationStatus.Installed };
```

Never add setters or constructors to domain entities.

### Async

- All I/O methods are `async Task` / `async Task<T>`.
- Always accept and forward `CancellationToken`. Default to `default` in
  signatures so callers are not forced to pass one.
- Never use `Task.Result` or `.Wait()` — always `await`.

### Nullable

Nullable reference types are enabled project-wide. Annotate every nullable
return type and parameter. Do not suppress warnings with `!` unless you have
verified the value cannot be null at that point.

### Error handling

- Throw domain exceptions (`GodotManException` subclasses) for business rule
  violations.
- Let infrastructure exceptions (`HttpRequestException`, `IOException`) bubble
  up wrapped inside `DownloadException` or `GodotManException`.
- Never swallow exceptions silently. Log before re-throwing when in
  infrastructure or application code.

### Dependency injection

- Register all services via extension methods in `DependencyInjection/` folders:
  `InfrastructureServiceExtensions.cs`, `PresentationServiceExtensions.cs`.
- Use `IServiceCollection` (Microsoft.Extensions.DependencyInjection).
- Compose everything in `Presentation/Program.cs`.
- Prefer constructor injection. Never use service locator.

### Project references (what may reference what)

```
Domain          →  (nothing)
Application     →  Domain
Infrastructure  →  Domain, Application
Presentation    →  Application, Infrastructure (for DI wiring only)
```

Presentation ViewModels must **only** call Application service interfaces.
They must never call Infrastructure types directly.

---

## 6. GitHub / Godot release conventions

The Godot project on GitHub is `godotengine/godot`. Releases follow a
predictable tag and asset naming convention that `GitHubAssetParser` must
understand:

### Tag format

```
4.3-stable
```

### Asset filename format

```
Godot_v{version}_{platform}.{arch}.{ext}
Godot_v{version}_mono_{platform}_{arch}.{ext}
```

Examples:

```
Godot_v4.3-stable_win64.exe.zip          → Standard, Windows, X64
Godot_v4.3-stable_win32.exe.zip          → Standard, Windows, X86
Godot_v4.3-stable_macos.universal.zip    → Standard, macOS, Universal
Godot_v4.3-stable_linux.x86_64.zip       → Standard, Linux, X64
Godot_v4.3-stable_mono_win64.zip         → Mono, Windows, X64
Godot_v4.3-stable_mono_macos.universal.zip → Mono, macOS, Universal
Godot_v4.3-stable_mono_linux_._x86_64.zip       → Standard, Linux, X64
Godot_v4.3-stable_export_templates.tpz   → Export templates (skip)
```

`GitHubAssetParser` must:

1. Detect `_mono_` in the filename → `GodotVariant.Mono`, else `Standard`.
2. Skip assets whose name contains `export_templates` or ends with `.sha256`.
3. Parse platform segment: `win` → Windows, `macos`/`osx` → macOS, `linux`/`x11` → Linux, `web` → Web, `android` → Android.
4. Parse arch segment: `64`/`x86_64` → X64, `32`/`x86_32` → X86, `arm64` → Arm64, `arm32` → Arm32, `universal` → Universal.
5. Set `IsExportTemplate = false` for all editor builds.

---

## 7. Key data flow

### Browsing releases

```
ReleaseListViewModel
  └─ IReleaseService.GetReleasesAsync(variant, includePreReleases)
       └─ IGodotReleaseRepository.GetReleasesAsync(variant, includePreReleases)
            └─ Octokit: GitHubClient.Repository.Release.GetAll("godotengine","godot")
                 └─ GitHubAssetParser.Parse(release) → GodotRelease
```

### Installing a release

```
InstalledViewModel.InstallCommand(release, asset)
  └─ IInstallationService.InstallAsync(release, asset, progress, ct)
       ├─ IDownloadService.DownloadAsync(asset.DownloadUrl, tempPath, progress, ct)
       ├─ IArchiveExtractor.ExtractAsync(tempPath, installDir, ct)
       ├─ IInstallationRepository.AddAsync(new GodotInstallation { ... })
       └─ delete tempPath
```

### Launching an installation

```
InstalledViewModel.LaunchCommand(installation)
  └─ IInstallationService.LaunchAsync(installation)
       ├─ Process.Start(installation.ExecutablePath)
       └─ IInstallationRepository.UpdateAsync(installation with { LastLaunchedAt = DateTimeOffset.UtcNow })
```

### Uninstalling

```
InstalledViewModel.UninstallCommand(installation)
  └─ IInstallationService.UninstallAsync(installation.Id)
       ├─ IInstallationRepository.UpdateAsync(... with { Status = Uninstalling })
       ├─ Directory.Delete(installation.InstallPath, recursive: true)
       └─ IInstallationRepository.RemoveAsync(installation.Id)
```

---

## 8. What to implement next

Work in this order to avoid blocked dependencies:

1. **`GodotMan.Application`** — service interfaces and implementations.
   All business logic lives here. No UI, no HTTP, no file system calls.

2. **`GodotMan.Infrastructure`** — concrete implementations of Domain
   interfaces. Add NuGet packages: `Octokit`, (HttpClient is built-in).

3. **`GodotMan.Presentation`** — Avalonia project. Add NuGet packages:
   `Avalonia`, `Avalonia.Desktop`, `Avalonia.ReactiveUI`.
   Wire up DI in `Program.cs`.

4. **Tests** — unit test Domain and Application with xUnit + NSubstitute.
   Integration-test Infrastructure with a live GitHub API call (optional,
   rate-limit aware).

---

## 9. Do not

- Do not add NuGet dependencies to `GodotMan.Domain`.
- Do not reference `GodotMan.Infrastructure` from `GodotMan.Application`.
- Do not call Domain interfaces directly from ViewModels — always go via
  Application service interfaces.
- Do not use `static` state or singletons outside of DI registration.
- Do not hardcode the GitHub repo owner/name — pass as configuration so it
  can be overridden in tests.
- Do not block on async code (no `.Result`, no `.Wait()`).
- Do not add setters to domain entities — use `with` expressions.
