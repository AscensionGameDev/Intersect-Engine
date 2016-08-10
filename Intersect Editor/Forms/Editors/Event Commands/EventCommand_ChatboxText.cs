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
    public partial class EventCommand_ChatboxText : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_ChatboxText(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            txtAddText.Text = _myCommand.Strs[0];
            cmbColor.Items.Clear();
            foreach (Color.ChatColor color in Enum.GetValues(typeof(Color.ChatColor)))
            {
                cmbColor.Items.Add(Globals.GetColorName(color));
            }
            cmbColor.SelectedIndex = cmbColor.Items.IndexOf(_myCommand.Strs[1]);
            if (cmbColor.SelectedIndex == -1) cmbColor.SelectedIndex = 0;
            cmbChannel.SelectedIndex = _myCommand.Ints[0];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = txtAddText.Text;
            _myCommand.Strs[1] = cmbColor.Text;
            _myCommand.Ints[0] = cmbChannel.SelectedIndex;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void lblCommands_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
        }
    }
}
