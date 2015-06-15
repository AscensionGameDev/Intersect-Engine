using Gwen;
using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Game
{
    public class SpellWindow : IGUIElement
    {
        //Controls
        private WindowControl _spellWindow;
        private ScrollControl _spellContainer;

        //Spell Container
        private int SpellXPadding = 4;
        private int SpellYPadding = 4;
        private List<ImagePanel> _spells = new List<ImagePanel>();

        //Init
        public SpellWindow(Canvas _gameCanvas)
        {
            _spellWindow = new WindowControl(_gameCanvas, "Skills");
            _spellWindow.SetSize(200, 300);
            _spellWindow.SetPosition(Graphics.ScreenWidth - 210, Graphics.ScreenHeight - 500);
            _spellWindow.DisableResizing();
            _spellWindow.Margin = Margin.Zero;
            _spellWindow.Padding = Padding.Zero;
            _spellWindow.IsHidden = true;

            _spellContainer = new ScrollControl(_spellWindow);
            _spellContainer.SetPosition(0, 0);
            _spellContainer.SetSize(_spellWindow.Width, _spellWindow.Height - 24);
            _spellContainer.EnableScroll(false, true);
            InitSpellContainer();
        }

        //Methods
        public void Update()
        {

        }
        private void InitSpellContainer()
        {
            int x = SpellXPadding;
            int y = SpellYPadding;
            for (int i = 0; i < Constants.MaxInvItems; i++)
            {
                _spells.Add(new ImagePanel(_spellContainer) { ImageName = "Resources/Spells/1.png" });
                _spells[i].SetSize(32, 32);
                _spells[i].SetPosition(x, y);
                _spells[i].Clicked += SpellWindow_Clicked;
                x += 32 + SpellXPadding;
                if (x + 32 + 4 > _spellContainer.Width)
                {
                    x = SpellXPadding;
                    y = y + 32 + SpellYPadding;
                }
            }
        }

        void SpellWindow_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.ChatboxContent.Add("You clicked on spell " + _spells.IndexOf((ImagePanel)sender));
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
    }
}
