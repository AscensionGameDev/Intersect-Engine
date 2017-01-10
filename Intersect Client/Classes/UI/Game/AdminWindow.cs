/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Library;
using Intersect_Library.GameObjects.Maps.MapList;
using Intersect_Library.Localization;

namespace Intersect_Client.Classes.UI.Game
{
    class AdminWindow
    {
        //Controls
        private WindowControl _adminWindow;

        //Player Mod Labels
        private Label _nameLabel;
        private Label _accessLabel;
        private Label _spriteLabel;
        private Label _faceLabel;

        //Graphics
        public ImagePanel _facePanel;
        public ImagePanel _spriteContainer;
        public ImagePanel _spritePanel;

        //Player Mod Textboxes
        private TextBox _nameTextbox;
        private ComboBox _spriteDropdown;
        private ComboBox _faceDropdown;
        private ComboBox _accessDropdown;

        //Player Mod Buttons
        private Button _kickButton;
        private Button _killButton;
        private Button _banButton;
        private Button _unbanButton;
        private Button _muteButton;
        private Button _unmuteButton;
        private Button _warpToMeButton;
        private Button _warpMeToButton;
        private Button _setSpriteButton;
        private Button _setPowerButton;
        private Button _setFaceButton;

        //Admin Powers
        private Label _noclipLabel;
        private CheckBox _noclipCheckBox;

        private TreeControl _mapList;
        private CheckBox _chkChronological;
        private Label _lblChronological;
        private Label _mapListLabel;

        //Windows
        BanMuteBox _banMuteWindow;

        //Init
        public AdminWindow(Canvas _gameCanvas)
        {
            _adminWindow = new WindowControl(_gameCanvas, Strings.Get("admin","title"));
            _adminWindow.SetSize(200, 540);
            _adminWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - _adminWindow.Width / 2, GameGraphics.Renderer.GetScreenHeight() / 2 - _adminWindow.Height / 2);
            _adminWindow.DisableResizing();
            _adminWindow.Margin = Margin.Zero;
            _adminWindow.Padding = Padding.Zero;

            //Player Mods
            _nameLabel = new Label(_adminWindow);
            _nameLabel.SetPosition(6,4);
            _nameLabel.Text = Strings.Get("admin", "name");

            _nameTextbox = new TextBox(_adminWindow);
            _nameTextbox.SetBounds(6,22,188,18);
            Gui.FocusElements.Add(_nameTextbox);

            _warpToMeButton = new Button(_adminWindow);
            _warpToMeButton.Text = Strings.Get("admin", "warp2me");
            _warpToMeButton.SetBounds(6, 44, 80, 18);
            _warpToMeButton.Clicked += _warpToMeButton_Clicked;

            _warpMeToButton = new Button(_adminWindow);
            _warpMeToButton.Text = Strings.Get("admin", "warpme2");
            _warpMeToButton.SetBounds(6, 64, 80, 18);
            _warpMeToButton.Clicked += _warpMeToButton_Clicked;

            _kickButton = new Button(_adminWindow);
            _kickButton.Text = Strings.Get("admin", "kick");
            _kickButton.SetBounds(90,44,50,18);
            _kickButton.Clicked += _kickButton_Clicked;

            _killButton = new Button(_adminWindow);
            _killButton.Text = Strings.Get("admin", "kill");
            _killButton.SetBounds(144,44,50,18);
            _killButton.Clicked += _killButton_Clicked;

            _banButton = new Button(_adminWindow);
            _banButton.Text = Strings.Get("admin", "ban");
            _banButton.SetBounds(90, 64, 50, 18);
            _banButton.Clicked += _banButton_Clicked;

            _noclipLabel = new Label(_adminWindow);
            _noclipLabel.Text = Strings.Get("admin", "noclip");
            _noclipLabel.SetPosition(6, 86);
            _noclipLabel.SetToolTipText("Check to walk through obstacles.");

