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
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void lblWebsite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ascensiongamedev.com");
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            InitLocalization();
        }

        private void InitLocalization()
        {
            this.Text = Strings.Get("about", "title");
            lblVersion.Text = Strings.Get("about","version", Application.ProductVersion);
            lblWebsite.Text = Strings.Get("about", "site");
        }
    }
}
