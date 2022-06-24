using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Spells
{

    public partial class SpellsWindow
    {

        //Spell List
        public List<SpellItem> Items = new List<SpellItem>();

        //Initialized
        private bool mInitializedSpells;

        //Item/Spell Rendering
        private ScrollControl mItemContainer;

        //Controls
        private WindowControl mSpellWindow;

        //Location
        public int X;

        public int Y;

        // Context menu
        private Framework.Gwen.Control.Menu mContextMenu;

        private MenuItem mUseSpellContextItem;

        private MenuItem mForgetSpellContextItem;

        //Init
        public SpellsWindow(Canvas gameCanvas)
        {
            mSpellWindow = new WindowControl(gameCanvas, Strings.Spells.title, false, "SpellsWindow");
            mSpellWindow.DisableResizing();

            mItemContainer = new ScrollControl(mSpellWindow, "SpellsContainer");
            mItemContainer.EnableScroll(false, true);
            mSpellWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            // Generate our context menu with basic options.
            mContextMenu = new Framework.Gwen.Control.Menu(gameCanvas, "SpellContextMenu");
            mContextMenu.IsHidden = true;
            mContextMenu.IconMarginDisabled = true;
            //TODO: Is this a memory leak?
            mContextMenu.Children.Clear();
            mUseSpellContextItem = mContextMenu.AddItem(Strings.SpellContextMenu.Cast);
            mUseSpellContextItem.Clicked += MUseSpellContextItem_Clicked;
            mForgetSpellContextItem = mContextMenu.AddItem(Strings.SpellContextMenu.Forget);
            mForgetSpellContextItem.Clicked += MForgetSpellContextItem_Clicked;
            mContextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        public void OpenContextMenu(int slot)
        {
            // Clear out the old options.
            mContextMenu.RemoveChild(mUseSpellContextItem, false);
            mContextMenu.RemoveChild(mForgetSpellContextItem, false);
            mContextMenu.Children.Clear();

            var spell = SpellBase.Get(Globals.Me.Spells[slot].Id);

            // No point showing a menu for blank space.
            if (spell == null)
            {
                return;
            }

            // Add our use spell option.
            mContextMenu.AddChild(mUseSpellContextItem);
            mUseSpellContextItem.SetText(Strings.SpellContextMenu.Cast.ToString(spell.Name));

            // If this spell is not bound, allow users to forget it!
            if (!spell.Bound)
            {
                mContextMenu.AddChild(mForgetSpellContextItem);
                mForgetSpellContextItem.SetText(Strings.SpellContextMenu.Forget.ToString(spell.Name));
            }

            // Set our spell slot as userdata for future reference.
            mContextMenu.UserData = slot;

            mContextMenu.SizeToChildren();
            mContextMenu.Open(Framework.Gwen.Pos.None);
        }

        private void MForgetSpellContextItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.ClickedEventArgs arguments)
        {
            var slot = (int)sender.Parent.UserData;
            Globals.Me.TryForgetSpell(slot);
        }

        private void MUseSpellContextItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.ClickedEventArgs arguments)
        {
            var slot = (int)sender.Parent.UserData;
            Globals.Me.TryUseSpell(slot);
        }

        //Methods
        public void Update()
        {
            if (!mInitializedSpells)
            {
                InitItemContainer();
                mInitializedSpells = true;
            }

            if (mSpellWindow.IsHidden == true)
            {
                return;
            }

            X = mSpellWindow.X;
            Y = mSpellWindow.Y;
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                var spell = SpellBase.Get(Globals.Me.Spells[i].Id);
                Items[i].Pnl.IsHidden = spell == null || Items[i].IsDragging;
                if (spell != null)
                {
                    Items[i].Update();
                }
            }
        }

        private void InitItemContainer()
        {
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                Items.Add(new SpellItem(this, i));
                Items[i].Container = new ImagePanel(mItemContainer, "Spell");
                Items[i].Setup();

                Items[i].Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

                var xPadding = Items[i].Container.Margin.Left + Items[i].Container.Margin.Right;
                var yPadding = Items[i].Container.Margin.Top + Items[i].Container.Margin.Bottom;
                Items[i]
                    .Container.SetPosition(
                        i %
                        (mItemContainer.Width / (Items[i].Container.Width + xPadding)) *
                        (Items[i].Container.Width + xPadding) +
                        xPadding,
                        i /
                        (mItemContainer.Width / (Items[i].Container.Width + xPadding)) *
                        (Items[i].Container.Height + yPadding) +
                        yPadding
                    );
            }
        }

        public void Show()
        {
            mSpellWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mSpellWindow.IsHidden;
        }

        public void Hide()
        {
            mSpellWindow.IsHidden = true;
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect()
            {
                X = mSpellWindow.LocalPosToCanvas(new Point(0, 0)).X -
                    (Items[0].Container.Padding.Left + Items[0].Container.Padding.Right) / 2,
                Y = mSpellWindow.LocalPosToCanvas(new Point(0, 0)).Y -
                    (Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom) / 2,
                Width = mSpellWindow.Width + Items[0].Container.Padding.Left + Items[0].Container.Padding.Right,
                Height = mSpellWindow.Height + Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom
            };

            return rect;
        }

    }

}
