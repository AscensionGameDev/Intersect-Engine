using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.Framework.Core;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Spells;

public partial class SpellItem : SlotItem
{
    // Controls
    private readonly Label _cooldownLabel;
    private readonly SpellsWindow _spellWindow;
    private Draggable? _dragIcon;
    private SpellDescriptionWindow? _descriptionWindow;

    // Drag Handling
    public bool IsDragging;
    private bool _canDrag;
    private long _clickTime;
    private bool _mouseOver;
    private int _mouseX = -1;
    private int _mouseY = -1;

    // Context Menu Handling
    private readonly MenuItem _useSpellMenuItem;
    private readonly MenuItem _forgetSpellMenuItem;

    public SpellItem(SpellsWindow spellWindow, Base parent, int index, ContextMenu contextMenu)
        : base(parent, nameof(SpellItem), index, contextMenu)
    {
        _spellWindow = spellWindow;
        TextureFilename = "spellitem.png";

        _iconImage.HoverEnter += _iconImage_HoverEnter;
        _iconImage.HoverLeave += _iconImage_HoverLeave;
        _iconImage.Clicked += _iconImage_Clicked;
        _iconImage.DoubleClicked += _iconImage_DoubleClicked;

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

        _contextMenu!.ClearChildren();
        _useSpellMenuItem = _contextMenu.AddItem(Strings.SpellContextMenu.Cast.ToString());
        _useSpellMenuItem.Clicked += _useSpellMenuItem_Clicked;
        _forgetSpellMenuItem = _contextMenu.AddItem(Strings.SpellContextMenu.Forget.ToString());
        _forgetSpellMenuItem.Clicked += _forgetSpellMenuItem_Clicked;
        _contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    #region Context Menu

    public void OpenContextMenu()
    {
        // Clear out the old options.
        _contextMenu!.ClearChildren();

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
        _contextMenu.AddChild(_useSpellMenuItem);
        _useSpellMenuItem.SetText(Strings.SpellContextMenu.Cast.ToString(spell.Name));

        // If this spell is not bound, allow users to forget it!
        if (!spell.Bound)
        {
            _contextMenu.AddChild(_forgetSpellMenuItem);
            _forgetSpellMenuItem.SetText(Strings.SpellContextMenu.Forget.ToString(spell.Name));
        }

        _contextMenu.SizeToChildren();
        _contextMenu.Open(Pos.None);
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

    private void _iconImage_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        _mouseOver = true;
        _canDrag = true;

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            _canDrag = false;
            return;
        }

        if (_descriptionWindow != null)
        {
            _descriptionWindow.Dispose();
            _descriptionWindow = null;
        }

        if (Globals.Me?.Spells is not { Length: > 0 } spellSlots)
        {
            return;
        }

        _descriptionWindow = new SpellDescriptionWindow(spellSlots[SlotIndex].Id, _spellWindow.X, _spellWindow.Y);
    }

    private void _iconImage_HoverLeave(Base sender, EventArgs arguments)
    {
        _mouseOver = false;
        _mouseX = -1;
        _mouseY = -1;

        if (_descriptionWindow != null)
        {
            _descriptionWindow.Dispose();
            _descriptionWindow = null;
        }
    }

    private void _iconImage_Clicked(Base sender, MouseButtonState arguments)
    {
        switch (arguments.MouseButton)
        {
            case MouseButton.Left:
                _clickTime = Timing.Global.MillisecondsUtc + 500;
                break;

            case MouseButton.Right:
                if (ClientConfiguration.Instance.EnableContextMenus)
                {
                    OpenContextMenu();
                }
                else
                {
                    Globals.Me?.TryForgetSpell(SlotIndex);
                }
                break;
        }
    }

