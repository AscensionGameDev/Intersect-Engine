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

namespace Intersect_Client.Classes.UI.Game
{
    class AdminWindow
    {
        //Controls
        private WindowControl _adminWindow;

        //Player Mod Labels
        private Label _nameLabel;
        private Label _reasonLabel;
        private Label _durationLabel;
        private Label _accessLabel;
        private Label _spriteLabel;

        //Player Mod Textboxes
        private TextBox _nameTextbox;
        private TextBox _reasonTextbox;
        private TextBox _durationTextbox;
        private TextBox _spriteTextbox;
        private TextBox _accessTextbox;

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

        //Admin Powers
        private Label _noclipLabel;
        private CheckBox _noclipCheckBox;
        private CheckBox _chkIPBan;
        private Label _IPBanLabel;

        private TreeControl _mapList;
        private CheckBox _chkChronological;
        private Label _lblChronological;
        private Label _mapListLabel;

        //Init
        public AdminWindow(Canvas _gameCanvas)
        {
            _adminWindow = new WindowControl(_gameCanvas, "Administration");
            _adminWindow.SetSize(200, 540);
            _adminWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - _adminWindow.Width / 2, GameGraphics.Renderer.GetScreenHeight() / 2 - _adminWindow.Height / 2);
            _adminWindow.DisableResizing();
            _adminWindow.Margin = Margin.Zero;
            _adminWindow.Padding = Padding.Zero;

            //Player Mods
            _nameLabel = new Label(_adminWindow);
            _nameLabel.SetPosition(4,4);
            _nameLabel.Text = "Name:";

            _nameTextbox = new TextBox(_adminWindow);
            _nameTextbox.SetBounds(4,16,100,18);

            _kickButton = new Button(_adminWindow);
            _kickButton.Text = "Kick Player";
            _kickButton.SetBounds(4,38,80,18);
            _kickButton.Clicked += _kickButton_Clicked;

            _killButton = new Button(_adminWindow);
            _killButton.Text = "Kill Player";
            _killButton.SetBounds(88,38,80,18);
            _killButton.Clicked += _killButton_Clicked;

            _warpToMeButton = new Button(_adminWindow);
            _warpToMeButton.Text = "Warp to Me";
            _warpToMeButton.SetBounds(4,60,80,18);
            _warpToMeButton.Clicked += _warpToMeButton_Clicked;

            _warpMeToButton = new Button(_adminWindow);
            _warpMeToButton.Text = "Warp Me To";
            _warpMeToButton.SetBounds(88,60,80,18);
            _warpMeToButton.Clicked += _warpMeToButton_Clicked;

            _durationLabel = new Label(_adminWindow);
            _durationLabel.SetPosition(4, 82);
            _durationLabel.Text = "Duration (days):";

            _durationTextbox = new TextBox(_adminWindow);
            _durationTextbox.SetBounds(4, 94, 100, 18);

            _reasonLabel = new Label(_adminWindow);
            _reasonLabel.SetPosition(108, 82);
            _reasonLabel.Text = "Reason:";

            _reasonTextbox = new TextBox(_adminWindow);
            _reasonTextbox.SetBounds(108, 94, 100, 18);

            _banButton = new Button(_adminWindow);
            _banButton.Text = "Ban Player";
            _banButton.SetBounds(4, 116, 80, 18);
            _banButton.Clicked += _banButton_Clicked;

            _unbanButton = new Button(_adminWindow);
            _unbanButton.Text = "Unban Account";
            _unbanButton.SetBounds(88, 116, 90, 18);
            _unbanButton.Clicked += _unbanButton_Clicked;

            _muteButton = new Button(_adminWindow);
            _muteButton.Text = "Mute Player";
            _muteButton.SetBounds(4, 138, 80, 18);
            _muteButton.Clicked += _muteButton_Clicked;

            _unmuteButton = new Button(_adminWindow);
            _unmuteButton.Text = "Unmute Player";
            _unmuteButton.SetBounds(88, 138, 90, 18);
            _unmuteButton.Clicked += _unmuteButton_Clicked;