            _noclipCheckBox = new CheckBox(_adminWindow);
            _noclipCheckBox.SetPosition(16 + _noclipLabel.Width, 86);
            _noclipCheckBox.IsChecked = Globals.Me.NoClip;
            _noclipCheckBox.CheckChanged += _noclipCheckBox_CheckChanged;
            _noclipCheckBox.SetToolTipText(Strings.Get("admin", "nocliptip"));

            _unbanButton = new Button(_adminWindow);
            _unbanButton.Text = Strings.Get("admin", "unban");
            _unbanButton.SetBounds(90, 84, 50, 18);
            _unbanButton.Clicked += _unbanButton_Clicked;

            _muteButton = new Button(_adminWindow);
            _muteButton.Text = Strings.Get("admin", "mute");
            _muteButton.SetBounds(144, 64, 50, 18);
            _muteButton.Clicked += _muteButton_Clicked;

            _unmuteButton = new Button(_adminWindow);
            _unmuteButton.Text = Strings.Get("admin", "unmute");
            _unmuteButton.SetBounds(144, 84, 50, 18);
            _unmuteButton.Clicked += _unmuteButton_Clicked;

            _spriteLabel = new Label(_adminWindow);
            _spriteLabel.SetPosition(6, 112);
            _spriteLabel.Text = Strings.Get("admin", "sprite");

            _spriteDropdown = new ComboBox(_adminWindow);
            _spriteDropdown.SetBounds(6, 128, 80, 18);
            _spriteDropdown.AddItem(Strings.Get("admin", "none"));
            var sprites = Globals.ContentManager.GetTextureNames(IntersectClientExtras.File_Management.GameContentManager.TextureType.Entity);
            Array.Sort(sprites, new AlphanumComparatorFast());
            foreach (var sprite in sprites)
            {
                _spriteDropdown.AddItem(sprite);
            }
            _spriteDropdown.ItemSelected += _spriteDropdown_ItemSelected;

            _setSpriteButton = new Button(_adminWindow);
            _setSpriteButton.Text = Strings.Get("admin", "setsprite");
            _setSpriteButton.SetBounds(6, 148, 80, 18);
            _setSpriteButton.Clicked += _setSpriteButton_Clicked;

            _spriteContainer = new ImagePanel(_adminWindow);
            _spriteContainer.SetSize(50, 50);
            _spriteContainer.SetPosition(115, 114);
            _spritePanel = new ImagePanel(_spriteContainer);

            _faceLabel = new Label(_adminWindow);
            _faceLabel.SetPosition(6, 172);
            _faceLabel.Text = Strings.Get("admin", "face");

            _faceDropdown = new ComboBox(_adminWindow);
            _faceDropdown.SetBounds(6, 188, 80, 18);
            _faceDropdown.AddItem(Strings.Get("admin", "none"));
            var faces = Globals.ContentManager.GetTextureNames(IntersectClientExtras.File_Management.GameContentManager.TextureType.Face);
            Array.Sort(faces, new AlphanumComparatorFast());
            foreach (var face in faces)
            {
                _faceDropdown.AddItem(face);
            }
            _faceDropdown.ItemSelected += _faceDropdown_ItemSelected;
            

            _setFaceButton = new Button(_adminWindow);
            _setFaceButton.Text = Strings.Get("admin", "setface");
            _setFaceButton.SetBounds(6, 208, 80, 18);
            _setFaceButton.Clicked += _setFaceButton_Clicked;

            _facePanel = new ImagePanel(_adminWindow);
            _facePanel.SetSize(50, 50);
            _facePanel.SetPosition(115, 174);

            _accessLabel = new Label(_adminWindow);
            _accessLabel.SetPosition(6, 232);
            _accessLabel.Text = Strings.Get("admin", "access");

            _accessDropdown = new ComboBox(_adminWindow);
            _accessDropdown.SetBounds(6, 248, 80, 18);
            _accessDropdown.AddItem(Strings.Get("admin", "access0")).UserData = "None";
            _accessDropdown.AddItem(Strings.Get("admin", "access1")).UserData = "Moderator";
            _accessDropdown.AddItem(Strings.Get("admin", "access2")).UserData = "Admin";

