/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect_Library;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps.MapList;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Warp : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_Warp(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbMap.Items.Clear();
            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                cmbMap.Items.Add(MapList.GetOrderedMaps()[i].Name);
                if (MapList.GetOrderedMaps()[i].MapNum == _myCommand.Ints[0])
                {
                    cmbMap.SelectedIndex = i;
                }
            }
            if (cmbMap.SelectedIndex == -1)
            {
                cmbMap.SelectedIndex = 0;
            }
            scrlX.Maximum = Options.MapWidth -1;
            scrlY.Maximum = Options.MapHeight - 1;
            scrlX.Value = _myCommand.Ints[1];
            scrlY.Value = _myCommand.Ints[2];
            lblX.Text = Strings.Get("warping", "x", scrlX.Value);
            lblY.Text = Strings.Get("warping", "y", scrlY.Value);
            cmbDirection.SelectedIndex = _myCommand.Ints[3];
        }

        private void InitLocalization()
        {
            grpWarp.Text = Strings.Get("eventwarp", "title");
            lblMap.Text = Strings.Get("warping", "map", "");
            lblX.Text = Strings.Get("warping", "x", scrlX.Value);
            lblY.Text = Strings.Get("warping", "y", scrlY.Value);
            lblDir.Text = Strings.Get("warping", "direction", "");
            btnVisual.Text = Strings.Get("warping", "visual");
            cmbDirection.Items.Clear();
            for (int i = -1; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Get("directions", i.ToString()));
            }
            btnSave.Text = Strings.Get("eventwarp", "okay");
            btnCancel.Text = Strings.Get("eventwarp", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = MapList.GetOrderedMaps()[cmbMap.SelectedIndex].MapNum;
            _myCommand.Ints[1] = scrlX.Value;
            _myCommand.Ints[2] = scrlY.Value;
            _myCommand.Ints[3] = cmbDirection.SelectedIndex;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void scrlX_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblX.Text = Strings.Get("warping", "x", scrlX.Value);
        }

        private void scrlY_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblY.Text = Strings.Get("warping", "y", scrlY.Value);
        }

        private void btnVisual_Click(object sender, EventArgs e)
        {
            frmWarpSelection frmWarpSelection = new frmWarpSelection();
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbMap.SelectedIndex].MapNum,scrlX.Value,scrlY.Value);
            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapNum == frmWarpSelection.GetMap())
                    {
                        cmbMap.SelectedIndex = i;
                        break;
                    }
                }
                scrlX.Value = frmWarpSelection.GetX();
                scrlY.Value = frmWarpSelection.GetY();
                lblX.Text = Strings.Get("warping", "x", scrlX.Value);
                lblY.Text = Strings.Get("warping", "y", scrlY.Value);
            }
        }

        private void lblY_Click(object sender, EventArgs e)
        {

        }

        private void lblX_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbMap_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }
    }
}
