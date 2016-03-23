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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Intersect_Editor.Forms
{
    public partial class frmProgress : Form
    {
        private string statusText;
        private int progressVal;
        private Boolean showCancelBtn;
        private Boolean shouldClose;
        public frmProgress()
        {
            InitializeComponent();
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }
        public void SetProgress(string label, int progress, bool showCancel)
        {
            statusText = label;
            progressVal = progress;
            showCancelBtn = showCancel;
            Application.DoEvents();
        }

        public void NotifyClose()
        {
            shouldClose = true;
        }

        private void tmrUpdater_Tick(object sender, EventArgs e)
        {
            lblStatus.Text = statusText;
            progressBar.Value = progressVal;
            btnCancel.Visible = showCancelBtn;
            if (shouldClose)
            {
                Close();
            }
        }
    }
}
