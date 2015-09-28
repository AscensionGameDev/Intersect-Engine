using Gwen;
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
using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Game
{
    class AdminWindow
    {
        //Controls
        private WindowControl _adminWindow;

        private TreeControl _mapList;
        private CheckBox _chkChronological;
        private Label _lblChronological;
        private Label _mapListLabel;

        //Init
        public AdminWindow(Canvas _gameCanvas)
        {
            _adminWindow = new WindowControl(_gameCanvas, "Administration");
            _adminWindow.SetSize(200, 400);
            _adminWindow.SetPosition(Graphics.ScreenWidth / 2 - _adminWindow.Width / 2, Graphics.ScreenHeight / 2 - _adminWindow.Height / 2);
            _adminWindow.DisableResizing();
            _adminWindow.Margin = Margin.Zero;
            _adminWindow.Padding = Padding.Zero;

            CreateMapList();

            _mapListLabel = new Label(_adminWindow);
            _mapListLabel.Text = "Map List: ";
            _mapListLabel.SetPosition(4f, 162);

            _chkChronological = new CheckBox(_adminWindow);
            _chkChronological.SetToolTipText("Order maps chronologically.");
            _chkChronological.SetPosition(_adminWindow.Width - 24, 162);

            _lblChronological = new Label(_adminWindow);
            _lblChronological.Text = "123...";
            _lblChronological.SetPosition(_chkChronological.X - 30, 162);

            UpdateMapList();

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
            AddMapListToTree(Database.MapStructure, null);
        }
        private void AddMapListToTree(MapList mapList, TreeNode parent)
        {
            TreeNode tmpNode;
            if (_chkChronological.IsChecked)
            {
                for (int i = 0; i < Database.OrderedMaps.Count; i++)
                {
                    tmpNode = _mapList.AddNode(Database.OrderedMaps[i].MapNum + ". " + Database.OrderedMaps[i].Name);
                    tmpNode.UserData = (Database.OrderedMaps[i]).MapNum;
                    tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                    //tmpNode.ImageIndex = 1;
                    //tmpNode.SelectedImageIndex = 1;
                }
                //treeMapList.Sort();
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
                        //tmpNode.ImageIndex = 0;
                        //tmpNode.SelectedImageIndex = 0;
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
                            tmpNode = _mapList.AddNode(((FolderMap)mapList.Items[i]).MapNum + ". " + mapList.Items[i].Name);
                            tmpNode.UserData = ((FolderMap)mapList.Items[i]).MapNum;
                            tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                        }
                        //tmpNode.ImageIndex = 1;
                        //tmpNode.SelectedImageIndex = 1;
                    }
                }
            }
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
