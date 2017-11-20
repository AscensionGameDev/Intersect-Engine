# Intersect Engine

Intersect is a free 2D MMORPG Maker. This game engine provides an intuitive and user friendly environmnent to create 2D ORPGs with no programming knowledge required. Source code is available here so there are absolutely no limitations to the games you can create. More information and screenshots can be found at our homepage - [https://www.freemmorpgmaker.com](https://www.freemmorpgmaker.com).

## Getting Started

Getting started with Intersect couldn't be easier. Make sure you have Visual Studio 2017 Community (or later) installed. Open the solution files found in the Client, Editor, and Server folders to build each of the major components of our engine. We have [documentation here](https://www.freemmorpgmaker.com/docs/en/Welcome.html) that will guide you through creating an admin account and designing your game.

### Prerequisites

```
.Net Framework 4.5 (or Mono on Linux/OSX)
MonoGame 3.7 (Developers only, included in repository)
```

### Installing

```
Open the solutions provided in the Client/Editor/Server folders and hit build. 
No special installation steps are required.
```

## Built With

* [MonoGame]() [(Ms-Pl)](https://github.com/MonoGame/MonoGame/blob/develop/LICENSE.txt) for Client/Editor graphic rendering and audio playback  
* [DarkUI](http://www.darkui.com/) [(LGPL 3)](https://github.com/RobinPerris/DarkUI/blob/master/LICENSE) for Editor form control styling  
* [DockPanelSuite](http://dockpanelsuite.com/) [(MIT)](https://github.com/dockpanelsuite/dockpanelsuite/blob/master/license.txt) for Editor user interface docking elements  
* [Gwen DotNet](https://code.google.com/archive/p/gwen-dotnet/) [(MIT)](http://www.opensource.org/licenses/mit-license.php) for Client user interface  
* [PngCs](https://github.com/tommyettinger/pngcs) [(Apache)](https://github.com/tommyettinger/pngcs/blob/master/LICENSE.txt) for Editor world screenshots  
* [Mono.Data.Sqlite](https://www.nuget.org/packages/Mono.Data.Sqlite.Portable) & [Sqlite3](https://www.sqlite.org/) for Server database and Editor map caching 

## Authors

* **[JC Snider](https://github.com/jcsnider)**
* **[Joe Bridges](https://github.com/irokaiser)**

Special thanks to [Robert Lodico](https://github.com/lodicolo) for all of his contributions and helping us get Intersect to release quicker than expected!

## License

This project has split licensing. The Client/Library are licensed MIT. The Editor/Server/Migration tool are licensed GPL. See license.md within each of the projects' folders.

## Acknowledgments

* Special thanks to Robin for his DarkUI library and various consultations during development of this engine. 
