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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Switch : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_Switch(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            //TODO Everything
            if (cmbSetSwitch.Items.Count == 0)
            {
                for (var i = 0; i < Constants.SwitchCount; i++)
                {
                    cmbSetSwitch.Items.Add("Switch " + (i + 1));
                }
            }
            if (_myCommand.Ints[0] > 0)
            {
                cmbSetSwitch.SelectedIndex = _myCommand.Ints[0];
            }
            else
            {
                cmbSetSwitch.SelectedIndex = 0;
                _myCommand.Ints[0] = 1;
            }
            cmbSetSwitchVal.SelectedIndex = _myCommand.Ints[1];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = cmbSetSwitch.SelectedIndex;
            _myCommand.Ints[1] = cmbSetSwitchVal.SelectedIndex;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
