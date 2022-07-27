using System.Collections;

using ImGuiNET;

using Intersect.Client.Framework.UserInterface.Styling;
using Intersect.Localization;
using Intersect.Time;

namespace Intersect.Client.Framework.UserInterface.Components;

public class MenuItem
{
    public event EventHandler Selected;

    public bool Enabled { get; set; } = true;

    public LocalizedString Name { get; set; }

    public LocalizedString Shortcut { get; set; }

    internal void OnSelected() => Selected?.Invoke(this, EventArgs.Empty);
}

public class Menu
{
    private readonly List<MenuItem> _items;

    public Menu(LocalizedString name)
    {
        _items = new List<MenuItem>();
        Name = name ?? throw new ArgumentNullException(nameof(name));
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

    public LocalizedString Name { get; set; }
}

public class MenuBar : Component, IList<Menu>
{
    private readonly List<Menu> _menus;

    public MenuBar()
    {
        _menus = new();

        Display = DisplayMode.Block;
    }

    public Menu this[int index] {
        get => _menus[index];
        set => _menus[index] = value;
    }

    public int Count => _menus.Count;

    public bool IsReadOnly => (_menus as IList<Menu>).IsReadOnly;

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        if (!ImGui.BeginMainMenuBar())
        {
            return false;
        }

        foreach (var menu in _menus)
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

        return true;
    }

    protected override void LayoutEnd(FrameTime frameTime)
    {
        ImGui.EndMainMenuBar();
    }

    public void Add(Menu item) => _menus.Add(item);

    public void AddRange(IEnumerable<Menu> items) => _menus.AddRange(items);

    public void Clear() => _menus.Clear();

    public bool Contains(Menu item) => _menus.Contains(item);

    public void CopyTo(Menu[] array, int arrayIndex) => _menus.CopyTo(array, arrayIndex);

    public IEnumerator<Menu> GetEnumerator() => _menus.GetEnumerator();

    public int IndexOf(Menu item) => _menus.IndexOf(item);

    public void Insert(int index, Menu item) => _menus.Insert(index, item);

    public bool Remove(Menu item) => _menus.Remove(item);

    public void RemoveAt(int index) => _menus.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
