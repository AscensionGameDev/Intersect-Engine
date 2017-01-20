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
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_SelfSwitch : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_SelfSwitch(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbSetSwitch.SelectedIndex = _myCommand.Ints[0];
            cmbSetSwitchVal.SelectedIndex = _myCommand.Ints[1];
        }

        private void InitLocalization()
        {
            grpSelfSwitch.Text = Strings.Get("eventselfswitch", "title");
            lblSelfSwitch.Text = Strings.Get("eventselfswitch", "label");
            cmbSetSwitch.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                cmbSetSwitch.Items.Add(Strings.Get("eventselfswitch", "selfswitch" + i));
            }
            cmbSetSwitchVal.Items.Clear();
            cmbSetSwitchVal.Items.Add(Strings.Get("eventselfswitch", "false"));
            cmbSetSwitchVal.Items.Add(Strings.Get("eventselfswitch", "true"));
            btnSave.Text = Strings.Get("eventselfswitch", "okay");
            btnCancel.Text = Strings.Get("eventselfswitch", "cancel");
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
