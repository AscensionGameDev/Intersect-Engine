# Intersect Engine

Intersect provides a complete game development suite for creating 2d mmorpgs with no programming experience or difficult setup steps required! Intersect is powered by [MonoGame](http://monogame.net), and has been designed with stability and performance in mind. Intersect comes with [custom assets](https://github.com/AscensionGameDev/Intersect-Assets) that are free to use in your projects (even commercially) meaning that you can start developing your game in minutes!

[![Home https://freemmorpgmaker.com](https://img.shields.io/badge/Home-Free%20MMORPG%20Maker-informational)](https://freemmorpgmaker.com) 
[![Docs https://docs.freemmorpgmaker.com](https://img.shields.io/badge/Docs-Online-success)](https://docs.ascensiongamedev.com) 
[![Visit us at https://ascensiongamedev.com](https://img.shields.io/badge/Community-Ascension%20Game%20Dev-orange)](https://ascensiongamedev.com) 
[![Join the chat at https://discord.gg/Ggt3KJV](https://img.shields.io/discord/363106200243535872?color=%237289DA&label=Discord&logoColor=white)](https://discord.gg/Ggt3KJV)

 * [Build Status](#build-status)
 * [Supported Platforms](#supported-platforms)
 * [Intersect Assets](#interset-assets)
 * [Support and Contributions](#support-and-contributions)
 * [Source Code](#source-code)
 * [Licensing](#licensing)
 * [Helpful Links](#helpful-links)


## Build Status

Our [build server](https://teamcity.freemmorpgmaker.com/?guest=1) builds, tests, and packages the latest Intersect changes.  The table below shows the current build status for the master and development branches.

| Name  | Status |
|:---|--------|
| Master (Stable) | [![Build Status](https://teamcity.freemmorpgmaker.com/app/rest/builds/buildType:stable/statusIcon)](https://teamcity.freemmorpgmaker.com/viewType.html?buildTypeId=stable&guest=1) |
| Prerelease (Nightly) | [![Build Status](https://teamcity.freemmorpgmaker.com/app/rest/builds/buildType:prerelease/statusIcon)](https://teamcity.freemmorpgmaker.com/viewType.html?buildTypeId=prerelease&guest=1) |


## Supported Platforms

 * Desktop PCs (Open GL)
    * Windows
    * Mac OS X
    * Linux

Our editor uses DirectX and must be ran in Windows, but you can host and play your game on any desktop os that supports OpenGL.

We're open to expanding to new platforms (mobile, web, etc) but don't have the capacity to do so at this time. If you're interested in helping out let us know!


## Intersect Assets

Intersect is distributed with [custom assets](https://github.com/AscensionGameDev/Intersect-Assets) that have been curated from our community and around the net. Assets all match in style and are all safe to use freely in commercial projects. 


## Support and Contributions

If you think you have found a bug or have a feature request, use our [issue tracker](https://github.com/AscensionGameDev/Intersect-Engine/issues). Before opening a new issue, please search to see if your problem has already been reported.  Try to be as detailed as possible in your issue reports.

If you need help using Intersect or have other questions we suggest you post on our [community forums](https://ascensiongamedev.com).  Please do not use the GitHub issue tracker for personal support requests.

If you are interested in contributing fixes or features to Intersect, please read our [contributors guide](CONTRIBUTING.md) first.

### Donations

If you'd like to help the project by supporting us financially we accept and greatly appreciate donations at [Ascension Game Dev](https://www.ascensiongamedev.com/donate/make-donation/).


## Source Code

Getting started with Intersect couldn't be easier. Make sure you have Visual Studio 2019 Community installed. 

 * Clone the source: `git clone https://github.com/AscensionGameDev/Intersect-Engine.git`
 * Open Intersect.sln
 * Restore Nuget packages
 * Build

We have [documentation here](https://docs.freemmorpgmaker.com/developer/start/vs.html) tailored towards new developers that will guide you through installing visual studio, cloning the source, and getting started if you need more information.


## Licensing

This project has split licensing. See license.md within each of the projects' folders.

| Project  | License |
|:---|--------|
| Intersect.Core | [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://tldrlegal.com/license/mit-license) |
| Intersect.Client | [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://tldrlegal.com/license/mit-license) |
| Intersect.Client.Framework | [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://tldrlegal.com/license/mit-license) |
| Intersect.Network | [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://tldrlegal.com/license/mit-license) |
| Intersect.Shared | [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://tldrlegal.com/license/mit-license) |
| Intersect.Editor | [![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) |
| Intersect.Server | [![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) |
| Intersect.Server.Framework | [![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) |
| Intersect.Utilities | [![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) |

Third-party libraries used by Intersect are under their own licenses.  Please refer to those libraries for details on the license they use.


## Helpful Links

 * The official website is [freemmorpgmaker.com](https://freemmorpgmaker.com).
 * Our [issue tracker](https://github.com/AscensionGameDev/Intersect-Engine/issues) is on GitHub.
 * Use our [community forums](https://ascensiongamedev.com/) for support questions.
 * The [official documentation](https://docs.freemmorpgmaker.com) is on our website.
 * The official [Intersect Assets](https://github.com/AscensionGameDev/Intersect-Assets) repo is separate and can be found on GitHub.
 * Download stable and development [installers and packages](https://freemmorpgmaker.com/download).