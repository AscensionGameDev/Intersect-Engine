using System;
using System.Windows.Forms;
using Intersect.Localization;

namespace Intersect.Editor.Forms
{
    public partial class frmProgress : Form
    {
        private int progressVal;
        private bool shouldClose;
        private bool showCancelBtn;
        private string statusText;

        public frmProgress()
        {
            InitializeComponent();
            InitLocalization();
        }

        private void InitLocalization()
        {
            btnCancel.Text = Strings.Get("progressform", "cancel");
        }

        public void SetTitle(string title)
        {
            Text = title;
        }

        public void SetProgress(string label, int progress, bool showCancel)
        {
            statusText = label;
            if (progress < 0)
            {
                progressVal = 0;
                progressBar.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                progressVal = progress;
                progressBar.Style = ProgressBarStyle.Blocks;
            }

            showCancelBtn = showCancel;
            tmrUpdater_Tick(null, null);
            Application.DoEvents();
        }

        public void NotifyClose()
        {
            shouldClose = true;
        }

        private void tmrUpdater_Tick(object sender, EventArgs e)
        {
            if (!InvokeRequired)
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
}