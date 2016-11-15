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
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects.Events;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_StartQuest : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_StartQuest(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            cmbQuests.Items.Clear();
            cmbQuests.Items.AddRange(Database.GetGameObjectList(GameObject.Quest));
            cmbQuests.SelectedIndex = Database.GameObjectListIndex(GameObject.Quest, refCommand.Ints[0]);
            chkShowOfferWindow.Checked = Convert.ToBoolean(refCommand.Ints[1]);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObject.Quest, cmbQuests.SelectedIndex);
            _myCommand.Ints[1] = Convert.ToInt32(chkShowOfferWindow.Checked);
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
