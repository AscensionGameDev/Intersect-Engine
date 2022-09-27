using System.Collections;
using ImGuiNET;
using Intersect.Localization;
using Intersect.Time;

namespace Intersect.Editor.Interface.Components;

public interface IHasMenuItems
{
    List<IMenuItem> Items { get; set; }
}

public interface IMenuItem { }

public class MenuItem : IHasMenuItems, IMenuItem
{
    private readonly List<IMenuItem> _items = new();

    public event EventHandler Selected;

    public bool Enabled { get; set; } = true;

    public List<IMenuItem> Items
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

    public LocalizedString Shortcut { get; set; }

    internal void OnSelected() => Selected?.Invoke(this, EventArgs.Empty);
}

public class MenuItemOption : MenuItem
{
    public bool Checked { get; set; }
}

public class MenuItemSeparator : IMenuItem { }

public class Menu : IHasMenuItems
{
    private readonly List<IMenuItem> _items = new();

    public Menu(LocalizedString name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public bool Enabled { get; set; } = true;

    public List<IMenuItem> Items
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

        SkipLayoutEnd = true;
    }

    public MenuBar(IEnumerable<Menu> menus) : this() => AddRange(menus);

    public Menu this[int index] {
        get => _menus[index];
        set => _menus[index] = value;
    }

    public int Count => _menus.Count;

    public bool IsMainMenuBar { get; set; } = false;

    public bool IsReadOnly => (_menus as IList<Menu>).IsReadOnly;

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        if (IsMainMenuBar)
        {
            if (!ImGui.BeginMainMenuBar())
            {
                return false;
            }
        }
        else
        {
            if (!ImGui.BeginMenuBar())
            {
                return false;
            }
        }

        foreach (var menu in _menus)
        {
            if (ImGui.BeginMenu(menu.Name, menu.Enabled))
            {
                LayoutItems(frameTime, menu);
                ImGui.EndMenu();
            }
        }

        LayoutEnd(frameTime);

        return true;
    }

    protected override void LayoutEnd(FrameTime frameTime)
    {
        if (IsMainMenuBar)
        {
            ImGui.EndMainMenuBar();
        }
        else
        {
            ImGui.EndMenuBar();
        }
    }

    protected virtual void LayoutItems(FrameTime frameTime, IHasMenuItems hasMenuItems)
    {
        foreach (var item in hasMenuItems.Items)
        {
            switch (item)
            {
                case MenuItemSeparator _:
                    ImGui.Separator();
                    break;

                case MenuItemOption menuItemOption:
                    var selected = menuItemOption.Checked;
                    var result = ImGui.MenuItem(menuItemOption.Name, menuItemOption.Shortcut, ref selected, menuItemOption.Enabled);
                    menuItemOption.Checked = selected;
                    if (result)
                    {
                        menuItemOption.OnSelected();
                    }
                    break;

                case MenuItem menuItem:
                    if (menuItem.Items.Count > 0)
                    {
                        if (ImGui.BeginMenu(menuItem.Name, menuItem.Enabled))
                        {
                            LayoutItems(frameTime, menuItem);
                            ImGui.EndMenu();
                        }
                    }
                    else if (ImGui.MenuItem(menuItem.Name, menuItem.Shortcut, false, menuItem.Enabled))
                    {
                        menuItem.OnSelected();
                    }
                    break;

                default:
                    throw new NotImplementedException(item?.GetType().FullName);
            }
        }
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
