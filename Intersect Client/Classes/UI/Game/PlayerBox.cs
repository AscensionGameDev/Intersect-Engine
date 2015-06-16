using Gwen;
using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Game
{
    public class PlayerBox
    {
        //Controls
        public WindowControl _playerBox;
        public Label _hpLbl;
        public Label _mpLbl;
        public Label _expLbl;
        public ImagePanel _hpBackground;
        public ImagePanel _hpBar;
        public ImagePanel _mpBackground;
        public ImagePanel _mpBar;
        public ImagePanel _expBackground;
        public ImagePanel _expBar;

        //Init
        public PlayerBox(Canvas _gameCanvas)
        {
            _playerBox = new WindowControl(_gameCanvas) { Title = "Vitals" };
            _playerBox.SetSize(173, 80);
            _playerBox.SetPosition(0, 0);
            _playerBox.DisableResizing();

            _playerBox.Margin = Margin.Zero;
            _playerBox.Padding = Padding.Zero;
            _playerBox.IsClosable = false;

            _hpBackground = new ImagePanel(_playerBox) { ImageName = "Resources/GUI/HPBarEmpty.png" };
            _hpBackground.SetSize(169, 17);
            _hpBackground.SetPosition(2, 0);

            _hpBar = new ImagePanel(_playerBox) { ImageName = "Resources/GUI/HPBar.png" };
            _hpBar.SetSize(169, 14);
            _hpBar.SetPosition(2, 3);

            _hpLbl = new Label(_playerBox);
            _hpLbl.SetText("1000/1000");
            _hpLbl.SetPosition(20, 3);
            _hpLbl.TextColor = Color.Black;

            _mpBackground = new ImagePanel(_playerBox) { ImageName = "Resources/GUI/ManaBarEmpty.png" };
            _mpBackground.SetSize(169, 17);
            _mpBackground.SetPosition(2, 18);

            _mpBar = new ImagePanel(_playerBox) { ImageName = "Resources/GUI/ManaBar.png" };
            _mpBar.SetSize(169, 14);
            _mpBar.SetPosition(2, 21);

            _mpLbl = new Label(_playerBox);
            _mpLbl.SetText("1000/1000");
            _mpLbl.SetPosition(20, 21);
            _mpLbl.TextColor = Color.Black;

            _expBackground = new ImagePanel(_playerBox) { ImageName = "Resources/GUI/EXPBarEmpty.png" };
            _expBackground.SetSize(169, 17);
            _expBackground.SetPosition(2, 36);

            _expBar = new ImagePanel(_playerBox) { ImageName = "Resources/GUI/EXPBar.png" };
            _expBar.SetSize(169, 14);
            _expBar.SetPosition(2, 39);

            _expLbl = new Label(_playerBox);
            _expLbl.SetText("1000/1000");
            _expLbl.SetPosition(20, 39);
            _expLbl.TextColor = Color.Black;
        }

        //Update
        public void Update()
        {

        }

        //Input Handlers
    }
}
