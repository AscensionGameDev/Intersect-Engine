using ImGuiNET;

using Intersect.Client.Framework.UserInterface.Styling;
using Intersect.Time;

namespace Intersect.Client.Framework.UserInterface.Components;

public class MenuItem
{
    public event EventHandler Selected;

    public bool Enabled { get; set; } = true;

    public string Name { get; set; }

    public string Shortcut { get; set; }

    internal void OnSelected() => Selected?.Invoke(this, EventArgs.Empty);
}

public class Menu
{
    private readonly List<MenuItem> _items;

    public Menu(string name)
    {
        _items = new List<MenuItem>();
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Menu name cannot be null, empty, or whitespace.", nameof(name));
    }

    public bool Enabled { get; set; } = true;

    public List<MenuItem> Items
    {
        get => _items;
        set
        {
            if (value == _items)
            {
                return;
            }

            _items.Clear();

            if (value != default)
            {
                _items.AddRange(value);
            }
        }
    }

    public string Name { get; }
}

public class MenuBar : Component
{
    public MenuBar()
    {
        Display = DisplayMode.Block;
        Menus = new();
    }

    public List<Menu> Menus { get; }

    protected override bool DrawBegin()
    {
        return ImGui.BeginMainMenuBar();
    }

    protected override void DrawEnd()
    {
        ImGui.EndMainMenuBar();
    }

    protected override void DrawBehindChildren(FrameTime frameTime)
    {
        foreach (var menu in Menus)
        {
            if (ImGui.BeginMenu(menu.Name, menu.Enabled))
            {
                foreach (var menuItem in menu.Items)
                {
                    if (ImGui.MenuItem(menuItem.Name, menuItem.Shortcut, false, menuItem.Enabled))
                    {
                        menuItem.OnSelected();
                    }
                }
            }

            ImGui.EndMenu();
        }
    }
}
