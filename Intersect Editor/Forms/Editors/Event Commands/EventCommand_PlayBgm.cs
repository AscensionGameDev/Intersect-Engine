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
using Intersect_Editor.Classes.Core;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_PlayBgm : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_PlayBgm(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbBgm.Items.Clear();
            cmbBgm.Items.Add(Strings.Get("general","none"));
            cmbBgm.Items.AddRange(GameContentManager.GetMusicNames());
            if (cmbBgm.Items.IndexOf(_myCommand.Strs[0]) > -1)
            {
                cmbBgm.SelectedIndex = cmbBgm.Items.IndexOf(_myCommand.Strs[0]);
            }
            else
            {
                cmbBgm.SelectedIndex = 0;
            }
        }

        private void InitLocalization()
        {
            grpBGM.Text = Strings.Get("eventplaybgm", "title");
            lblBGM.Text = Strings.Get("eventplaybgm", "label");
            btnSave.Text = Strings.Get("eventplaybgm", "okay");
            btnCancel.Text = Strings.Get("eventplaybgm", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = cmbBgm.Text;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
