using System;
using System.Collections.Generic;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;

namespace Intersect.Client.UI.Game.Spells
{
    public class SpellsWindow
    {
        //Initialized
        private bool mInitializedSpells;

        //Item/Spell Rendering
        private ScrollControl mItemContainer;

        //Controls
        private WindowControl mSpellWindow;

        //Spell List
        public List<SpellItem> Items = new List<SpellItem>();

        //Location
        public int X;

        public int Y;

        //Init
        public SpellsWindow(Canvas gameCanvas)
        {
            mSpellWindow = new WindowControl(gameCanvas, Strings.Spells.title, false, "SpellsWindow");
            mSpellWindow.DisableResizing();

            mItemContainer = new ScrollControl(mSpellWindow, "SpellsContainer");
            mItemContainer.EnableScroll(false, true);
            mSpellWindow.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());
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
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Globals.Me.Spells[i].SpellId != Guid.Empty)
                {
                    Items[i].Pnl.IsHidden = false;
                    Items[i].Update();
                    if (Items[i].IsDragging)
                    {
                        Items[i].Pnl.IsHidden = true;
                    }
                }
                else
                {
                    Items[i].Pnl.IsHidden = true;
                }
            }
        }

        private void InitItemContainer()
        {
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                Items.Add(new SpellItem(this, i));
                Items[i].Container = new ImagePanel(mItemContainer, "Spell");
                Items[i].Setup();
                
                Items[i].Container.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());

                var xPadding = Items[i].Container.Padding.Left + Items[i].Container.Padding.Right;
                var yPadding = Items[i].Container.Padding.Top + Items[i].Container.Padding.Bottom;
                Items[i].Container.SetPosition(
                    (i % (mItemContainer.Width / (Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Width + xPadding) + xPadding,
                    (i / (mItemContainer.Width / (Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Height + yPadding) + yPadding);
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
            FloatRect rect = new FloatRect()
            {
                X = mSpellWindow.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).X -
                    (Items[0].Container.Padding.Left + Items[0].Container.Padding.Right) / 2,
                Y = mSpellWindow.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).Y -
                    (Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom) / 2,
                Width = mSpellWindow.Width + (Items[0].Container.Padding.Left + Items[0].Container.Padding.Right),
                Height = mSpellWindow.Height + (Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom)
            };
            return rect;
        }
    }
}