            _setPowerButton = new Button(_adminWindow);
            _setPowerButton.Text = Strings.Get("admin", "setpower");
            _setPowerButton.SetBounds(6, 268, 80, 18);
            _setPowerButton.Clicked += _setPowerButton_Clicked;

            CreateMapList();
            _mapListLabel = new Label(_adminWindow);
            _mapListLabel.Text = Strings.Get("admin", "maplist");
            _mapListLabel.SetPosition(4f, 294);

            _chkChronological = new CheckBox(_adminWindow);
            _chkChronological.SetToolTipText(Strings.Get("admin", "chronologicaltip"));
            _chkChronological.SetPosition(_adminWindow.Width - 24, 294);
            _chkChronological.CheckChanged += _chkChronological_CheckChanged;

            _lblChronological = new Label(_adminWindow);
            _lblChronological.Text = Strings.Get("admin", "chronological");
            _lblChronological.SetPosition(_chkChronological.X - 30, 294);

            UpdateMapList();
        }

        private void _spriteDropdown_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            _spritePanel.Texture = Globals.ContentManager.GetTexture(IntersectClientExtras.File_Management.GameContentManager.TextureType.Entity, _spriteDropdown.Text);
            if (_spritePanel.Texture != null)
            {
                _spritePanel.SetUV(0, 0, .25f, .25f);
                _spritePanel.SetSize(_spritePanel.Texture.GetWidth() / 4, _spritePanel.Texture.GetHeight() / 4);
                Align.AlignTop(_spritePanel);
                Align.CenterHorizontally(_spritePanel);
            }
        }

        private void _faceDropdown_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            _facePanel.Texture = Globals.ContentManager.GetTexture(IntersectClientExtras.File_Management.GameContentManager.TextureType.Face, _faceDropdown.Text);
        }

        void _noclipCheckBox_CheckChanged(Base sender, EventArgs arguments)
        {
            Globals.Me.NoClip = _noclipCheckBox.IsChecked;
        }

        //Methods
        public void SetName(string name)
        {
            _nameTextbox.Text = name;
        }
        private void CreateMapList()
        {
            _mapList = new TreeControl(_adminWindow);
            _mapList.SetPosition(4f, 316);
            _mapList.Height = 188;
            _mapList.Width = _adminWindow.Width - 8;
        }
        public void UpdateMapList()
        {
            _mapList.Dispose();
            CreateMapList();
            AddMapListToTree(MapList.GetList(), null);
        }
        private void AddMapListToTree(MapList mapList, TreeNode parent)
        {
            TreeNode tmpNode;
            if (_chkChronological.IsChecked)
            {
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    tmpNode = _mapList.AddNode(MapList.GetOrderedMaps()[i].Name);
                    tmpNode.UserData = (MapList.GetOrderedMaps()[i]).MapNum;
                    tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                    tmpNode.Clicked += tmpNode_DoubleClicked;
                }
            }
            else
            {
                for (int i = 0; i < mapList.Items.Count; i++)
                {
                    if (mapList.Items[i].GetType() == typeof(MapListFolder))
                    {
                        if (parent == null)
                        {
                            tmpNode = _mapList.AddNode(mapList.Items[i].Name);
                            tmpNode.UserData = ((MapListFolder)mapList.Items[i]);
                            AddMapListToTree(((MapListFolder)mapList.Items[i]).Children, tmpNode);
                        }
                        else
                        {
                            tmpNode = parent.AddNode(mapList.Items[i].Name);
                            tmpNode.UserData = ((MapListFolder)mapList.Items[i]);
                            AddMapListToTree(((MapListFolder)mapList.Items[i]).Children, tmpNode);
                        }
                    }
                    else
                    {
                        if (parent == null)
                        {
                            tmpNode = _mapList.AddNode(mapList.Items[i].Name);
                            tmpNode.UserData = ((MapListMap)mapList.Items[i]).MapNum;
                            tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                            tmpNode.Clicked += tmpNode_DoubleClicked;
                        }
                        else
                        {
                            tmpNode = parent.AddNode(mapList.Items[i].Name);
                            tmpNode.UserData = ((MapListMap)mapList.Items[i]).MapNum;
                            tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                            tmpNode.Clicked += tmpNode_DoubleClicked;
                        }
                    }
                }
            }
        }

        void _kickButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction((int)AdminActions.Kick, _nameTextbox.Text);
            }
        }

        void _killButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction((int)AdminActions.Kill, _nameTextbox.Text);
            }
        }

        void _warpToMeButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction((int)AdminActions.WarpToMe, _nameTextbox.Text);
            }
        }

        void _warpMeToButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction((int)AdminActions.WarpMeTo, _nameTextbox.Text);
            }
        }

        void _muteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {
                _banMuteWindow = new BanMuteBox(Strings.Get("admin","mutecaption", _nameTextbox.Text), Strings.Get("admin","muteprompt",_nameTextbox.Text), true, MuteUser);
            }
        }

        void MuteUser(Object sender, EventArgs e)
        {
            PacketSender.SendAdminAction((int)AdminActions.Mute, _nameTextbox.Text, _banMuteWindow.GetDuration().ToString(), _banMuteWindow.GetReason(), Convert.ToString(_banMuteWindow.BanIp()));
            _banMuteWindow.Dispose();
        }

        void BanUser(Object sender, EventArgs e)
        {
            PacketSender.SendAdminAction((int)AdminActions.Ban, _nameTextbox.Text, _banMuteWindow.GetDuration().ToString(), _banMuteWindow.GetReason(), Convert.ToString(_banMuteWindow.BanIp()));
            _banMuteWindow.Dispose();
        }

        void _banButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0 && _nameTextbox.Text.Trim().ToLower() != Globals.Me.MyName.Trim().ToLower())
            {
                _banMuteWindow = new BanMuteBox(Strings.Get("admin", "bancaption", _nameTextbox.Text), Strings.Get("admin", "banprompt", _nameTextbox.Text), true, BanUser);
            }
        }

        private void _setFaceButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {

                PacketSender.SendAdminAction((int)AdminActions.SetFace, _nameTextbox.Text, _faceDropdown.Text);
            }
        }

        void _unmuteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {
                var confirmWindow = new InputBox(Strings.Get("admin", "unmutecaption", _nameTextbox.Text), Strings.Get("admin", "unmuteprompt", _nameTextbox.Text), true, UnmuteUser, null, -1, false);
            }
        }

        void _unbanButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {
                var confirmWindow = new InputBox(Strings.Get("admin", "unbancaption", _nameTextbox.Text), Strings.Get("admin", "unbanprompt", _nameTextbox.Text), true, UnbanUser,null,-1,false);
            }
        }

        void UnmuteUser(Object sender, EventArgs e)
        {
            PacketSender.SendAdminAction((int)AdminActions.UnMute, _nameTextbox.Text);
        }

        void UnbanUser(Object sender, EventArgs e)
        {
            PacketSender.SendAdminAction((int)AdminActions.UnBan, _nameTextbox.Text);
        }

        void _setSpriteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction((int)AdminActions.SetSprite, _nameTextbox.Text, _spriteDropdown.Text);
            }
        }

        void _setPowerButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_nameTextbox.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction((int)AdminActions.SetAccess, _nameTextbox.Text, _accessDropdown.UserData.ToString());
            }
        }

        void _chkChronological_CheckChanged(Base sender, EventArgs arguments)
        {
            UpdateMapList();
        }

        void tmpNode_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.WarpTo, ((TreeNode)sender).UserData.ToString(), "");
        }
        public void Update()
        {

        }
        public void Show()
        {
            _adminWindow.IsHidden = false;
        }
        public bool IsVisible()
        {
            return !_adminWindow.IsHidden;
        }
        public void Hide()
        {
            _adminWindow.IsHidden = true;
        }
    }
}
