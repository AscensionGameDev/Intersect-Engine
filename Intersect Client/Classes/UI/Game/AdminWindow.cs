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
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    class AdminWindow
    {
        //Controls
        private WindowControl _adminWindow;

        //Player Mods
        private Label _nameLabel;
        private TextBox _nameTextbox;
        private Button _kickButton;
        private Button _banButton;
        private Button _warpToMeButton;
        private Button _warpMeToButton;

        //Admin Powers
        private Label _noclipLabel;
        private CheckBox _noclipCheckBox;

        private TreeControl _mapList;
        private CheckBox _chkChronological;
        private Label _lblChronological;
        private Label _mapListLabel;

        //Init
        public AdminWindow(Canvas _gameCanvas)
        {
            _adminWindow = new WindowControl(_gameCanvas, "Administration");
            _adminWindow.SetSize(200, 400);
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

            _banButton = new Button(_adminWindow);
            _banButton.Text = "Ban Player";
            _banButton.SetBounds(88,38,80,18);

            _warpMeToButton = new Button(_adminWindow);
            _warpMeToButton.Text = "Warp to Me";
            _warpMeToButton.SetBounds(4,60,80,18);

            _warpMeToButton = new Button(_adminWindow);
            _warpMeToButton.Text = "Warp Me To";
            _warpMeToButton.SetBounds(88,60,80,18);

            _noclipLabel = new Label(_adminWindow);
            _noclipLabel.Text = "No Clip";
            _noclipLabel.SetPosition(4, 84);

            _noclipCheckBox = new CheckBox(_adminWindow);
            _noclipCheckBox.SetPosition(12 + _noclipLabel.Width, 84);
            _noclipCheckBox.IsChecked = Globals.Me.NoClip;
            _noclipCheckBox.CheckChanged += _noclipCheckBox_CheckChanged;






            CreateMapList();
            _mapListLabel = new Label(_adminWindow);
            _mapListLabel.Text = "Map List: ";
            _mapListLabel.SetPosition(4f, 162);

            _chkChronological = new CheckBox(_adminWindow);
            _chkChronological.SetToolTipText("Order maps chronologically.");
            _chkChronological.SetPosition(_adminWindow.Width - 24, 162);
            _chkChronological.CheckChanged += _chkChronological_CheckChanged;

            _lblChronological = new Label(_adminWindow);
            _lblChronological.Text = "123...";
            _lblChronological.SetPosition(_chkChronological.X - 30, 162);

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
            _mapList.SetPosition(4f, 180);
            _mapList.Height = 188;
            _mapList.Width = _adminWindow.Width - 8;
        }
        public void UpdateMapList()
        {
            _mapList.Dispose();
            CreateMapList();
            AddMapListToTree(Globals.MapStructure, null);
        }
        private void AddMapListToTree(MapList mapList, TreeNode parent)
        {
            TreeNode tmpNode;
            if (_chkChronological.IsChecked)
            {
                for (int i = 0; i < Globals.OrderedMaps.Count; i++)
                {
                    tmpNode = _mapList.AddNode(Globals.OrderedMaps[i].MapNum + ". " + Globals.OrderedMaps[i].Name);
                    tmpNode.UserData = (Globals.OrderedMaps[i]).MapNum;
                    tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                }
            }
            else
            {
                for (int i = 0; i < mapList.Items.Count; i++)
                {
                    if (mapList.Items[i].GetType() == typeof(FolderDirectory))
                    {
                        if (parent == null)
                        {
                            tmpNode = _mapList.AddNode(mapList.Items[i].Name);
                            tmpNode.UserData = ((FolderDirectory)mapList.Items[i]);
                            AddMapListToTree(((FolderDirectory)mapList.Items[i]).Children, tmpNode);
                        }
                        else
                        {
                            tmpNode = parent.AddNode(mapList.Items[i].Name);
                            tmpNode.UserData = ((FolderDirectory)mapList.Items[i]);
                            AddMapListToTree(((FolderDirectory)mapList.Items[i]).Children, tmpNode);
                        }
                    }
                    else
                    {
                        if (parent == null)
                        {
                            tmpNode = _mapList.AddNode(((FolderMap)mapList.Items[i]).MapNum + ". " + mapList.Items[i].Name);
                            tmpNode.UserData = ((FolderMap)mapList.Items[i]).MapNum;
                            tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                        }
                        else
                        {
                            tmpNode = parent.AddNode(((FolderMap)mapList.Items[i]).MapNum + ". " + mapList.Items[i].Name);
                            tmpNode.UserData = ((FolderMap)mapList.Items[i]).MapNum;
                            tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                        }
                    }
                }
            }
        }

        void _chkChronological_CheckChanged(Base sender, EventArgs arguments)
        {
            UpdateMapList();
        }

        void tmpNode_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction((int)Enums.AdminActions.WarpTo, (int)((TreeNode)sender).UserData);
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
