using System.Globalization;
using Intersect.Configuration;
using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Framework;
using Intersect.Framework.Core.AssetManagement;
using Intersect.Framework.Utilities;
using Intersect.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ApplicationContext = Intersect.Core.ApplicationContext;

namespace Intersect.Editor.Forms;


public partial class FrmUpdate : Form
{

    private readonly object _manifestTaskLock = new();

    private long _nextUpdateAttempt;
    private Task? _pendingManifestTask;
    private TokenResponse? _tokenResponse;
    private Updater? _updater;
    private UpdaterStatus? _updaterStatus;

    public FrmUpdate()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        InitializeComponent();
        Icon = Program.Icon;
    }

    private void frmUpdate_Load(object sender, EventArgs e)
    {
        try
        {
            Strings.Load();
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Error loading strings");
            throw;
        }

        GameContentManager.CheckForResources();
        Database.LoadOptions();
        InitLocalization();


        if (ClientConfiguration.Instance.UpdateUrl is not { } updateUrl || string.IsNullOrWhiteSpace(updateUrl))
        {
            return;
        }

        _updater = new Updater(
            updateUrl,
            "editor/update.json",
            "version.editor.json",
            7
        );

        var rawTokenResponse = Preferences.LoadPreference(nameof(TokenResponse));
        if (string.IsNullOrWhiteSpace(rawTokenResponse))
        {
            return;
        }

        try
        {
            _tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(rawTokenResponse);
            _updater.SetAuthorizationData(_tokenResponse);
        }
        catch (Exception exception)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                exception,
                "Failed to deserialize token on disk, re-authentication will be necessary"
            );
        }
    }

    private void InitLocalization()
    {
        Text = Strings.Update.Title;
        lblVersion.Text = Strings.Login.version.ToString(Application.ProductVersion);
        lblVersion.Location = new System.Drawing.Point(
            (lblVersion.Parent?.ClientRectangle.Right - (lblVersion.Parent?.Padding.Right + lblVersion.Width + 4)) ?? 0,
            (lblVersion.Parent?.ClientRectangle.Bottom - (lblVersion.Parent?.Padding.Bottom + lblVersion.Height + 4)) ??
            0
        );
        lblStatus.Text = Strings.Update.Checking;
    }

    protected override void OnClosed(EventArgs e)
    {
        _updater?.Stop();
        base.OnClosed(e);
        Application.Exit();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_updater is null)
        {
            SwitchToLogin(requiresAuthentication: false, deferHide: true);
        }
        else
        {
            tmrUpdate.Enabled = true;
        }
    }

    private void SwitchToLogin(bool requiresAuthentication, bool deferHide = false)
    {
        lblFiles.Hide();
        lblSize.Hide();
        tmrUpdate.Enabled = false;

        var loginForm = Globals.LoginForm ??= new FrmLogin(requiresAuthentication);

        _pendingManifestTask = null;

        try
        {
            loginForm.Show();

            Hide();
        }
        catch
        {
            throw;
        }
        finally
        {
        }
    }

    private void CheckForUpdate()
    {
        lock (_manifestTaskLock)
        {
            if (_pendingManifestTask != null)
            {
                return;
            }

            _pendingManifestTask = Task.Run(
                () =>
                {
                    _updaterStatus = _updater?.TryGetManifest(out _, force: _tokenResponse != null);
                    if (_updaterStatus == UpdaterStatus.Offline)
                    {
                        _nextUpdateAttempt = Environment.TickCount64 + 10_000;
                    }

                    _pendingManifestTask = null;
                }
            );
        }
    }

    internal void ShowWithToken(TokenResponse tokenResponse)
    {
        _tokenResponse = tokenResponse ?? throw new ArgumentNullException(nameof(tokenResponse));

        Preferences.SavePreference(nameof(TokenResponse), JsonConvert.SerializeObject(_tokenResponse));
        _updater?.SetAuthorizationData(_tokenResponse);

        _updaterStatus = null;

        lblFiles.Show();
        lblSize.Show();
        tmrUpdate.Enabled = true;

        Show();

        Globals.LoginForm?.Close();
        Globals.LoginForm = null;
    }

    private void tmrUpdate_Tick(object sender, EventArgs e)
    {
        if (_updater == null)
        {
            return;
        }

        switch (_updaterStatus)
        {
            case UpdaterStatus.NoUpdateNeeded:
                SwitchToLogin(false);
                return;
            case UpdaterStatus.NeedsAuthentication:
                SwitchToLogin(true);
                return;
            case UpdaterStatus.Ready:
                _nextUpdateAttempt = long.MinValue;
                _updaterStatus = null;
                _updater.Start();
                break;
            case UpdaterStatus.Offline:
                break;
            default:
                throw Exceptions.UnreachableInvalidEnum(_updaterStatus ?? default);
        }

        if (_nextUpdateAttempt != long.MinValue)
        {
            var now = Environment.TickCount64;
            if (now < _nextUpdateAttempt)
            {
                return;
            }

            _nextUpdateAttempt = now + 10_000;
            CheckForUpdate();
            return;
        }

        progressBar.Style = _updater.Status == UpdateStatus.DownloadingManifest
            ? ProgressBarStyle.Marquee
            : ProgressBarStyle.Continuous;

        switch (_updater.Status)
        {
            case UpdateStatus.DownloadingManifest:
                lblStatus.Text = Strings.Update.Checking;
                break;
            case UpdateStatus.UpdateInProgress:
                lblFiles.Show();
                lblSize.Show();
                lblFiles.Text = Strings.Update.Files.ToString(_updater.FilesRemaining);
                lblSize.Text = Strings.Update.Size.ToString(Updater.GetHumanReadableFileSize(_updater.SizeRemaining));
                lblStatus.Text = Strings.Update.Updating.ToString((int)_updater.Progress);
                progressBar.Value = Math.Min(100, (int)_updater.Progress);
                break;
            case UpdateStatus.Restart:
                lblFiles.Hide();
                lblSize.Hide();
                progressBar.Value = 100;
                lblStatus.Text = Strings.Update.Restart.ToString();
                tmrUpdate.Enabled = false;

                if (!ProcessHelper.TryRelaunch())
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning("Failed to restart automatically");
                }

                this.Close();

                break;
            case UpdateStatus.UpdateCompleted:
                progressBar.Value = 100;
                lblStatus.Text = Strings.Update.Done;
                SwitchToLogin(false);
                break;
            case UpdateStatus.Error:
                lblFiles.Hide();
                lblSize.Hide();
                progressBar.Value = 100;
                lblStatus.Text = Strings.Update.Error.ToString(_updater.Exception?.Message ?? "");
                break;
            case UpdateStatus.None:
                SwitchToLogin(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
