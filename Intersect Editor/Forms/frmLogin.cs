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

namespace Intersect_Editor.Forms
{
    public partial class FrmLogin : Form
    {
        //Cross Thread Delegates
        public delegate void BeginEditorLoop();
        public BeginEditorLoop EditorLoopDelegate;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            Database.InitDatabase();
            lblVersion.Text = "Editor v." + Application.ProductVersion;
            EditorLoopDelegate = new BeginEditorLoop(EditorLoop.StartLoop);
        }

        private void tmrSocket_Tick(object sender, EventArgs e)
        {
            Network.Update();
            var statusString = "Connecting to server...";
            if (Network.Connected)
            {
                statusString = "Connected to server.";
            }
            else if (Network.Connecting)
            {

            }
            else
            {
                statusString = "Failed to connect, retrying in " + ((Globals.ReconnectTime - Environment.TickCount)/1000).ToString("0") + " seconds.";
            }
            Globals.LoginForm.lblStatus.Text = statusString;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Network.Connected) return;
            if (txtUsername.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
            {
                PacketSender.SendLogin(txtUsername.Text.Trim(), txtPassword.Text.Trim());
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar != 13) return;
            e.Handled = true;
            if (txtUsername.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
            {
                if (!Network.Connected) return;
                PacketSender.SendLogin(txtUsername.Text.Trim(), txtPassword.Text.Trim());
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }
    }


}
