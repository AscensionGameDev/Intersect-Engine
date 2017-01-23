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
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;

namespace Intersect_Editor.Forms.Editors.Quest
{
    public partial class Quest_TaskEditor : UserControl
    {
        private QuestBase.QuestTask _myTask;
        public bool Cancelled = false;
        private ByteBuffer _eventBackup = new ByteBuffer();
        public Quest_TaskEditor(QuestBase.QuestTask refTask)
        {
            InitializeComponent();
            _myTask = refTask;
            _eventBackup.WriteBytes(_myTask.CompletionEvent.EventData());
            cmbConditionType.SelectedIndex = _myTask.Objective;
            txtStartDesc.Text = _myTask.Desc;
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Event Driven
                    break;
                case 1: //Gather Items
                    cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObject.Item, _myTask.Data1);
                    scrlItemQuantity.Value = _myTask.Data2;
                    lblItemQuantity.Text = "Amount: " + scrlItemQuantity.Value;
                    break;
                case 2: //Kill NPCS
                    cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObject.Item, _myTask.Data1);
                    scrlNpcQuantity.Value = _myTask.Data2;
                    lblNpcQuantity.Text = "Amount: " + scrlNpcQuantity.Value;
                    break;
            }
        }

        private void UpdateFormElements()
        {
            grpGatherItems.Hide();
            grpKillNpcs.Hide();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Event Driven
                    break;
                case 1: //Gather Items
                    grpGatherItems.Show();
                    cmbItem.Items.Clear();
                    cmbItem.Items.AddRange(Database.GetGameObjectList(GameObject.Item));
                    if (cmbItem.Items.Count > 0) cmbItem.SelectedIndex = 0;
                    scrlItemQuantity.Value = 1;
                    break;
                case 2: //Kill Npcs
                    grpKillNpcs.Show();
                    cmbNpc.Items.Clear();
                    cmbNpc.Items.AddRange(Database.GetGameObjectList(GameObject.Npc));
                    if (cmbNpc.Items.Count > 0) cmbNpc.SelectedIndex = 0;
                    scrlNpcQuantity.Value = 1;
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myTask.Objective = cmbConditionType.SelectedIndex;
            _myTask.Desc = txtStartDesc.Text;
            switch (_myTask.Objective)
            {
                case 0: //Event Driven
                    _myTask.Data1 = 0;
                    _myTask.Data2 = 1;
                    break;
                case 1: //Gather Items
                    _myTask.Data1 = Database.GameObjectIdFromList(GameObject.Item, cmbItem.SelectedIndex);
                    _myTask.Data2 = scrlItemQuantity.Value;
                    break;
                case 2: //Kill Npcs
                    _myTask.Data1 = Database.GameObjectIdFromList(GameObject.Npc, cmbNpc.SelectedIndex);
                    _myTask.Data2 = scrlNpcQuantity.Value;
                    break;
            }
            ParentForm.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            _myTask.CompletionEvent = new EventBase(-1, _eventBackup);
            ParentForm.Close();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void scrlItemQuantity_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblItemQuantity.Text = "Amount: " + scrlItemQuantity.Value;
        }

        private void scrlNpcQuantity_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblNpcQuantity.Text = "Amount: " + scrlNpcQuantity.Value;
        }

        private void btnEditTaskEvent_Click(object sender, EventArgs e)
        {
            _myTask.CompletionEvent.MyName = "Task Completion Event";
            FrmEvent editor = new FrmEvent(null);
            editor.MyEvent = _myTask.CompletionEvent;
            editor.InitEditor();
            editor.ShowDialog();
            Globals.MainForm.BringToFront();
            BringToFront();
        }
    }
}
