using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Hotbar;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Spells;

public partial class SpellItem : SlotItem
{
    // Controls
    private readonly Label _cooldownLabel;
    private readonly SpellsWindow _spellWindow;

    // Context Menu Handling
    private readonly MenuItem _useSpellMenuItem;
    private readonly MenuItem _forgetSpellMenuItem;

    public SpellItem(SpellsWindow spellWindow, Base parent, int index, ContextMenu contextMenu)
        : base(parent, nameof(SpellItem), index, contextMenu)
    {
        _spellWindow = spellWindow;
        TextureFilename = "spellitem.png";

        Icon.HoverEnter += Icon_HoverEnter;
        Icon.HoverLeave += Icon_HoverLeave;
        Icon.Clicked += Icon_Clicked;
        Icon.DoubleClicked += Icon_DoubleClicked;

        _cooldownLabel = new Label(this, "CooldownLabel")
        {
            IsVisibleInParent = false,
            FontName = "sourcesansproblack",
            FontSize = 8,
            TextColor = new Color(0, 255, 255, 255),
            Alignment = [Alignments.Center],
            BackgroundTemplateName = "quantity.png",
            Padding = new Padding(2),
        };

        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        contextMenu.ClearChildren();
        _useSpellMenuItem = contextMenu.AddItem(Strings.SpellContextMenu.Cast.ToString());
        _useSpellMenuItem.Clicked += _useSpellMenuItem_Clicked;
        _forgetSpellMenuItem = contextMenu.AddItem(Strings.SpellContextMenu.Forget.ToString());
        _forgetSpellMenuItem.Clicked += _forgetSpellMenuItem_Clicked;
        contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    #region Context Menu

    protected override void OnContextMenuOpening(ContextMenu contextMenu)
    {
        // Clear out the old options.
        contextMenu.ClearChildren();

        if (Globals.Me?.Spells is not { Length: > 0 } spellSlots)
        {
            return;
        }

        // No point showing a menu for blank space.
        if (!SpellDescriptor.TryGet(spellSlots[SlotIndex].Id, out var spell))
        {
            return;
        }

        // Add our use spell option.
        contextMenu.AddChild(_useSpellMenuItem);
        _useSpellMenuItem.SetText(Strings.SpellContextMenu.Cast.ToString(spell.Name));

        // If this spell is not bound, allow users to forget it!
        if (!spell.Bound)
        {
            contextMenu.AddChild(_forgetSpellMenuItem);
            _forgetSpellMenuItem.SetText(Strings.SpellContextMenu.Forget.ToString(spell.Name));
        }

        base.OnContextMenuOpening(contextMenu);
    }

    private void _useSpellMenuItem_Clicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryUseSpell(SlotIndex);
    }

    private void _forgetSpellMenuItem_Clicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryForgetSpell(SlotIndex);
    }

    #endregion

    #region Mouse Events

    private void Icon_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        if (Globals.Me?.Spells is not { Length: > 0 } spellSlots)
        {
            return;
        }

        Interface.GameUi.SpellDescriptionWindow?.Show(spellSlots[SlotIndex].Id);
    }

    private void Icon_HoverLeave(Base sender, EventArgs arguments)
    {
        Interface.GameUi.SpellDescriptionWindow?.Hide();
    }

    private void Icon_Clicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton is MouseButton.Right)
        {
            if (ClientConfiguration.Instance.EnableContextMenus)
            {
                OpenContextMenu();
            }
            else
            {
                Globals.Me?.TryForgetSpell(SlotIndex);
            }
        }
    }

    private void Icon_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryUseSpell(SlotIndex);
    }

    #endregion

    #region Drag and Drop

    public override bool DragAndDrop_HandleDrop(Package package, int x, int y)
    {
        if (Globals.Me is not { } player)
        {
            return false;
        }

        var targetNode = Interface.FindComponentUnderCursor();

        // Find the first parent acceptable in that tree that can accept the package
        while (targetNode != default)
        {
            switch (targetNode)
            {
                case SpellItem spellItem:
                    player.SwapSpells(SlotIndex, spellItem.SlotIndex);
                    return true;

                case HotbarItem hotbarItem:
                    player.AddToHotbar(hotbarItem.SlotIndex, 1, SlotIndex);
                    return true;

                default:
                    targetNode = targetNode.Parent;
                    break;
            }
        }

        // If we've reached the top of the tree, we can't drop here, so cancel drop
        return false;
    }

    #endregion

    public override void Update()
    {
        if (Globals.Me == default)
        {
            return;
        }

        if (Globals.Me?.Spells is not { Length: > 0 } spellSlots)
        {
            return;
        }

        if (!SpellDescriptor.TryGet(spellSlots[SlotIndex].Id, out var spell))
        {
            Icon.Hide();
            Icon.Texture = null;
            _cooldownLabel.Hide();
            return;
        }

        _cooldownLabel.IsVisibleInParent = !Icon.IsDragging && Globals.Me.IsSpellOnCooldown(SlotIndex);
        if (_cooldownLabel.IsVisibleInParent)
        {
            var itemCooldownRemaining = Globals.Me.GetSpellRemainingCooldown(SlotIndex);
            _cooldownLabel.Text = TimeSpan.FromMilliseconds(itemCooldownRemaining).WithSuffix("0.0");
            Icon.RenderColor.A = 100;
        }
        else
        {
            Icon.RenderColor.A = 255;
        }

        if (Icon.TextureFilename == spell.Icon)
        {
            return;
        }

        var spellTexture = GameContentManager.Current.GetTexture(TextureType.Spell, spell.Icon);
        if (spellTexture != default)
        {
            Icon.Texture = spellTexture;
            Icon.RenderColor.A = (byte)(_cooldownLabel.IsVisibleInParent ? 100 : 255);
            Icon.IsVisibleInParent = true;
        }
        else
        {
            if (Icon.Texture != null)
            {
                Icon.Texture = null;
                Icon.IsVisibleInParent = false;
            }
        }
    }
}
