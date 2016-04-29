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
using Intersect_Editor.Classes.General;
using Intersect_Library;

namespace Intersect_Editor.Forms.Editors
{
    public partial class frmCommonEvent : Form
    {
        public frmCommonEvent()
        {
            InitializeComponent();
            ListCommonEvents();
        }

        private void ListCommonEvents()
        {
            lstCommonEvents.Items.Clear();
            for (int i = 0; i < Options.MaxCommonEvents; i++)
            {
                lstCommonEvents.Items.Add((i + 1) + ". " + Globals.CommonEvents[i].MyName);
            }
        }

        private void lstCommonEvents_DoubleClick(object sender, EventArgs e)
        {
            if (lstCommonEvents.SelectedIndex > -1)
            {
                FrmEvent editor = new FrmEvent(null);
                editor.MyEvent = Globals.CommonEvents[lstCommonEvents.SelectedIndex];
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
