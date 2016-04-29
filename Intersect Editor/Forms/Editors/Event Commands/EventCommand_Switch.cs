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
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects.Events;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Switch : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        private bool _loading = false;
        public EventCommand_Switch(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            _loading = true;
            if (_myCommand.Ints[0] == (int) SwitchVariableTypes.ServerSwitch)
            {
                rdoGlobalSwitch.Checked = true;
            }
            _loading = false;
            InitEditor();

        }

        private void InitEditor()
        {
            cmbSetSwitch.Items.Clear();
            int switchCount = 0;
            if (rdoPlayerSwitch.Checked)
            {
                for (var i = 0; i < Options.MaxPlayerSwitches; i++)
                {
                    cmbSetSwitch.Items.Add((i + 1) + ". " + Globals.PlayerSwitches[i]);
                }
                switchCount = Options.MaxPlayerSwitches;
            }
            else
            {
                for (var i = 0; i < Options.MaxServerSwitches; i++)
                {
                    cmbSetSwitch.Items.Add((i + 1) + ". " + Globals.ServerSwitches[i]);
                }
                switchCount = Options.MaxServerSwitches;
            }
            if (_myCommand.Ints[1] >= 0 && _myCommand.Ints[1] < switchCount)
            {
                cmbSetSwitch.SelectedIndex = _myCommand.Ints[1];
            }
            else
            {
                cmbSetSwitch.SelectedIndex = 0;
                _myCommand.Ints[1] = 0;
            }
            cmbSetSwitchVal.SelectedIndex = _myCommand.Ints[2];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (rdoPlayerSwitch.Checked) _myCommand.Ints[0] = (int) SwitchVariableTypes.PlayerSwitch;
            if (rdoGlobalSwitch.Checked) _myCommand.Ints[0] = (int)SwitchVariableTypes.ServerSwitch;
            _myCommand.Ints[1] = cmbSetSwitch.SelectedIndex;
            _myCommand.Ints[2] = cmbSetSwitchVal.SelectedIndex;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void rdoPlayerSwitch_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!_loading) cmbSetSwitch.SelectedIndex = 0;
            if (!_loading) cmbSetSwitchVal.SelectedIndex = 0;
        }

        private void rdoGlobalSwitch_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!_loading) cmbSetSwitch.SelectedIndex = 0;
            if (!_loading) cmbSetSwitchVal.SelectedIndex = 0;
        }
    }
}
