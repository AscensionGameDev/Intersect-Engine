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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Text : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_Text(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            txtShowText.Text = _myCommand.Strs[0];
            cmbFace.Items.Clear();
            cmbFace.Items.Add("None");
            cmbFace.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Face));
            if (cmbFace.Items.IndexOf(_myCommand.Strs[1]) > -1)
            {
                cmbFace.SelectedIndex = cmbFace.Items.IndexOf(_myCommand.Strs[1]);
            }
            else
            {
                cmbFace.SelectedIndex = 0;
            }
            UpdateFacePreview();
        }

        private void InitLocalization()
        {
            grpShowText.Text = Strings.Get("eventshowtext", "title");
            lblText.Text = Strings.Get("eventshowtext", "text");
            lblFace.Text = Strings.Get("eventshowtext", "face");
            lblCommands.Text = Strings.Get("eventshowtext", "commands");
            btnSave.Text = Strings.Get("eventshowtext", "okay");
            btnCancel.Text = Strings.Get("eventshowtext", "cancel");
        }

        private void UpdateFacePreview()
        {
            if (File.Exists("resources/faces/" + cmbFace.Text))
            {
                pnlFace.BackgroundImage = new Bitmap("resources/faces/" + cmbFace.Text);
            }
            else
            {
                pnlFace.BackgroundImage = null;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = txtShowText.Text;
            _myCommand.Strs[1] = cmbFace.Text;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void cmbFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFacePreview();
        }

        private void lblCommands_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
        }
    }
}
