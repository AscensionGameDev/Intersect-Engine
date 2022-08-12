using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Platform;

namespace Intersect.Editor.Interface.Windows.MainWindow;

internal partial class EditorMainWindow
{
    [EditorMenu(Order = 1)]
    private Menu CreateMenuFile()
    {
        var itemPreferences = new MenuItem
        {
            Name = Strings.Windows.Preferences.Title,
        };

        itemPreferences.Selected += (_, _) =>
        {
            var isAlreadyOpen = FindWindows<PreferencesWindow>().Any();
            if (!isAlreadyOpen)
            {
                Components.Add(new PreferencesWindow());
            }
        };

        var itemPackageGame = new MenuItem
        {
            Name = Strings.MenuBar.File.PackageGame,
        };

        var itemExit = new MenuItem
        {
            Name = Strings.General.Exit,
        };

        itemExit.Selected += (_, _) =>
        {
            Close(); 
        };

        var menu = new Menu(Strings.MenuBar.File)
        {
            Items = new()
            {
                itemPreferences,
                itemPackageGame,
                itemExit
            }
        };

        return menu;
    }

    [EditorMenu(Order = 2)]
    private Menu CreateMenuEdit()
    {
        var itemUndo = new MenuItem
        {
            Name = Strings.MenuBar.Edit.Undo,
        };

        itemUndo.Selected += (_, _) => { };

        var itemRedo = new MenuItem
        {
            Name = Strings.MenuBar.Edit.Redo,
        };

        itemRedo.Selected += (_, _) => { };

        var itemCut = new MenuItem
        {
            Name = Strings.MenuBar.Edit.Cut,
        };

        itemCut.Selected += (_, _) => { };

        var itemCopy = new MenuItem
        {
            Name = Strings.MenuBar.Edit.Copy,
        };

        itemCopy.Selected += (_, _) => { };

        var itemPaste = new MenuItem
        {
            Name = Strings.MenuBar.Edit.Paste,
        };

        itemPaste.Selected += (_, _) => { };

        var menu = new Menu(Strings.MenuBar.Edit)
        {
            Items = new()
            {
                itemUndo,
                itemRedo,
                new MenuItemSeparator(),
                itemCut,
                itemCopy,
                itemPaste,
            }
        };

        return menu;
    }

    [EditorMenu(Order = 3)]
    private Menu CreateMenuView()
    {
        var menu = new Menu(Strings.MenuBar.View)
        {

        };

        return menu;
    }

    [EditorMenu(Order = 4)]
    private Menu CreateMenuSelection()
    {
        var menu = new Menu(Strings.MenuBar.Selection)
        {

        };

        return menu;
    }

    [EditorMenu(Order = 5)]
    private Menu CreateMenuContentEditors()
    {
        var menuItemLocalization = new MenuItem
        {
            Name = Strings.Windows.Localization.Title,
        };

        menuItemLocalization.Selected += (_, _) =>
        {
            var isAlreadyOpen = FindWindows<LocalizationWindow>().Any();
            if (!isAlreadyOpen)
            {
                Components.Add(new LocalizationWindow());
            }
        };

        var items = new List<MenuItem>
        {
            menuItemLocalization,
            new MenuItemSeparator(),
        };

        foreach (var descriptorType in Enum.GetValues<GameObjectType>().OrderBy(value => value.ToString()))
        {
            var menuItem = new MenuItem
            {
                Name = Strings.Descriptors.Names[descriptorType].Plural,
            };

            menuItem.Selected += (_, _) =>
            {
                var isAlreadyOpen = FindWindows<DescriptorWindow>().Any(
                    descriptorWindow => descriptorWindow.DescriptorType == descriptorType
                );
                if (!isAlreadyOpen)
                {
                    Components.Add(new DescriptorWindow(descriptorType));
                }
            };

            items.Add(menuItem);
        }

        var menu = new Menu(Strings.MenuBar.ContentEditors)
        {
            Items = items,
        };

        return menu;
    }

    [EditorMenu(Order = -1)]
    private Menu CreateMenuHelp()
    {
        var itemAbout = new MenuItem
        {
            Name = Strings.Windows.About.Title,
        };

        itemAbout.Selected += (_, _) => Components.Add(new AboutWindow());

        var itemDocumentation = new MenuItem
        {
            Name = Strings.MenuBar.Help.Documentation,
        };

        itemDocumentation.Selected += (_, _) => WebBrowser.OpenInDefaultBrowser("https://docs.freemmorpgmaker.com/");

        var itemPostQuestion = new MenuItem
        {
            Name = Strings.MenuBar.Help.PostQuestion,
        };

        itemPostQuestion.Selected += (_, _) => WebBrowser.OpenInDefaultBrowser("https://www.ascensiongamedev.com/community/forum/53-questions-and-answers/");

        var itemReportBug = new MenuItem
        {
            Name = Strings.MenuBar.Help.ReportBug,
        };

        itemReportBug.Selected += (_, _) => WebBrowser.OpenInDefaultBrowser("https://github.com/AscensionGameDev/Intersect-Engine/issues/new/choose");

        var menu = new Menu(Strings.MenuBar.Help)
        {
            Items = new()
            {
                itemAbout,
                itemDocumentation,
                new MenuItemSeparator(),
                itemPostQuestion,
                itemReportBug,
            }
        };

        return menu;
    }
}
