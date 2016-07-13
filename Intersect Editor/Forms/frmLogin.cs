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
using System.Diagnostics;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using System.Text;
using System.Security.Cryptography;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Microsoft.Win32;

namespace Intersect_Editor.Forms
{
    public partial class FrmLogin : Form
    {
        //Cross Thread Delegates
        public delegate void BeginEditorLoop();
        public BeginEditorLoop EditorLoopDelegate;
        private string SavedPassword = "";

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            GameContentManager.CheckForResources();
            if (Database.LoadOptions())
            {
                lblVersion.Text = "Editor v." + Application.ProductVersion;
                EditorLoopDelegate = new BeginEditorLoop(EditorLoop.StartLoop);
                if (LoadPreference("username").Trim().Length > 0)
                {
                    txtUsername.Text = LoadPreference("Username");
                    txtPassword.Text = "*****";
                    SavedPassword = LoadPreference("Password");
                    chkRemember.Checked = true;
                }
            }
            else
            {
                MessageBox.Show("Failed to load config.xml. Does it exist?  Path(Resources/config.xml)");
                Application.Exit();
            }
        }

        private void tmrSocket_Tick(object sender, EventArgs e)
        {
            Network.Update();
            var statusString = "Connecting to server...";
            if (Network.Connected)
            {
                statusString = "Connected to server. Ready to login!";
                btnLogin.Enabled = true;
            }
            else if (Network.Connecting)
            {

            }
            else
            {
                statusString = "Failed to connect, retrying in " + ((Globals.ReconnectTime - Environment.TickCount)/1000).ToString("0") + " seconds.";
                btnLogin.Enabled = false;
            }
            Globals.LoginForm.lblStatus.Text = statusString;
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains("raptr"))
                {
                    Globals.LoginForm.lblStatus.Text = "Please close AMD Gaming Evolved before logging into the Intersect editor.";
                    btnLogin.Enabled = false;
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Network.Connected || !btnLogin.Enabled) return;
            if (txtUsername.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
            {
                var sha = new SHA256Managed();
                if (SavedPassword != "")
                {
                    PacketSender.SendLogin(txtUsername.Text.Trim(), SavedPassword);
                }
                else
                {
                    PacketSender.SendLogin(txtUsername.Text.Trim(), BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text.Trim()))).Replace("-", ""));
                }
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar != 13) return;
            e.Handled = true;
            btnLogin_Click(null, null);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }

        public void TryRemembering()
        {
            var sha = new SHA256Managed();
            if (chkRemember.Checked)
            {
                SavePreference("Username", txtUsername.Text);
                if (SavedPassword != "")
                {
                    SavePreference("Password",SavedPassword);
                }
                else
                {
                    SavePreference("Password",BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text.Trim()))).Replace("-", ""));
                }
            }
            else
            {
                SavePreference("Username", "");
                SavePreference("Password","");
            }
        }

        private void SavePreference(string key, string value)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey("Software", true);

            regkey.CreateSubKey("IntersectEditor");
            regkey = regkey.OpenSubKey("IntersectEditor", true);
            regkey.CreateSubKey(Globals.ServerHost + ":" + Globals.ServerPort);
            regkey = regkey.OpenSubKey(Globals.ServerHost + ":" +  Globals.ServerPort, true);
            regkey.SetValue(key, value);
        }

        private string LoadPreference(string key)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey("Software", false);
            regkey = regkey.OpenSubKey("IntersectEditor", false);
            if (regkey == null) { return ""; }
            regkey = regkey.OpenSubKey(Globals.ServerHost + ":" + Globals.ServerPort);
            if (regkey == null) { return ""; }
            string value = (string)regkey.GetValue(key);
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            return value;
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (SavedPassword != "")
            {
                SavedPassword = "";
                txtPassword.Text = "";
                chkRemember.Checked = false;
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (SavedPassword != "")
            {
                SavedPassword = "";
                txtUsername.Text = "";
                txtPassword.Text = "";
                chkRemember.Checked = false;
            }
        }
    }


}
