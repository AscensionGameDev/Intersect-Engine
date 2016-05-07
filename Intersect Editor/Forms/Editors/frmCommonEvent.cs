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

namespace Intersect_Editor.Forms.Editors
{
    public partial class frmCommonEvent : Form
    {
        public frmCommonEvent()
        {
            InitializeComponent();
            ListCommonEvents();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.CommonEvent)
            {
                ListCommonEvents();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.CommonEvent);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstCommonEvents.SelectedIndex > -1 && EventBase.GetEvent(Database.GameObjectIdFromList(GameObject.CommonEvent,lstCommonEvents.SelectedIndex)) != null)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(EventBase.GetEvent(Database.GameObjectIdFromList(GameObject.CommonEvent, lstCommonEvents.SelectedIndex)));
                }
            }
        }

        private void ListCommonEvents()
        {
            lstCommonEvents.Items.Clear();
            lstCommonEvents.Items.AddRange(Database.GetGameObjectList(GameObject.CommonEvent));
        }

        private void lstCommonEvents_DoubleClick(object sender, EventArgs e)
        {
            if (lstCommonEvents.SelectedIndex > -1)
            {
                FrmEvent editor = new FrmEvent(null);
                editor.MyEvent = EventBase.GetEvent(Database.GameObjectIdFromList(GameObject.CommonEvent, lstCommonEvents.SelectedIndex));
                editor.InitEditor();
                editor.ShowDialog();
                ListCommonEvents();
            }
        }

        private void frmCommonEvent_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }
    }
}
