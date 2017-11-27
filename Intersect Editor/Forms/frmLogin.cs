using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Intersect.Config;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms
{
    public partial class FrmLogin : Form
    {
        //Cross Thread Delegates
        public delegate void BeginEditorLoop();

        public BeginEditorLoop EditorLoopDelegate;
        private string mSavedPassword = "";
        private bool mOptionsLoaded = false;

        public FrmLogin()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
            GameContentManager.CheckForResources();
            Database.LoadOptions();
            optionsLoaded = true;
            Strings.Load(ClientOptions.Language);
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

        private void InitLocalization()
        {
            Text = Strings.Login.title;
            lblVersion.Text = Strings.Login.version.ToString( Application.ProductVersion);
            lblGettingStarted.Text = Strings.Login.gettingstarted;
            lblUsername.Text = Strings.Login.username;
            lblPassword.Text = Strings.Login.password;
            chkRemember.Text = Strings.Login.rememberme;
            btnLogin.Text = Strings.Login.login;
            lblStatus.Text = Strings.Login.connecting;
        }

        private void tmrSocket_Tick(object sender, EventArgs e)
        {
            if (!mOptionsLoaded) return;
            EditorNetwork.Update();
            var statusString = Strings.Login.connecting;
            if (EditorNetwork.Connected)
            {
                statusString = Strings.Login.connected;
                btnLogin.Enabled = true;
            }
            else if (EditorNetwork.Connecting)
            {
            }
            else
            {
                statusString = Strings.Login.failedtoconnect.ToString(
                    ((Globals.ReconnectTime - Globals.System.GetTimeMs()) / 1000).ToString("0"));
                btnLogin.Enabled = false;
            }
            Globals.LoginForm.lblStatus.Text = statusString;
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains("raptr"))
                {
                    Globals.LoginForm.lblStatus.Text = Strings.Login.raptr;
                    btnLogin.Enabled = false;
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!EditorNetwork.Connected || !btnLogin.Enabled) return;
            if (txtUsername.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
            {
                var sha = new SHA256Managed();
                if (mSavedPassword != "")
                {
                    PacketSender.SendLogin(txtUsername.Text.Trim(), mSavedPassword);
                }
                else
                {
                    PacketSender.SendLogin(txtUsername.Text.Trim(),
                        BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text.Trim())))
                            .Replace("-", ""));
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
            EditorNetwork.EditorLidgrenNetwork?.Disconnect("quitting");
            base.OnClosed(e);
            Application.Exit();
        }

        public void TryRemembering()
        {
            var sha = new SHA256Managed();
            if (chkRemember.Checked)
            {
                Preferences.SavePreference("Username", txtUsername.Text);
                if (mSavedPassword != "")
                {
                    Preferences.SavePreference("Password", mSavedPassword);
                }
                else
                {
                    Preferences.SavePreference("Password",
                        BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text.Trim())))
                            .Replace("-", ""));
                }
            }
            else
            {
                Preferences.SavePreference("Username", "");
                Preferences.SavePreference("Password", "");
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) return;
            if (mSavedPassword != "")
            {
                mSavedPassword = "";
                txtPassword.Text = "";
                chkRemember.Checked = false;
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) return;
            if (mSavedPassword != "")
            {
                mSavedPassword = "";
                txtUsername.Text = "";
                txtPassword.Text = "";
                chkRemember.Checked = false;
            }
        }

        private void FrmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                new FrmOptions().ShowDialog();
            }
        }

        public void HideSafe() => ShowSafe(false);

        public void ShowSafe(bool show = true)
        {
            var doShow = new Action<Form>(instance =>
            {
                if (show) instance?.Show();
                else instance?.Hide();
            });

            if (!InvokeRequired)
            {
                doShow(this);
            }
            else
            {
                Invoke(doShow, this);
            }
        }
    }
}