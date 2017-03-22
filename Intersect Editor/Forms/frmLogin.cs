
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using System.Text;
using System.Security.Cryptography;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.Localization;

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
                Strings.Init(Strings.IntersectComponent.Editor,Options.Language);
                EditorLoopDelegate = EditorLoop.StartLoop;
                if (Preferences.LoadPreference("username").Trim().Length > 0)
                {
                    txtUsername.Text = Preferences.LoadPreference("Username");
                    txtPassword.Text = "*****";
                    SavedPassword = Preferences.LoadPreference("Password");
                    chkRemember.Checked = true;
                }
                Database.InitMapCache();
                InitLocalization();
            }
            else
            {
                MessageBox.Show("Failed to load config.xml. Does it exist?  Path(Resources/config.xml)");
                Application.Exit();
            }
        }

        private void InitLocalization()
        {
            Text = Strings.Get("login", "title");
            lblVersion.Text = Strings.Get("login", "version", Application.ProductVersion);
            lblGettingStarted.Text = Strings.Get("login", "gettingstarted");
            lblUsername.Text = Strings.Get("login", "username");
            lblPassword.Text = Strings.Get("login", "password");
            chkRemember.Text = Strings.Get("login", "rememberme");
            btnLogin.Text = Strings.Get("login", "login");
        }

        private void tmrSocket_Tick(object sender, EventArgs e)
        {
            Network.Update();
            var statusString = Strings.Get("login", "connecting");
            if (Network.Connected)
            {
                statusString = Strings.Get("login", "connected");
                btnLogin.Enabled = true;
            }
            else if (Network.Connecting)
            {

            }
            else
            {
                statusString = Strings.Get("login", "failedtoconnect",((Globals.ReconnectTime - Globals.System.GetTimeMs())/1000).ToString("0"));
                btnLogin.Enabled = false;
            }
            Globals.LoginForm.lblStatus.Text = statusString;
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains("raptr"))
                {
                    Globals.LoginForm.lblStatus.Text = Strings.Get("login", "raptr");
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
                Preferences.SavePreference("Username", txtUsername.Text);
                if (SavedPassword != "")
                {
                    Preferences.SavePreference("Password",SavedPassword);
                }
                else
                {
                    Preferences.SavePreference("Password",BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text.Trim()))).Replace("-", ""));
                }
            }
            else
            {
                Preferences.SavePreference("Username", "");
                Preferences.SavePreference("Password","");
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) return;
            if (SavedPassword != "")
            {
                SavedPassword = "";
                txtPassword.Text = "";
                chkRemember.Checked = false;
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) return;
            if (SavedPassword != "")
            {
                SavedPassword = "";
                txtUsername.Text = "";
                txtPassword.Text = "";
                chkRemember.Checked = false;
            }
        }

        private void FrmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                new frmOptions().ShowDialog();
            }
        }
    }


}
