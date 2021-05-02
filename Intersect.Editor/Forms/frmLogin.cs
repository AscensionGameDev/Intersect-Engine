using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Network;

namespace Intersect.Editor.Forms
{

    public partial class FrmLogin : Form
    {

        //Cross Thread Delegates
        public delegate void BeginEditorLoop();

        public BeginEditorLoop EditorLoopDelegate;

        private bool mOptionsLoaded = false;

        private string mSavedPassword = "";

        public FrmLogin()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            InitializeComponent();

            this.Icon = Properties.Resources.Icon;
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
            Strings.Load();
            GameContentManager.CheckForResources();
            Database.LoadOptions();
            mOptionsLoaded = true;
            EditorLoopDelegate = Main.StartLoop;
            if (Preferences.LoadPreference("username").Trim().Length > 0)
            {
                txtUsername.Text = Preferences.LoadPreference("Username");
                txtPassword.Text = "*****";
                mSavedPassword = Preferences.LoadPreference("Password");
                chkRemember.Checked = true;
            }

            Database.InitMapCache();
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.Login.title;
            lblVersion.Text = Strings.Login.version.ToString(Application.ProductVersion);
            lblGettingStarted.Text = Strings.Login.gettingstarted;
            lblUsername.Text = Strings.Login.username;
            lblPassword.Text = Strings.Login.password;
            chkRemember.Text = Strings.Login.rememberme;
            btnLogin.Text = Strings.Login.login;
            lblStatus.Text = Strings.Login.connecting;
        }

        private void tmrSocket_Tick(object sender, EventArgs e)
        {
            if (!mOptionsLoaded)
            {
                return;
            }

            Networking.Network.Update();
            var statusString = Strings.Login.connecting;
            btnLogin.Enabled = Networking.Network.Connected;
            if (Networking.Network.Connected)
            {
                statusString = Strings.Login.connected;
            }
            else if (Networking.Network.Connecting)
            {
            }
            else if (Networking.Network.ConnectionDenied)
            {
                statusString = Strings.Login.Denied;
            }
            else
            {
                var seconds = (Globals.ReconnectTime - Globals.System.GetTimeMs()) / 1000;
                statusString = Strings.Login.failedtoconnect.ToString(seconds.ToString("0"));
            }

            Process.GetProcesses()
                .ToList()
                .ForEach(
                    process =>
                    {
                        if (!(process?.ProcessName.Contains("raptr") ?? false))
                        {
                            return;
                        }

                        statusString = Strings.Login.raptr;
                        btnLogin.Enabled = false;
                    }
                );

            Globals.LoginForm.lblStatus.Text = statusString;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Networking.Network.Connected || !btnLogin.Enabled)
            {
                return;
            }

            if (txtUsername.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
            {
                using (var sha = new SHA256Managed())
                {
                    if (mSavedPassword != "")
                    {
                        PacketSender.SendLogin(txtUsername.Text.Trim(), mSavedPassword);
                    }
                    else
                    {
                        PacketSender.SendLogin(
                            txtUsername.Text.Trim(),
                            BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text.Trim())))
                                .Replace("-", "")
                        );
                    }
                }
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar != 13)
            {
                return;
            }

            e.Handled = true;
            btnLogin_Click(null, null);
        }

        protected override void OnClosed(EventArgs e)
        {
            Networking.Network.EditorLidgrenNetwork?.Disconnect(NetworkStatus.Quitting.ToString());
            base.OnClosed(e);
            Application.Exit();
        }

        public void TryRemembering()
        {
            using (var sha = new SHA256Managed())
            {
                if (chkRemember.Checked)
                {
                    Preferences.SavePreference("Username", txtUsername.Text);
                    if (mSavedPassword != "")
                    {
                        Preferences.SavePreference("Password", mSavedPassword);
                    }
                    else
                    {
                        Preferences.SavePreference(
                            "Password",
                            BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text.Trim())))
                                .Replace("-", "")
                        );
                    }
                }
                else
                {
                    Preferences.SavePreference("Username", "");
                    Preferences.SavePreference("Password", "");
                }
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                return;
            }

            if (mSavedPassword != "")
            {
                mSavedPassword = "";
                txtPassword.Text = "";
                chkRemember.Checked = false;
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                return;
            }

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

        public void HideSafe()
        {
            ShowSafe(false);
        }

        public void ShowSafe(bool show = true)
        {
            var doShow = new Action<Form>(
                instance =>
                {
                    if (show)
                    {
                        instance?.Show();
                    }
                    else
                    {
                        instance?.Hide();
                    }
                }
            );

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
