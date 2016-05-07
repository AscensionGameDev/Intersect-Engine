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

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_ChangeSprite : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_ChangeSprite(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            cmbSprite.Items.Clear();
            cmbSprite.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Entity));
            if (cmbSprite.Items.IndexOf(_myCommand.Strs[0]) > -1)
            {
                cmbSprite.SelectedIndex = cmbSprite.Items.IndexOf(_myCommand.Strs[0]);
            }
            else
            {
                cmbSprite.SelectedIndex = 0;
            }
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            Bitmap destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(destBitmap);
            g.Clear(Color.Black);
            if (File.Exists("resources/entities/" + cmbSprite.Text))
            {
                Bitmap sourceBitmap = new Bitmap("resources/entities/" + cmbSprite.Text);
                g.DrawImage(sourceBitmap,new Rectangle(pnlPreview.Width/2 - sourceBitmap.Width/8,pnlPreview.Height/2 - sourceBitmap.Height/8,sourceBitmap.Width/4,sourceBitmap.Height/4), new Rectangle(0,0,sourceBitmap.Width/4,sourceBitmap.Height/4),GraphicsUnit.Pixel);
            }
            g.Dispose();
            pnlPreview.BackgroundImage = destBitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = cmbSprite.Text;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }
    }
}
