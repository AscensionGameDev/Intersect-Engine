using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;

namespace Intersect.Editor.Interface;

internal partial class EditorMainWindow
{
    [EditorMenu]
    private Menu CreateMenuFile()
    {
        var itemPreferences = new MenuItem
        {
            Name = Strings.MenuBar.File.Preferences,
        };

        var itemPackageGame = new MenuItem
        {
            Name = Strings.MenuBar.File.PackageGame,
        };

        var itemExit = new MenuItem
        {
            Name = Strings.MenuBar.File.Exit,
        };

        itemExit.Selected += (s, e) => Close();

        var menuFile = new Menu(Strings.MenuBar.File)
        {
            Items = new()
            {
                itemPreferences,
                itemPackageGame,
                itemExit
            }
        };

        return menuFile;
    }
}