    private void _iconImage_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryUseSpell(SlotIndex);
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
            _iconImage.Hide();
            _iconImage.Texture = null;
            _cooldownLabel.Hide();
            return;
        }

        _cooldownLabel.IsVisibleInParent = !IsDragging && Globals.Me.IsSpellOnCooldown(SlotIndex);
        if (_cooldownLabel.IsVisibleInParent)
        {
            var itemCooldownRemaining = Globals.Me.GetSpellRemainingCooldown(SlotIndex);
            _cooldownLabel.Text = TimeSpan.FromMilliseconds(itemCooldownRemaining).WithSuffix("0.0");
            _iconImage.RenderColor.A = 100;
        }
        else
        {
            _iconImage.RenderColor.A = 255;
        }

        if (Path.GetFileName(_iconImage.Texture?.Name) != spell.Icon)
        {
            var spellIconTexture = Globals.ContentManager?.GetTexture(TextureType.Spell, spell.Icon);
            if (spellIconTexture != null)
            {
                _iconImage.Texture = spellIconTexture;
                _iconImage.RenderColor.A = (byte)(_cooldownLabel.IsVisibleInParent ? 100 : 255);
                _iconImage.IsVisibleInParent = true;
            }
            else
            {
                if (_iconImage.Texture != null)
                {
                    _iconImage.Texture = null;
                    _iconImage.IsVisibleInParent = false;
                }
            }

            _descriptionWindow?.Dispose();
            _descriptionWindow = null;
        }

        if (!IsDragging)
        {
            if (_mouseOver)
            {
                if (!Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
                {
                    _canDrag = true;
                    _mouseX = -1;
                    _mouseY = -1;
                    if (Timing.Global.MillisecondsUtc < _clickTime)
                    {
                        _clickTime = 0;
                    }
                }
                else
                {
                    if (_canDrag && Draggable.Active == null)
                    {
                        if (_mouseX == -1 || _mouseY == -1)
                        {
                            _mouseX = InputHandler.MousePosition.X - _iconImage.ToCanvas(new Point(0, 0)).X;
                            _mouseY = InputHandler.MousePosition.Y - _iconImage.ToCanvas(new Point(0, 0)).Y;
                        }
                        else
                        {
                            var xdiff = _mouseX -
                                        (InputHandler.MousePosition.X - _iconImage.ToCanvas(new Point(0, 0)).X);

                            var ydiff = _mouseY -
                                        (InputHandler.MousePosition.Y - _iconImage.ToCanvas(new Point(0, 0)).Y);

                            if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                            {
                                IsDragging = true;
                                _iconImage.IsVisibleInParent = false;
                                _dragIcon = new Draggable(
                                    _iconImage.ToCanvas(new Point(0, 0)).X + _mouseX,
                                    _iconImage.ToCanvas(new Point(0, 0)).X + _mouseY, _iconImage.Texture, _iconImage.RenderColor
                                );
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (_dragIcon?.Update() == true)
            {
                //Drug the item and now we stopped
                IsDragging = false;
                _iconImage.IsVisibleInParent = true;

                var dragRect = new FloatRect(
                    _dragIcon.X - (Padding.Left + Padding.Right) / 2f,
                    _dragIcon.Y - (Padding.Top + Padding.Bottom) / 2f,
                    (Padding.Left + Padding.Right) / 2f + _iconImage.Width,
                    (Padding.Top + Padding.Bottom) / 2f + _iconImage.Height
                );

                float bestIntersect = 0;
                var bestIntersectIndex = -1;

                //So we picked up an item and then dropped it. Lets see where we dropped it to.
                //Check spell first.
                if (_spellWindow.RenderBounds().IntersectsWith(dragRect))
                {
                    for (var i = 0; i < Options.Instance.Player.MaxSpells; i++)
                    {
                        if (_spellWindow.Items[i] is not SpellItem spellSlot)
                        {
                            continue;
                        }

                        if (i < _spellWindow.Items.Count &&
                            spellSlot.RenderBounds().IntersectsWith(dragRect))
                        {
                            if (FloatRect.Intersect(spellSlot.RenderBounds(), dragRect).Width *
                                FloatRect.Intersect(spellSlot.RenderBounds(), dragRect).Height >
                                bestIntersect)
                            {
                                bestIntersect =
                                    FloatRect.Intersect(spellSlot.RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(spellSlot.RenderBounds(), dragRect).Height;

                                bestIntersectIndex = i;
                            }
                        }
                    }

                    if (bestIntersectIndex > -1)
                    {
                        if (SlotIndex != bestIntersectIndex && !Globals.Me.IsCasting)
                        {
                            Globals.Me.SwapSpells(bestIntersectIndex, SlotIndex);
                        }
                    }
                }
                else if (Interface.GameUi.Hotbar.RenderBounds().IntersectsWith(dragRect))
                {
                    for (var i = 0; i < Options.Instance.Player.HotbarSlotCount; i++)
                    {
                        if (Interface.GameUi.Hotbar.Items[i].RenderBounds().IntersectsWith(dragRect))
                        {
                            if (FloatRect.Intersect(
                                        Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect
                                    )
                                    .Width *
                                FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                    .Height >
                                bestIntersect)
                            {
                                bestIntersect =
                                    FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                        .Width *
                                    FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                        .Height;

                                bestIntersectIndex = i;
                            }
                        }
                    }

                    if (bestIntersectIndex > -1)
                    {
                        Globals.Me.AddToHotbar((byte) bestIntersectIndex, 1, SlotIndex);
                    }
                }

                _dragIcon.Dispose();
            }
        }
    }

    public FloatRect RenderBounds()
    {
        var rect = new FloatRect()
        {
            X = _iconImage.ToCanvas(new Point(0, 0)).X,
            Y = _iconImage.ToCanvas(new Point(0, 0)).Y,
            Width = _iconImage.Width,
            Height = _iconImage.Height
        };

        return rect;
    }
}
