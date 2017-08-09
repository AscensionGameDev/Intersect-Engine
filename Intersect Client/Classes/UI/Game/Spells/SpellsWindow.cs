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
        private bool _initializedSpells;

        //Item/Spell Rendering
        private ScrollControl _itemContainer;

        private ImagePanel _itemTemplate;

        //Controls
        private WindowControl _spellWindow;

        //Spell List
        public List<SpellItem> Items = new List<SpellItem>();

        //Location
        public int X;

        public int Y;

        //Init
        public SpellsWindow(Canvas _gameCanvas)
        {
            _spellWindow = new WindowControl(_gameCanvas, Strings.Get("spells", "title"), false, "SpellsWindow");
            _spellWindow.DisableResizing();

            _itemContainer = new ScrollControl(_spellWindow, "SpellsContainer");
            _itemContainer.EnableScroll(false, true);

            _itemTemplate = new ImagePanel(_itemContainer, "SpellContainer");

            new ImagePanel(_itemTemplate, "SpellIcon");
        }

        //Methods
        public void Update()
        {
            if (!_initializedSpells)
            {
                InitItemContainer();
                _initializedSpells = true;
            }
            if (_spellWindow.IsHidden == true)
            {
                return;
            }
            X = _spellWindow.X;
            Y = _spellWindow.Y;
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Globals.Me.Spells[i].SpellNum > -1)
                {
                    Items[i].pnl.IsHidden = false;
                    Items[i].Update();
                    if (Items[i].IsDragging)
                    {
                        Items[i].pnl.IsHidden = true;
                    }
                }
                else
                {
                    Items[i].pnl.IsHidden = true;
                }
            }
        }

        private void InitItemContainer()
        {
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                Items.Add(new SpellItem(this, i));
                Items[i].container = new ImagePanel(_itemContainer, "SpellContainer");
                Items[i].Setup();

                //TODO Made this more efficient.
                Gui.LoadRootUIData(Items[i].container, "InGame.xml");

                var xPadding = Items[i].container.Padding.Left + Items[i].container.Padding.Right;
                var yPadding = Items[i].container.Padding.Top + Items[i].container.Padding.Bottom;
                Items[i].container.SetPosition(
                    (i % (_itemContainer.Width / (Items[i].container.Width + xPadding))) *
                    (Items[i].container.Width + xPadding) + xPadding,
                    (i / (_itemContainer.Width / (Items[i].container.Width + xPadding))) *
                    (Items[i].container.Height + yPadding) + yPadding);
            }
            _itemTemplate.Hide();
        }

        public void Show()
        {
            _spellWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !_spellWindow.IsHidden;
        }

        public void Hide()
        {
            _spellWindow.IsHidden = true;
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = _spellWindow.LocalPosToCanvas(new Point(0, 0)).X -
                    (Items[0].container.Padding.Left + Items[0].container.Padding.Right) / 2,
                Y = _spellWindow.LocalPosToCanvas(new Point(0, 0)).Y -
                    (Items[0].container.Padding.Top + Items[0].container.Padding.Bottom) / 2,
                Width = _spellWindow.Width + (Items[0].container.Padding.Left + Items[0].container.Padding.Right),
                Height = _spellWindow.Height + (Items[0].container.Padding.Top + Items[0].container.Padding.Bottom)
            };
            return rect;
        }
    }
}