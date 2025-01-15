# Intersect Engine

Intersect provides a complete game development suite for creating 2d mmorpgs with no programming experience or difficult setup steps required! Intersect is powered by [MonoGame](http://monogame.net), and has been designed with stability and performance in mind. Intersect comes with [custom assets](https://github.com/AscensionGameDev/Intersect-Assets) that are free to use in your projects (even commercially) meaning that you can start developing your game in minutes!

[![Home https://freemmorpgmaker.com](https://img.shields.io/badge/Home-Free%20MMORPG%20Maker-informational)](https://freemmorpgmaker.com)
[![Docs https://docs.freemmorpgmaker.com](https://img.shields.io/badge/Docs-Online-success)](https://docs.freemmorpgmaker.com)
[![main](https://github.com/AscensionGameDev/Intersect-Engine/actions/workflows/build.yml/badge.svg)](https://github.com/AscensionGameDev/Intersect-Engine/actions/workflows/build.yml)
[![Visit us at https://ascensiongamedev.com](https://img.shields.io/badge/Community-Ascension%20Game%20Dev-orange)](https://ascensiongamedev.com)
[![Join the chat at https://discord.gg/Ggt3KJV](https://img.shields.io/discord/363106200243535872?color=%237289DA&label=Discord&logoColor=white)](https://discord.gg/Ggt3KJV)

- [Intersect Engine](#intersect-engine)
	- [Automated Builds](#automated-builds)
	- [Supported Platforms](#supported-platforms)
	- [Compiling/Development](#compilingdevelopment)
		- [Dependencies](#dependencies)
			- [Required](#required)
		- [Getting started](#getting-started)
		- [Compiling](#compiling)
	- [Intersect Assets](#intersect-assets)
	- [Support and Contributions](#support-and-contributions)
	- [Source Code](#source-code)
	- [Licensing](#licensing)
	- [Helpful Links](#helpful-links)


## Automated Builds

We use [GitHub Actions](https://github.com/AscensionGameDev/Intersect-Engine/actions) for building and packaging the latest updates for the engine. Releases with downloadable bundles can be found on the [Releases page on our GitHub repository](https://github.com/AscensionGameDev/Intersect-Engine/releases).

## Supported Platforms

Please refer to the [Requirements](./REQUIREMENTS.md) document for support matrices.

Our editor uses DirectX and must be ran in Windows, but you can host and play your game on any desktop os that supports OpenGL.

We're open to expanding to new platforms (mobile, web, etc) but don't have the capacity to do so at this time. If you're interested in helping out let us know!

## Compiling/Development

### Dependencies

#### Required

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0), verified for v8.0.12 (SDK 8.0.405)
- git: required for cloning submodules, if you want to do this manually you don't need this and use the .zip download but that is not supported
  - Tested on 2.47.1

### Getting started

After cloning or updating,
- on all platforms run `git submodule update --init --recursive`
- on non-Windows platforms, additionally run `git apply disable-windows-editor.patch`

Before updating on non-Windows platforms, run `git apply -R disable-windows-editor.patch`

### Compiling

To compile either use an IDE or one of the following commands:
- `dotnet build -p:Configuration=Debug -p:PackageVersion=0.8.0-beta -p:Version=0.8.0`
  - Debug builds, all debug builds will _not_ create single-file binary outputs
- `dotnet build -p:Configuration=Release -p:PackageVersion=0.8.0-beta -p:Version=0.8.0`
	- Release builds will create single-file binary outputs but to get a clean output use `dotnet publish`
- `dotnet publish -p:Configuration=Release -p:PackageVersion=0.8.0-beta -p:Version=0.8.0 -r <runtime-id>`
	- e.g. `dotnet publish -p:Configuration=Release -p:PackageVersion=0.8.0-beta -p:Version=0.8.0 -r linux-x64`
	- The automated builds use the above command for the RIDs `linux-x64`, `osx-x64` and `win-x64`

### Extended Engine Features

For the extended engine features, please refer to the [Features](./Documentation/Features.md) documentation in the `Documentation` directory.

Extended engine features include things such as:
- [Built-in Server Website](./Documentation/Features.md#homepage)

## Intersect Assets

Intersect is distributed with [custom assets](https://github.com/AscensionGameDev/Intersect-Assets) that have been curated from our community and around the net. Assets all match in style and are all safe to use freely in commercial projects.


## Support and Contributions

If you think you have found a bug or have a feature request, use our [issue tracker](https://github.com/AscensionGameDev/Intersect-Engine/issues). Before opening a new issue, please search to see if your problem has already been reported.  Try to be as detailed as possible in your issue reports.

If you need help using Intersect or have other questions we suggest you post on our [community forums](https://ascensiongamedev.com).  Please do not use the GitHub issue tracker for personal support requests.

If you are interested in contributing fixes or features to Intersect, please read our [contributors guide](CONTRIBUTING.md) first.


## Source Code

Getting started with Intersect couldn't be easier. Make sure you have Visual Studio 2019 Community installed.

 * Clone the source: `git clone https://github.com/AscensionGameDev/Intersect-Engine.git`
 * Open Intersect.sln
 * Restore Nuget packages
 * Build

We have [documentation here](https://docs.freemmorpgmaker.com/developer/start/vs.html) tailored towards new developers that will guide you through installing visual studio, cloning the source, and getting started if you need more information.


## Licensing

This project has split licensing. See license.md within each of the projects' folders.

| Project                    | License                                                                                                                                      |
| :------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Intersect.Core             | [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://tldrlegal.com/license/mit-license)                               |
| Intersect.Client           | [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://tldrlegal.com/license/mit-license)                               |
| Intersect.Client.Framework | [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://tldrlegal.com/license/mit-license)                               |
| Intersect.Network          | [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://tldrlegal.com/license/mit-license)                               |
| Intersect.Editor           | [![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) |
| Intersect.Server           | [![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) |
| Intersect.Server.Framework | [![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) |
| Intersect.Utilities        | [![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) |

Third-party libraries used by Intersect are under their own licenses.  Please refer to those libraries for details on the license they use.


## Helpful Links

 * The official website is [freemmorpgmaker.com](https://freemmorpgmaker.com).
 * Our [issue tracker](https://github.com/AscensionGameDev/Intersect-Engine/issues) is on GitHub.
 * Use our [community forums](https://ascensiongamedev.com/) for support questions.
 * The [official documentation](https://docs.freemmorpgmaker.com) is on our website.
 * The official [Intersect Assets](https://github.com/AscensionGameDev/Intersect-Assets) repo is separate and can be found on GitHub.
 * Download stable and development [installers and packages](https://freemmorpgmaker.com/download).
