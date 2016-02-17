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
    public partial class EventCommand_ChangeSpells : UserControl
    {
        private EventCommand _myCommand;
        private EventPage _currentPage;
        private readonly FrmEvent _eventEditor;
        public EventCommand_ChangeSpells(EventCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            _currentPage = refPage;
            cmbSpell.Items.Clear();
            if (cmbSpell.Items.Count == 0)
            {
                for (var i = 0; i < Globals.GameSpells.Count(); i++)
                {
                    cmbSpell.Items.Add((i + 1) + ". " + Globals.GameSpells[i].Name);
                }
            }
            cmbAction.SelectedIndex = _myCommand.Ints[0];
            cmbSpell.SelectedIndex = _myCommand.Ints[1];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = cmbAction.SelectedIndex;
            _myCommand.Ints[1] = cmbSpell.SelectedIndex;
            if (_myCommand.Ints[4] == 0)
            // command.Ints[4, and 5] are reserved for when the action succeeds or fails
            {
                for (var i = 0; i < 2; i++)
                {
                    _currentPage.CommandLists.Add(new CommandList());
                    _myCommand.Ints[4 + i] = _currentPage.CommandLists.Count - 1;
                }
            }
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
