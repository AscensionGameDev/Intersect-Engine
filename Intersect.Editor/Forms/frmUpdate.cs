using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Intersect.Configuration;
using Intersect.Editor.Content;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Updater;

namespace Intersect.Editor.Forms
{

    public partial class FrmUpdate : Form
    {

        private Updater.Updater mUpdater;

        public FrmUpdate()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            InitializeComponent();

            this.Icon = Properties.Resources.Icon;
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
            Strings.Load();
            GameContentManager.CheckForResources();
            Database.LoadOptions();
            InitLocalization();
            mUpdater = new Updater.Updater(ClientConfiguration.Instance.UpdateUrl, Path.Combine("version.json"), false, 5);
        }

        private void InitLocalization()
        {
            Text = Strings.Update.Title;
            lblVersion.Text = Strings.Login.version.ToString(Application.ProductVersion);
            lblStatus.Text = Strings.Update.Checking;
        }

        protected override void OnClosed(EventArgs e)
        {
            mUpdater.Stop();
            base.OnClosed(e);
            Application.Exit();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (mUpdater != null)
            {

                progressBar.Style = mUpdater.Status == UpdateStatus.Checking
                    ? ProgressBarStyle.Marquee
                    : ProgressBarStyle.Continuous;

                switch (mUpdater.Status)
                {
                    case UpdateStatus.Checking:
                        lblStatus.Text = Strings.Update.Checking;
                        break;
                    case UpdateStatus.Updating:
                        lblFiles.Show();
                        lblSize.Show();
                        lblFiles.Text = Strings.Update.Files.ToString(mUpdater.FilesRemaining);
                        lblSize.Text = Strings.Update.Size.ToString(mUpdater.GetHumanReadableFileSize(mUpdater.SizeRemaining));
                        lblStatus.Text = Strings.Update.Updating.ToString((int)mUpdater.Progress);
                        progressBar.Value = (int) mUpdater.Progress;
                        break;
                    case UpdateStatus.Restart:
                        lblFiles.Hide();
                        lblSize.Hide();
                        progressBar.Value = 100;
                        lblStatus.Text = Strings.Update.Restart.ToString();
                        tmrUpdate.Enabled = false;
                        Process.Start(
                            Environment.GetCommandLineArgs()[0],
                            Environment.GetCommandLineArgs().Length > 1
                                ? string.Join(" ", Environment.GetCommandLineArgs().Skip(1))
                                : null
                        );

                        this.Close();

                        break;
                    case UpdateStatus.Done:
                        lblFiles.Hide();
                        lblSize.Hide();
                        progressBar.Value = 100;
                        lblStatus.Text = Strings.Update.Done;
                        tmrUpdate.Enabled = false;
                        Hide();
                        Globals.LoginForm.Show();
                        break;
                    case UpdateStatus.Error:
                        lblFiles.Hide();
                        lblSize.Hide();
                        progressBar.Value = 100;
                        lblStatus.Text = Strings.Update.Error.ToString(mUpdater.Exception?.Message ?? "");
                        break;
                    case UpdateStatus.None:
                        lblFiles.Hide();
                        lblSize.Hide();
                        tmrUpdate.Enabled = false;
                        Hide();
                        Globals.LoginForm.Show();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

}