            _spriteLabel = new Label(_adminWindow);
            _spriteLabel.SetPosition(4, 160);
            _spriteLabel.Text = "Sprite:";

            _spriteTextbox = new TextBox(_adminWindow);
            _spriteTextbox.SetBounds(4, 172, 100, 18);

            _IPBanLabel = new Label(_adminWindow);
            _IPBanLabel.Text = "IP Ban?";
            _IPBanLabel.SetPosition(108, 172);

            _chkIPBan = new CheckBox(_adminWindow);
            _chkIPBan.SetPosition(108 + _IPBanLabel.Width, 172);

            _setSpriteButton = new Button(_adminWindow);
            _setSpriteButton.Text = "Set Sprite";
            _setSpriteButton.SetBounds(4, 194, 80, 18);
            _setSpriteButton.Clicked += _setSpriteButton_Clicked;

            _accessLabel = new Label(_adminWindow);
            _accessLabel.SetPosition(4, 216);
            _accessLabel.Text = "Access:";

            _accessTextbox = new TextBox(_adminWindow);
            _accessTextbox.SetBounds(4, 228, 100, 18);

            _setPowerButton = new Button(_adminWindow);
            _setPowerButton.Text = "Set Power";
            _setPowerButton.SetBounds(4, 250, 80, 18);
            _setPowerButton.Clicked += _setPowerButton_Clicked;

            _noclipLabel = new Label(_adminWindow);
            _noclipLabel.Text = "No Clip";
            _noclipLabel.SetPosition(4, 272);

            _noclipCheckBox = new CheckBox(_adminWindow);
            _noclipCheckBox.SetPosition(12 + _noclipLabel.Width, 272);
            _noclipCheckBox.IsChecked = Globals.Me.NoClip;
            _noclipCheckBox.CheckChanged += _noclipCheckBox_CheckChanged;

            CreateMapList();
            _mapListLabel = new Label(_adminWindow);
            _mapListLabel.Text = "Map List: ";
            _mapListLabel.SetPosition(4f, 294);

            _chkChronological = new CheckBox(_adminWindow);
            _chkChronological.SetToolTipText("Order maps chronologically.");
            _chkChronological.SetPosition(_adminWindow.Width - 24, 294);
            _chkChronological.CheckChanged += _chkChronological_CheckChanged;

            _lblChronological = new Label(_adminWindow);
            _lblChronological.Text = "123...";
            _lblChronological.SetPosition(_chkChronological.X - 30, 294);

            UpdateMapList();
        }

        void _noclipCheckBox_CheckChanged(Base sender, EventArgs arguments)
        {
            Globals.Me.NoClip = _noclipCheckBox.IsChecked;
        }

        //Methods
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
            PacketSender.SendAdminAction((int)AdminActions.Kick, _nameTextbox.Text);
        }

        void _killButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.Kill, _nameTextbox.Text);
        }

        void _warpToMeButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.WarpToMe, _nameTextbox.Text);
        }

        void _warpMeToButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.WarpMeTo, _nameTextbox.Text);
        }

        void _muteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.Mute, _nameTextbox.Text, _durationTextbox.Text, _reasonTextbox.Text, Convert.ToString(_chkIPBan.IsChecked));
        }

        void _banButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.Ban, _nameTextbox.Text, _durationTextbox.Text, _reasonTextbox.Text, Convert.ToString(_chkIPBan.IsChecked));
        }

        void _unmuteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.UnMute, _nameTextbox.Text);
        }

        void _unbanButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.UnBan, _nameTextbox.Text);
        }

        void _setSpriteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.SetSprite, _nameTextbox.Text, _spriteTextbox.Text);
        }

        void _setPowerButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.SetAccess, _nameTextbox.Text, _accessTextbox.Text);
        }

        void _chkChronological_CheckChanged(Base sender, EventArgs arguments)
        {
            UpdateMapList();
        }

        void tmpNode_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)AdminActions.WarpTo, (string)((TreeNode)sender).UserData, "");
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
