using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Utilities;

namespace Intersect.Editor.Forms;


public partial class FrmLogin : Form
{

    //Cross Thread Delegates
    public delegate void BeginEditorLoop();

    public BeginEditorLoop EditorLoopDelegate;

    private bool mOptionsLoaded = false;

    private string mSavedPassword = string.Empty;

    public FrmLogin()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        InitializeComponent();
        Icon = Program.Icon;
    }

    private void frmLogin_Load(object sender, EventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
        try
        {
            Strings.Load();
        }
        catch (Exception exception)
        {
            Log.Error(exception);
            throw;
        }
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

    public long LastNetworkStatusChangeTime { get; private set; }

    private NetworkStatus _networkStatus;

    public void SetNetworkStatus(NetworkStatus networkStatus)
    {
        _networkStatus = networkStatus;
        LastNetworkStatusChangeTime = Timing.Global.MillisecondsUtc;
    }

    private void tmrSocket_Tick(object sender, EventArgs e)
    {
        if (!mOptionsLoaded)
        {
            return;
        }

        Networking.Network.Update();
        btnLogin.Enabled = _networkStatus == NetworkStatus.Online || Networking.Network.Connected;

        string statusString = _networkStatus switch
        {
            NetworkStatus.Unknown => Strings.Login.Denied,
            NetworkStatus.Connecting => Strings.Login.connecting,
            NetworkStatus.Online => Strings.Login.connected,
            NetworkStatus.Offline => Strings.Login.failedtoconnect.ToString(((Globals.NextServerStatusPing - Timing.Global.MillisecondsUtc) / 1000).ToString("0")),
            NetworkStatus.Failed => Strings.Login.Denied,
            NetworkStatus.VersionMismatch => Strings.Login.Denied,
            NetworkStatus.ServerFull => Strings.Login.Denied,
            NetworkStatus.HandshakeFailure => Strings.Login.Denied,
            NetworkStatus.Quitting => Strings.Login.Denied,
            _ => throw new UnreachableException(),
        };

        var hasRaptr = Process.GetProcesses()
            .ToArray()
            .Any(process => process.ProcessName.Contains("raptr"));
        if (hasRaptr)
        {
            statusString = Strings.Login.raptr;
            btnLogin.Enabled = false;
        }

        Globals.LoginForm.lblStatus.Text = statusString;

        if (_loginPending && Networking.Network.Connected)
        {
            _loginPending = false;
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
    }

    private bool _loginPending = false;

    private void btnLogin_Click(object sender, EventArgs e)
    {
        if (!Networking.Network.Connected)
        {
            Networking.Network.Connect();
        }

        _loginPending = true;
        btnLogin.Enabled = false;
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
            mSavedPassword = string.Empty;
            txtPassword.Text = string.Empty;
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
            mSavedPassword = string.Empty;
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
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
