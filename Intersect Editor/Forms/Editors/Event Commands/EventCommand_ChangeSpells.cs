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
using Intersect_Library.Localization;

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
            InitLocalization();
            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObject.Spell));
            cmbAction.SelectedIndex = _myCommand.Ints[0];
            cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObject.Spell,_myCommand.Ints[1]);
        }

        private void InitLocalization()
        {
            grpChangeSpells.Text = Strings.Get("eventchangespells", "title");
            cmbAction.Items.Clear();
            for (int i = 0; i < 2; i++)
            {
                cmbAction.Items.Add(Strings.Get("eventchangespells", "action" + i));
            }
            lblAction.Text = Strings.Get("eventchangespells", "action");
            lblSpell.Text = Strings.Get("eventchangespells", "spell");
            btnSave.Text = Strings.Get("eventchangespells", "okay");
            btnCancel.Text = Strings.Get("eventchangespells", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = cmbAction.SelectedIndex;
            _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.Spell,cmbSpell.SelectedIndex);
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
