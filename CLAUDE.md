# CLAUDE.md - AI Assistant Guide for Intersect Engine

## Project Overview

**Intersect Engine** is a complete 2D MMORPG game development suite built on MonoGame with .NET 8. It provides a client-server architecture with an integrated game editor, enabling developers to create multiplayer games without programming experience, while remaining extensible for advanced developers.

**Key Facts:**
- **Language**: C# (.NET 8)
- **License**: Split licensing (MIT for client/framework, GPLv3 for server/editor)
- **Architecture**: Client-Server with embedded single-player mode
- **Primary Platforms**: Windows (editor), Linux, macOS (client/server)
- **Current Version**: 0.8.0-beta
- **Repository**: https://github.com/AscensionGameDev/Intersect-Engine

## Table of Contents

1. [Codebase Architecture](#codebase-architecture)
2. [Project Structure](#project-structure)
3. [Development Environment Setup](#development-environment-setup)
4. [Build System](#build-system)
5. [Git Workflow and Branching](#git-workflow-and-branching)
6. [Code Conventions](#code-conventions)
7. [Development Workflows](#development-workflows)
8. [Testing](#testing)
9. [Common Tasks](#common-tasks)
10. [Key Design Patterns](#key-design-patterns)
11. [Important File Locations](#important-file-locations)
12. [Pull Request Guidelines](#pull-request-guidelines)

---

## Codebase Architecture

### Multi-Tier Modular Architecture

```
┌─────────────────────────────────────────────────────────┐
│                   Application Layer                      │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Client.exe   │  │ Server.exe   │  │ Editor.exe   │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                 Implementation Layer                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Client.Core  │  │ Server.Core  │  │ Editor.Core  │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                   Framework Layer                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │Client.Frmwrk │  │Server.Frmwrk │  │   Network    │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                     Core Layer                           │
│  ┌──────────────┐  ┌──────────────────────────────────┐ │
│  │Intersect.Core│  │ Framework.Core (Game Objects)    │ │
│  └──────────────┘  └──────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

### Primary Projects and Dependencies

| Project | License | Purpose | Key Dependencies |
|---------|---------|---------|------------------|
| **Intersect.Core** | MIT | Application lifecycle, networking, plugins | - |
| **Framework.Core** | MIT | Game object definitions, shared types | Intersect.Core |
| **Intersect.Network** | MIT | LiteNetLib wrapper, encryption | Intersect.Core |
| **Client.Framework** | MIT | Client public API, graphics abstractions | Framework.Core |
| **Client.Core** | MIT | MonoGame rendering, UI (Gwen), input | Client.Framework |
| **Client** | MIT | Client executable | Client.Core |
| **Server.Framework** | GPLv3 | Server public API | Framework.Core |
| **Server.Core** | GPLv3 | Game logic, database (EF Core), entities | Server.Framework |
| **Server** | GPLv3 | Server executable, web dashboard | Server.Core |
| **Editor** | GPLv3 | Map/content editor (Windows/DirectX only) | Client.Framework, Server.Framework |
| **SinglePlayer** | MIT | Embedded server for offline play | Client.Core, Server.Core |

### Architectural Patterns in Use

1. **Service-Oriented Architecture**: Core application services (plugins, networking, database) managed through `IApplicationService` lifecycle
2. **Factory Pattern**: `FactoryRegistry<T>` for dependency injection
3. **Observer Pattern**: Event-driven packet handling via `PacketDispatcher`
4. **Plugin Architecture**: Dynamic plugin loading with isolated contexts
5. **Entity Component System**: Inheritance-based entity hierarchy (Player, NPC, Projectile, Resource)
6. **Repository Pattern**: Entity Framework Core for data access
7. **Command Pattern**: Network packets as commands
8. **Template Method**: `ApplicationService` base class defines lifecycle

---

## Project Structure

### Directory Layout

```
Intersect-Engine/
├── .github/              # GitHub Actions workflows
│   └── workflows/        # build.yml, pull_request.yml
├── Documentation/        # Extended features documentation
├── Examples/             # Plugin examples
│   ├── Plugin.Server/    # Server plugin example
│   ├── Plugin.Client/    # Client plugin example
│   └── Plugin.Multitarget/ # Multi-target plugin example
├── Framework/            # Shared framework layer
│   ├── Framework.Core/   # Game objects, packets, serialization
│   ├── Framework/        # Framework implementation
│   └── Framework.Multitarget/ # Cross-platform targeting
├── Intersect (Core)/     # Core infrastructure (note the space!)
│   ├── Core/             # Application lifecycle, services
│   ├── Network/          # Networking abstractions
│   ├── Plugins/          # Plugin system
│   └── Serialization/    # JSON/binary serialization
├── Intersect.Network/    # LiteNetLib implementation
├── Intersect.Client.Framework/ # Client API, graphics, UI (Gwen)
├── Intersect.Client.Core/      # Client implementation
├── Intersect.Client/           # Client executable
├── Intersect.Server.Framework/ # Server API
├── Intersect.Server.Core/      # Server implementation, database
├── Intersect.Server/           # Server executable, web UI
├── Intersect.Editor/           # Map/game editor
├── Intersect.SinglePlayer/     # Single-player mode
├── Intersect.Tests/            # Core tests
├── Intersect.Tests.*/          # Component-specific tests
├── Utilities/            # Helper tools (port checker)
├── assets/               # Submodule: Intersect-Assets
├── scripts/              # Build and maintenance scripts
├── targets/              # MSBuild target files
└── vendor/               # Third-party dependencies
```

### Key Namespaces

- `Intersect.Core` - Application runtime, services, networking
- `Intersect.Framework.Core` - Game objects (Items, NPCs, Maps, Events, etc.)
- `Intersect.Network` - Network transport layer
- `Intersect.Server.Core.Entities` - Server-side entity implementations
- `Intersect.Server.Core.Database` - Database models and migrations
- `Intersect.Client.Core.Entities` - Client-side entity view models
- `Intersect.Client.Framework.Gwen` - UI framework
- `Intersect.Client.Framework.Graphics` - Rendering abstractions
- `Intersect.Plugins` - Plugin infrastructure

---

## Development Environment Setup

### Required Dependencies

1. **.NET 8 SDK** (tested with 8.0.405)
   ```bash
   dotnet --version  # Should be 8.0.x
   ```

2. **Git** (for submodules)
   ```bash
   git --version  # Tested with 2.47.1
   ```

3. **IDE** (recommended):
   - Visual Studio 2022+ (Windows)
   - Visual Studio Code with C# extension
   - JetBrains Rider

### Initial Setup

#### All Platforms
```bash
# Clone repository
git clone https://github.com/AscensionGameDev/Intersect-Engine.git
cd Intersect-Engine

# Initialize submodules (required for assets)
git submodule update --init --recursive
```

#### Non-Windows Platforms (Linux/macOS)
```bash
# Apply patch to disable Windows-only projects (Editor)
git apply disable-windows-only.patch

# Before updating from upstream:
git apply -R disable-windows-only.patch  # Revert patch first
git pull
git apply disable-windows-only.patch     # Reapply
```

### Restore Dependencies
```bash
dotnet restore Intersect.sln
```

---

## Build System

### Build Configurations

- **Debug** - Development builds, no single-file output
- **Release** - Production builds, single-file binaries
- **DebugTests** - Test-specific debug build
- **DebugFull** - Full debug symbols
- **DebugPlugins** - Plugin development debugging

### Common Build Commands

#### Development Build
```bash
dotnet build -p:Configuration=Debug \
             -p:PackageVersion=0.8.0-beta \
             -p:Version=0.8.0
```

#### Release Build
```bash
dotnet build -p:Configuration=Release \
             -p:PackageVersion=0.8.0-beta \
             -p:Version=0.8.0
```

#### Platform-Specific Publish
```bash
# Linux
dotnet publish -p:Configuration=Release \
               -p:PackageVersion=0.8.0-beta \
               -p:Version=0.8.0 \
               -r linux-x64

# macOS
dotnet publish -r osx-x64 -p:Configuration=Release

# Windows
dotnet publish -r win-x64 -p:Configuration=Release
```

### Runtime Identifiers
- `linux-x64`, `linux-arm64`
- `osx-x64`, `osx-arm64`
- `win-x64`

### Build Properties

Key MSBuild properties (see `Common.props`, `Intersect.props`):
- `PackageVersion` - NuGet package version
- `Version` - Assembly version
- `Configuration` - Build configuration
- `TargetFramework` - Always `net8.0`
- `LangVersion` - Always `latest`
- `Nullable` - Always `enable`
- `ImplicitUsings` - Always `enable`

### Important Build Notes

- **Network Keys**: Keys are generated during first build of `Intersect.Network` and cached in `Intersect.Network/bin/Release/keys/`
- **Single-File Binaries**: Only created in Release builds via `dotnet publish`
- **Self-Contained**: Use `--sc` flag for self-contained deployments (includes runtime)

---

## Git Workflow and Branching

### Branch Strategy

| Branch | Version | Purpose | Allowed Changes |
|--------|---------|---------|-----------------|
| `main` | 0.8.0.x | Stable releases | Bug fixes (non-breaking), markdown docs, GitHub automation |
| `prerelease` | 0.7.x.y | Release candidates | Non-breaking fixes and features |
| `development` | 0.8.x.y | Active development | Breaking changes permitted |

### Versioning

- **Pattern**: `<major>.<minor>.<patch>.<build>`
- **Pre-1.0**: `0.<major>.<minor>.<build>`
- **Semantic Versioning**: Breaking changes increment major/minor
- **Breaking Changes**: Any change that alters/removes field, property, method, or class signatures, or alters/deletes assemblies

### Commit Signing

- **Required for `main` branch**: All commits MUST be GPG signed
- **Strongly recommended**: Sign all commits on all branches
- Setup: https://docs.github.com/en/authentication/managing-commit-signature-verification/signing-commits

### Commit Message Format

Use lowercase except for names (classes, etc.):

```
feat: <what was added>
fix: <what is fixed>
chore: <what was done>
```

**Examples:**
- `feat: add server selection interface`
- `fix: added null-check to prevent crash`
- `fix: removed outdated expectation in TestTryAddFriend`
- `chore: documented configuration options`
- `chore: added tests for MathHelper`
- `chore: resolved null reference warnings`

### Commit Quality Guidelines

**Do:**
- Commit test code separately and after the feature code (same PR)
- Add documentation in the same commit as new code
- Commit immediately after renames, file creation/deletion
  - `chore: renamed ClassA to ClassB`
  - `chore: separated class declaration into separate file`
- Commit immediately after running formatting tools (ensure it compiles first)
  - `chore: formatting`

**Don't:**
- Commit bug fix + new feature code together (separate commits)
- Commit multiple separate bug fixes in one commit
- Commit hand-modified + tool-modified code together

---

## Code Conventions

### C# Style (.editorconfig)

#### Indentation
- **C# files**: 4 spaces
- **XML files**: 2 spaces
- **JSON/YAML**: 2 spaces
- **Use spaces, not tabs** for C# code

#### Line Endings
- **LF** (`\n`) for all files
- **UTF-8** encoding

#### Naming Conventions
- **PascalCase** for constant fields
- **No `this.` qualification** unless required for disambiguation
- **Prefer language keywords** over BCL types (`int` vs `Int32`)

#### Expression Style
- **var** preferred for built-in types when type is apparent
- **Expression-bodied members** for properties, indexers, accessors
- **Block bodies** for methods, constructors, operators
- **Pattern matching** over `is` with cast check
- **Braces** always required for control flow

#### Formatting
- **New line before open brace** (Allman style)
- **New line before** `else`, `catch`, `finally`
- **Space after keywords** in control flow statements
- **Space around binary operators**
- **No space after cast**

#### Modern C# Features
- **File-scoped namespaces** preferred (C# 10+)
- **Nullable reference types** enabled project-wide
- **Implicit usings** enabled
- **Latest C# language version** (`LangVersion: latest`)

### Example Code Style

```csharp
namespace Intersect.Server.Entities;

public class Player : Entity
{
    private readonly List<Item> _inventory;

    public string Name { get; set; }

    public int Level => CalculateLevel();

    public Player(Guid id)
    {
        _inventory = new List<Item>();
        Id = id;
    }

    private int CalculateLevel()
    {
        if (Experience < 100)
        {
            return 1;
        }

        return (int)Math.Floor(Experience / 100.0) + 1;
    }
}
```

### ReSharper Settings

- `resharper_csharp_wrap_arguments_style = chop_if_long`
- `resharper_csharp_wrap_parameters_style = chop_if_long`
- `resharper_max_invocation_arguments_on_line = 5`
- `resharper_trailing_comma_in_multiline_lists = true`

---

## Development Workflows

### Network Packet Development

**Location**: `Framework/Intersect.Framework.Core/Network/Packets/`

1. **Create packet class** inheriting from `IntersectPacket`
   ```csharp
   [MessagePackObject]
   public class ExamplePacket : IntersectPacket
   {
       [Key(0)]
       public string Message { get; set; }

       [Key(1)]
       public int Value { get; set; }
   }
   ```

2. **Register in plugin** (if plugin-based):
   ```csharp
   context.Packet.TryRegisterPacketType<ExamplePacket>();
   ```

3. **Create packet handler**:
   ```csharp
   public class ExamplePacketHandler : IPacketHandler<ExamplePacket>
   {
       public void Handle(IConnection connection, ExamplePacket packet)
       {
           // Handle packet
       }
   }
   ```

4. **Register handler**:
   ```csharp
   context.Packet.TryRegisterPacketHandler<ExamplePacketHandler, ExamplePacket>();
   ```

### Plugin Development

**Example Location**: `Examples/Intersect.Examples.Plugin.Server/`

1. **Create plugin entry class**:
   ```csharp
   public class MyServerPlugin : ServerPluginEntry
   {
       public override void OnBootstrap(IPluginBootstrapContext context)
       {
           // Register packet types, configure services
           context.Logging.Application.Info("Plugin bootstrapping...");
       }

       public override void OnStart(IServerPluginContext context)
       {
           // Start services, register handlers
           context.Logging.Plugin.Info("Plugin started!");
       }

       public override void OnStop(IServerPluginContext context)
       {
           // Cleanup resources
       }
   }
   ```

2. **Create manifest** (plugin.json or implement `IManifestHelper`):
   ```json
   {
       "Name": "My Plugin",
       "Key": "my-plugin",
       "Version": "1.0.0",
       "Authors": ["Your Name"],
       "Homepage": "https://example.com"
   }
   ```

3. **Build and deploy**:
   ```bash
   dotnet build -p:Configuration=Release
   # Copy output to resources/plugins/
   ```

### Database Model Changes

**Location**: `Intersect.Server.Core/Database/`

1. **Add/modify entity model** in appropriate folder:
   - `PlayerData/Players/` - Player-related models
   - `PlayerData/Guilds/` - Guild models
   - `GameData/` - Static game data

2. **Create migration**:
   ```bash
   dotnet ef migrations add YourMigrationName --project Intersect.Server.Core
   ```

3. **Apply migration** (automatic on server start, or manual):
   ```bash
   dotnet ef database update --project Intersect.Server.Core
   ```

4. **Thread safety**: Use `DbInterface.Pool` for async database operations

### UI Development (Client)

**Location**: `Intersect.Client.Framework/Gwen/`

1. **Create control** inheriting from Gwen base:
   ```csharp
   public class MyCustomControl : Base
   {
       public MyCustomControl(Base parent) : base(parent)
       {
           // Initialize control
       }

       protected override void Render(SkinBase skin)
       {
           // Custom rendering
       }
   }
   ```

2. **Use data binding**:
   ```csharp
   var dataProvider = new DataProvider<string>();
   dataProvider.Value = "Hello";
   myLabel.SetDataBinding(dataProvider);
   ```

3. **Load textures**:
   ```csharp
   var texture = context.Content.Load<GameTexture>(
       ContentType.Texture,
       "path/to/texture.png",
       "texture-alias"
   );
   ```

---

## Testing

### Test Projects

- `Intersect.Tests` - Core framework tests
- `Intersect.Tests.Network` - Network layer tests
- `Intersect.Tests.Client` - Client tests
- `Intersect.Tests.Client.Framework` - Client framework tests
- `Intersect.Tests.Server` - Server logic tests
- `Intersect.Tests.Editor` - Editor tests
- `Examples/Plugin.Client.Tests` - Plugin testing example

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific project tests
dotnet test Intersect.Tests.Server/Intersect.Tests.Server.csproj

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

### Test Configuration

- **Configuration**: `DebugTests` for test-specific builds
- **Framework**: xUnit, NUnit, or MSTest (check individual projects)
- **Mocking**: Moq or NSubstitute where applicable

### Test Commit Guidelines

- **Commit test code separately** from feature code
- **Commit after feature code** in the same PR
- **Test fixes** can be merged to any relevant branch
- **Test additions** should target `prerelease` or `development`

---

## Common Tasks

### Adding a New Game Object Type

1. **Define in Framework.Core**: `Framework/Intersect.Framework.Core/GameObjects/`
   ```csharp
   [MessagePackObject]
   public class NewGameObject : IGameObject
   {
       [Key(0)]
       public Guid Id { get; set; }

       [Key(1)]
       public string Name { get; set; }
   }
   ```

2. **Add database model** (if persistent): `Intersect.Server.Core/Database/GameData/`

3. **Create migration**: `dotnet ef migrations add AddNewGameObject`

4. **Add editor support**: `Intersect.Editor/` UI and logic

### Adding Server Configuration Options

1. **Edit Options class**: `Intersect (Core)/Configuration/Options.cs`
   ```csharp
   [JsonProperty]
   public NewOptionsSection NewSection { get; set; } = new();
   ```

2. **Document in code comments** for auto-generated docs

3. **Default configuration** created automatically on first run

### Implementing a New Entity Type

1. **Server-side**: Inherit from `Entity` in `Intersect.Server.Core/Entities/`
   ```csharp
   public class CustomEntity : Entity
   {
       public override void Update(long timeMs)
       {
           base.Update(timeMs);
           // Custom update logic
       }
   }
   ```

2. **Client-side**: Create view model in `Intersect.Client.Core/Entities/`

3. **Synchronization**: Add packet types for client-server sync

### Adding a New Content Type

1. **Define content type**: `Intersect.Client.Framework/Content/ContentType.cs`

2. **Update content manager**: Support loading new type

3. **Add asset organization**: Update directory structure in `resources/`

---

## Key Design Patterns

### Application Service Pattern

Base class for all services:

```csharp
public abstract class ApplicationService<TInterface, TImpl> : IApplicationService
    where TImpl : TInterface
{
    public void Bootstrap(IApplicationContext context) { }
    public void Start(IApplicationContext context) { }
    public void Stop() { }
}
```

**Lifecycle**: Bootstrap → Start → Run → Stop

### Factory Registry Pattern

```csharp
FactoryRegistry<IPluginContext>.RegisterFactory(new ServerPluginContext.Factory());
var context = FactoryRegistry<IPluginContext>.Create();
```

### Packet Dispatcher Pattern

```csharp
// Register handler
PacketDispatcher.RegisterHandler(typeof(MyPacket), HandleMyPacket);

// Dispatch
PacketDispatcher.Dispatch(connection, packet);
```

### Event-Driven Architecture

- **Packet events**: Pre-process and post-process hooks
- **Connection events**: OnConnected, OnDisconnected
- **Entity events**: Update, Spawn, Destroy
- **UI events**: Data provider change notifications

---

## Important File Locations

### Configuration Files

- **Editor Config**: `.editorconfig` - Code style rules
- **Build Properties**: `Common.props`, `Directory.Build.props`, `Intersect.props`
- **NuGet Config**: `NuGet.Config`
- **Git Attributes**: `.gitattributes` - Line ending rules
- **Git Ignore**: `.gitignore`
- **Submodules**: `.gitmodules` - Assets repository

### Documentation

- **README**: `README.md` - Project overview
- **Contributing**: `CONTRIBUTING.md` - Contribution guidelines
- **Versioning**: `VERSIONING.md` - Version strategy
- **Security**: `SECURITY.md` - Security policies
- **Requirements**: `REQUIREMENTS.md` - Platform support matrix
- **Features**: `Documentation/Features.md` - Extended features
- **Authors**: `AUTHORS.md` - Contributors list
- **License**: `LICENSE.md` - License information

### Build & CI/CD

- **GitHub Actions**: `.github/workflows/build.yml`, `.github/workflows/pull_request.yml`
- **Build Scripts**: `scripts/`
- **MSBuild Targets**: `targets/`
- **Bundles**: `.github/bundles/` - Package definitions

### Runtime Resources

- **Server Config**: `Intersect.Server/appsettings.json`
- **Network Keys**: `Intersect.Network/bin/Release/keys/`
- **Game Assets**: `assets/` (submodule)

---

## Pull Request Guidelines

### Before Creating a PR

1. **Associate with issue**: Start PR description with `Resolves #123`
2. **Rebase on target branch**: Keep PR up-to-date
3. **Resolve merge conflicts**: Author's responsibility
4. **Test thoroughly**: Provide screenshots/recordings
5. **Sign commits**: GPG signing required for `main`, recommended for all

### PR Quality Checklist

- [ ] One PR addresses one issue (exceptions: refactors, code quality)
- [ ] Commit messages follow format (`feat:`, `fix:`, `chore:`)
- [ ] Commits are coherent chunks that compile
- [ ] Tests added/updated and passing
- [ ] Documentation updated if needed
- [ ] No backwards-compatibility hacks for removed code
- [ ] Author listed in `AUTHORS.md` (first contribution)
- [ ] Supplementary materials provided (screenshots, recordings, logs)

### PR Title Format

Same as commit messages, but precedence: `feat` > `fix` > `chore`

**Examples:**
- `feat: add guild alliance system (#456)`
- `fix: prevent crash when loading invalid map data (#789)`
- `chore: improve test coverage for packet handling (#123)`

### Merge Strategy

- **Additive/Subtractive PRs**: Squash merge (default)
- **Promotion PRs**: Rebase merge (branch promotions only)
- **Reviews Required**: 1 contributor + 1 maintainer minimum
- **Checks**: Must pass all CI/CD checks

### Branch Targets

- **Features**: → `development` only
- **Bug fixes**: → any relevant branch
- **Test additions**: → `prerelease` or `development`
- **Test fixes**: → any relevant branch
- **Documentation (code)**: → `development` only
- **Documentation (markdown)**: → any relevant branch

---

## AI Assistant Best Practices

### When Working with This Codebase

1. **Read before modifying**: Always read files before editing them
2. **Respect licensing**: Remember split licensing (MIT vs GPLv3)
3. **Follow editorconfig**: Maintain code style consistency
4. **Test changes**: Build and run tests after modifications
5. **Keep context**: Note the directory name "Intersect (Core)" has a space
6. **Check platform**: Some features are Windows-only (Editor)
7. **Use proper paths**: Always use absolute paths for file operations
8. **Understand architecture**: Review this document's architecture section
9. **Plugin first**: Consider plugin-based solutions before core changes
10. **Backward compatibility**: Avoid breaking changes on stable branches

### Common Pitfalls to Avoid

- **Don't** modify `LICENSE.md` files without unanimous contributor consent
- **Don't** create backwards-compatibility shims (rename unused vars, etc.)
- **Don't** add features to `main` branch (bug fixes only)
- **Don't** commit unsigned code to `main`
- **Don't** forget to update submodules after clone
- **Don't** apply Windows-only patch on Windows systems
- **Don't** use interactive git commands (`git rebase -i`, `git add -i`)
- **Don't** guess at configuration values - read existing config first

### Debugging Tips

1. **Build output**: Check `bin/Debug/` or `bin/Release/`
2. **Network keys**: Regenerate if missing: rebuild `Intersect.Network`
3. **Database issues**: Check migrations in `Intersect.Server.Core/Migrations/`
4. **Asset loading**: Verify `assets/` submodule is initialized
5. **Platform errors**: Ensure correct patch applied (non-Windows)
6. **Plugin issues**: Check plugin manifest and bootstrap logging

### Useful Commands Reference

```bash
# Full clean build
dotnet clean
dotnet restore
dotnet build

# Run specific project
dotnet run --project Intersect.Server/Intersect.Server.csproj

# Watch mode (auto-rebuild on changes)
dotnet watch --project Intersect.Server/Intersect.Server.csproj

# Format code
dotnet format

# List projects
dotnet sln list

# Dependency graph
dotnet list package --include-transitive
```

---

## Additional Resources

- **Official Website**: https://freemmorpgmaker.com
- **Documentation**: https://docs.freemmorpgmaker.com
- **Community Forums**: https://ascensiongamedev.com
- **Discord**: https://discord.gg/Ggt3KJV
- **Issue Tracker**: https://github.com/AscensionGameDev/Intersect-Engine/issues
- **Assets Repository**: https://github.com/AscensionGameDev/Intersect-Assets
- **Downloads**: https://freemmorpgmaker.com/download

---

## Quick Reference: Key Interfaces

### Plugin Development
- `IPluginBootstrapContext` - Bootstrap-phase context
- `IPluginContext` / `IServerPluginContext` / `IClientPluginContext` - Runtime context
- `PluginEntry<TContext>` - Base plugin entry class
- `IManifestHelper` - Plugin manifest metadata

### Networking
- `IPacket` - Base packet interface
- `IntersectPacket` - MessagePack-serialized packet base
- `IConnection` - Network connection abstraction
- `IPacketHandler<TPacket>` - Packet handler interface

### Application Services
- `IApplicationService` - Service lifecycle interface
- `IApplicationContext` - Application runtime container
- `ApplicationService<TInterface, TImpl>` - Service base class

### Content Management
- `IContentManager` - Asset loading interface
- `ContentType` - Asset type enumeration
- `IGameTexture` - Texture interface

### Entity System
- `IEntity` - Entity interface (public API)
- `Entity` - Base entity implementation (server)
- `Player`, `Npc`, `Projectile`, `Resource` - Derived entity types

---

**Document Version**: 1.0
**Last Updated**: 2025-12-06
**Intersect Version**: 0.8.0-beta
**Maintained By**: AI Assistant Documentation System

For questions or clarifications about this guide, refer to the community forums or Discord.
