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
using Intersect.Client.Interface.Game.DescriptionWindows;
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
    private SpellDescriptionWindow? _descriptionWindow;

    // Context Menu Handling
    private readonly MenuItem _useSpellMenuItem;
    private readonly MenuItem _forgetSpellMenuItem;

    public SpellItem(SpellsWindow spellWindow, Base parent, int index, ContextMenu contextMenu)
        : base(parent, nameof(SpellItem), index, contextMenu)
    {
        _spellWindow = spellWindow;
        TextureFilename = "spellitem.png";

        IconImage.HoverEnter += IconImage_HoverEnter;
        IconImage.HoverLeave += IconImage_HoverLeave;
        IconImage.Clicked += IconImage_Clicked;
        IconImage.DoubleClicked += IconImage_DoubleClicked;

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

    private void IconImage_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        _descriptionWindow?.Dispose();
        _descriptionWindow = null;

        if (Globals.Me?.Spells is not { Length: > 0 } spellSlots)
        {
            return;
        }

        _descriptionWindow = new SpellDescriptionWindow(spellSlots[SlotIndex].Id, _spellWindow.X, _spellWindow.Y);
    }

    private void IconImage_HoverLeave(Base sender, EventArgs arguments)
    {
        _descriptionWindow?.Dispose();
        _descriptionWindow = null;
    }

    private void IconImage_Clicked(Base sender, MouseButtonState arguments)
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

    private void IconImage_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryUseSpell(SlotIndex);
    }

    #endregion

    #region Drag and Drop

    public override bool DragAndDrop_HandleDrop(Package package, int x, int y)
    {
        var targetNode = Interface.FindComponentUnderCursor(NodeFilter.None);

        // Find the first parent acceptable in that tree that can accept the package
        while (targetNode != default)
        {
            switch (targetNode)
            {
                case SpellItem spellItem:
                    Globals.Me?.SwapSpells(SlotIndex, spellItem.SlotIndex);
                    return true;

                case HotbarItem hotbarItem:
                    Globals.Me?.AddToHotbar(hotbarItem.SlotIndex, 1, SlotIndex);
                    return true;

                default:
                    targetNode = targetNode.Parent;
                    break;
            }

            // If we've reached the top of the tree, we can't drop here, so cancel drop
            if (targetNode == null)
            {
                return false;
            }
        }

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
            IconImage.Hide();
            IconImage.Texture = null;
            _cooldownLabel.Hide();
            return;
        }

        _cooldownLabel.IsVisibleInParent = !IconImage.IsDragging && Globals.Me.IsSpellOnCooldown(SlotIndex);
        if (_cooldownLabel.IsVisibleInParent)
        {
            var itemCooldownRemaining = Globals.Me.GetSpellRemainingCooldown(SlotIndex);
            _cooldownLabel.Text = TimeSpan.FromMilliseconds(itemCooldownRemaining).WithSuffix("0.0");
            IconImage.RenderColor.A = 100;
        }
        else
        {
            IconImage.RenderColor.A = 255;
        }

        if (Path.GetFileName(IconImage.Texture?.Name) != spell.Icon)
        {
            var spellIconTexture = Globals.ContentManager?.GetTexture(TextureType.Spell, spell.Icon);
            if (spellIconTexture != null)
            {
                IconImage.Texture = spellIconTexture;
                IconImage.RenderColor.A = (byte)(_cooldownLabel.IsVisibleInParent ? 100 : 255);
                IconImage.IsVisibleInParent = true;
            }
            else
            {
                if (IconImage.Texture != null)
                {
                    IconImage.Texture = null;
                    IconImage.IsVisibleInParent = false;
                }
            }

            _descriptionWindow?.Dispose();
            _descriptionWindow = null;
        }
    }
}
