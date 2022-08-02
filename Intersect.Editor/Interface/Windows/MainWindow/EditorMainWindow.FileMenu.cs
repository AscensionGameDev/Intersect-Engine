using System.Diagnostics;

using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Localization;
using Intersect.Platform;

using Newtonsoft.Json;

namespace Intersect.Editor.Interface.Windows.MainWindow;

internal partial class EditorMainWindow
{
    [EditorMenu(Order = 1)]
    private Menu CreateMenuFile()
    {
        var itemPreferences = new MenuItem
        {
            Name = Strings.MenuBar.File.Preferences,
        };

        itemPreferences.Selected += (_, _) => { };

        var itemPackageGame = new MenuItem
        {
            Name = Strings.MenuBar.File.PackageGame,
        };

        var itemExit = new MenuItem
        {
            Name = Strings.MenuBar.File.Exit,
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
        var menu = new Menu(Strings.MenuBar.ContentEditors)
        {

        };

        return menu;
    }

    [EditorMenu(Order = -1)]
    private Menu CreateMenuHelp()
    {
        var itemAbout = new MenuItem
        {
            Name = Strings.MenuBar.Help.About,
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
