using System.Collections.Generic;
using Intersect;
using Intersect.Client.Classes.UI.Game.Spells;
using Intersect.Localization;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.UI.Game
{
    public class SpellsWindow
    {
        //Initialized
        private bool mInitializedSpells;

        //Item/Spell Rendering
        private ScrollControl mItemContainer;

        private ImagePanel mItemTemplate;

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
            mSpellWindow = new WindowControl(gameCanvas, Strings.Get("spells", "title"), false, "SpellsWindow");
            mSpellWindow.DisableResizing();

            mItemContainer = new ScrollControl(mSpellWindow, "SpellsContainer");
            mItemContainer.EnableScroll(false, true);

            mItemTemplate = new ImagePanel(mItemContainer, "SpellContainer");

            new ImagePanel(mItemTemplate, "SpellIcon");
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
                if (Globals.Me.Spells[i].SpellNum > -1)
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
                Items[i].Container = new ImagePanel(mItemContainer, "SpellContainer");
                Items[i].Setup();

                //TODO Made this more efficient.
                Gui.LoadRootUiData(Items[i].Container, "InGame.xml");

                var xPadding = Items[i].Container.Padding.Left + Items[i].Container.Padding.Right;
                var yPadding = Items[i].Container.Padding.Top + Items[i].Container.Padding.Bottom;
                Items[i].Container.SetPosition(
                    (i % (mItemContainer.Width / (Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Width + xPadding) + xPadding,
                    (i / (mItemContainer.Width / (Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Height + yPadding) + yPadding);
            }
            mItemTemplate.Hide();
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
                X = mSpellWindow.LocalPosToCanvas(new Point(0, 0)).X -
                    (Items[0].Container.Padding.Left + Items[0].Container.Padding.Right) / 2,
                Y = mSpellWindow.LocalPosToCanvas(new Point(0, 0)).Y -
                    (Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom) / 2,
                Width = mSpellWindow.Width + (Items[0].Container.Padding.Left + Items[0].Container.Padding.Right),
                Height = mSpellWindow.Height + (Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom)
            };
            return rect;
        }
    }
}