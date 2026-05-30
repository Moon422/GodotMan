# GodotMan

A cross-platform desktop application for managing multiple Godot Engine installations, downloads, and version switching.

## Features

- **Browse Releases**: View available Godot Engine releases (Standard and Mono variants)
- **Download & Install**: Download and install specific Godot Engine versions
- **Manage Installations**: Track installed versions, launch them, and set defaults
- **Download Progress**: Real-time download progress with speed and ETA tracking
- **Pre-release Support**: Option to include pre-release versions in your releases list
- **Cross-platform**: Runs on Windows, macOS, and Linux

## Architecture

GodotMan follows a clean, layered architecture:

```
GodotMan.App (Presentation Layer)
    ↓ uses
GodotMan.Services (Application/Business Logic Layer)
    ↓ uses
GodotMan.Infrastructure (Data Access & External Services)
    ↓ uses
GodotMan.Domain (Core Entities & Interfaces)
```

### Project Structure

```
GodotMan/
├── GodotMan.App/              # Avalonia UI presentation layer (MVVM + ReactiveUI)
│   ├── ViewModels/            # MVVM ViewModels
│   ├── Views/                 # Avalonia XAML Views
│   ├── Controls/              # Custom Avalonia controls
│   ├── App.axaml              # Application entry point
│   └── MainWindow.axaml       # Main application window
├── GodotMan.Services/         # Business logic & application services
│   ├── Services/              # Service implementations
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Mappers/               # DTO mappers
│   └── Interfaces/            # Service interfaces
├── GodotMan.Infrastructure/   # Data access & external services
│   ├── DependencyInjection/   # DI configuration
│   ├── Services/              # Implementation of repository & download services
│   └── Configuration/         # App configuration setup
├── GodotMan.Domain/           # Core domain models
│   ├── Entities/              # Domain entities
│   ├── Enums/                 # Domain enumerations
│   ├── Exceptions/            # Custom exceptions
│   └── Interfaces/            # Domain interfaces
└── GodotMan.slnx              # Solution file
```

## Prerequisites

- **.NET 10** or later
- **C# 14** or later
- Platform-specific requirements:
  - **Windows**: .NET Desktop Runtime
  - **macOS**: .NET Runtime
  - **Linux**: .NET Runtime

## Building

### From the command line:

```bash
dotnet build
```

### Run the application:

```bash
dotnet run --project GodotMan.App/GodotMan.App.csproj
```

### Publish a self-contained executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

Replace `win-x64` with:

- `linux-x64` for Linux
- `osx-x64` for macOS (Intel)
- `osx-arm64` for macOS (Apple Silicon)

## Development Setup

### Prerequisites for development:

- Visual Studio Code, Visual Studio 2022, or JetBrains Rider
- .NET 10 SDK
- Avalonia extension for your IDE (recommended)

### Project file dependencies:

The presentation layer (`GodotMan.App`) references:

- `GodotMan.Domain`
- `GodotMan.Services`
- `GodotMan.Infrastructure`

### Key NuGet packages:

- **Avalonia**: Cross-platform UI framework
- **Avalonia.ReactiveUI**: MVVM integration for Avalonia
- **ReactiveUI**: Reactive MVVM framework
- **Microsoft.Extensions.Hosting**: Dependency injection & host configuration
- **Microsoft.Extensions.Logging**: Structured logging
- **Octokit**: GitHub API client (for fetching release data)

## Usage

### Main Window

The main window contains:

1. **Navigation Sidebar**: Switch between main sections
   - Releases
   - Installed Versions

2. **Tab Bar**: Quick access to variant tabs (Standard / Mono)

3. **Main Content Area**: Displays the current view

### Releases View

- Browse available Godot Engine releases
- Filter by variant (Standard or Mono)
- Toggle pre-release versions
- View release details
- Install selected releases

### Installed View

- View all installed Godot Engine versions
- Launch an installed version
- Set a default version
- Uninstall versions
- Track installation status

### Download Progress

- Monitor active downloads
- View download speed and ETA
- Cancel downloads if needed

## Data Storage

Downloaded files and installation data are stored in:

- **Windows**: `%LocalAppData%\Normitech\GodotMan`
- **macOS**: `~/Library/Application Support/Normitech/GodotMan`
- **Linux**: `~/.local/share/Normitech/GodotMan`

## Technology Stack

- **UI Framework**: Avalonia 12.x
- **MVVM Pattern**: ReactiveUI
- **Reactive Programming**: System.Reactive
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **API Client**: Octokit (GitHub API)
- **Logging**: Microsoft.Extensions.Logging
- **Language**: C# 14 with nullable reference types enabled

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request

### Code Standards

- Follow C# naming conventions (PascalCase for public members, camelCase for private)
- Enable nullable reference types in all projects
- Add XML documentation comments for public APIs
- Write unit tests for business logic
- Use dependency injection for all services

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Issues & Feature Requests

Found a bug? Have a feature request? Please open an issue on GitHub with:

- A clear description
- Steps to reproduce (for bugs)
- Expected vs. actual behavior
- Screenshots (if applicable)
- Your environment (OS, .NET version)

## Roadmap

Future enhancements:

- [ ] Custom installation paths per version
- [ ] Project templates browser
- [ ] Direct project opening from Godot versions
- [ ] Version comparison tool
- [ ] Automatic update checking
- [ ] Settings/Preferences panel
- [ ] Plugin/extension manager
- [ ] Dark/Light theme toggle

## Support

For issues, questions, or suggestions, please:

1. Check existing GitHub issues
2. Review the documentation
3. Open a new GitHub issue with detailed information

## Authors

- **Your Name** - Initial creator

## Acknowledgments

- Godot Engine community
- Avalonia UI team
- ReactiveUI community
