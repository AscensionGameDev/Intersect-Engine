using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using DarkUI.Controls;

using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmTime : Form
    {

        private TimeBase mBackupTime;

        private Bitmap mTileBackbuffer;

        private TimeBase mYTime;

        public FrmTime()
        {
            InitializeComponent();
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.TimeEditor.title;
            lblTimes.Text = Strings.TimeEditor.times;
            grpSettings.Text = Strings.TimeEditor.settings;
            lblIntervals.Text = Strings.TimeEditor.interval;
            cmbIntervals.Items.Clear();
            for (var i = 0; i < Strings.TimeEditor.intervals.Count; i++)
            {
                cmbIntervals.Items.Add(Strings.TimeEditor.intervals[i]);
            }

            chkSync.Text = Strings.TimeEditor.sync;
            lblRate.Text = Strings.TimeEditor.rate;
            lblRateSuffix.Text = Strings.TimeEditor.ratesuffix;
            lblRateDesc.Text = Strings.TimeEditor.ratedesc;
            grpRangeOptions.Text = Strings.TimeEditor.overlay;
            lblColorDesc.Text = Strings.TimeEditor.colorpaneldesc;
            btnSave.Text = Strings.TimeEditor.save;
            btnCancel.Text = Strings.TimeEditor.cancel;
        }

        public void InitEditor(TimeBase time)
        {
            //Create a backup in case we want to revert
            mYTime = time;
            mBackupTime = new TimeBase();
            mBackupTime.LoadFromJson(time.GetInstanceJson());

            mTileBackbuffer = new Bitmap(pnlColor.Width, pnlColor.Height);
            UpdateList(TimeBase.GetTimeInterval(cmbIntervals.SelectedIndex));
            typeof(Panel).InvokeMember(
                "DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null,
                pnlColor, new object[] {true}
            );

            chkSync.Checked = mYTime.SyncTime;
            txtTimeRate.Text = mYTime.Rate.ToString();
            cmbIntervals.SelectedIndex = TimeBase.GetIntervalIndex(mYTime.RangeInterval);
            UpdateList(mYTime.RangeInterval);
            txtTimeRate.Enabled = !mYTime.SyncTime;
        }

        private void cmbIntervals_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mYTime.RangeInterval != TimeBase.GetTimeInterval(cmbIntervals.SelectedIndex))
            {
                mYTime.RangeInterval = TimeBase.GetTimeInterval(cmbIntervals.SelectedIndex);
                UpdateList(mYTime.RangeInterval);
                mYTime.ResetColors();
                grpRangeOptions.Hide();
            }
        }

        private void UpdateList(int duration)
        {
            lstTimes.Items.Clear();
            var time = new DateTime(2000, 1, 1, 0, 0, 0);
            for (var i = 0; i < 1440; i += duration)
            {
                var addRange = time.ToString("h:mm:ss tt") + " " + Strings.TimeEditor.to + " ";
                time = time.AddMinutes(duration);
                addRange += time.ToString("h:mm:ss tt");
                lstTimes.Items.Add(addRange);
            }
        }

        private void pnlColor_DoubleClick(object sender, EventArgs e)
        {
            clrSelector.Color = pnlColor.BackColor;
            if (clrSelector.ShowDialog() == DialogResult.OK)
            {
                pnlColor.BackColor = clrSelector.Color;
                mYTime.DaylightHues[lstTimes.SelectedIndex].R = pnlColor.BackColor.R;
                mYTime.DaylightHues[lstTimes.SelectedIndex].G = pnlColor.BackColor.G;
                mYTime.DaylightHues[lstTimes.SelectedIndex].B = pnlColor.BackColor.B;
            }
        }

        private void pnlColor_Paint(object sender, PaintEventArgs e)
        {
            var g = System.Drawing.Graphics.FromImage(mTileBackbuffer);
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(pnlColor.BackgroundImage, new System.Drawing.Point(0, 0));
            Brush brush = new SolidBrush(
                System.Drawing.Color.FromArgb(
                    scrlAlpha.Value, pnlColor.BackColor.R, pnlColor.BackColor.G, pnlColor.BackColor.B
                )
            );

            g.FillRectangle(brush, new Rectangle(0, 0, pnlColor.Width, pnlColor.Height));
            e.Graphics.DrawImage(mTileBackbuffer, new System.Drawing.Point(0, 0));
        }

        private void scrlAlpha_Scroll(object sender, ScrollValueEventArgs e)
        {
            var brightness = (int) ((255 - scrlAlpha.Value) / 255f * 100);
            lblBrightness.Text = Strings.TimeEditor.brightness.ToString(brightness.ToString());
            mYTime.DaylightHues[lstTimes.SelectedIndex].A = (byte) scrlAlpha.Value;
            pnlColor.Refresh();
        }

        private void lstTimes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTimes.SelectedIndex == -1)
            {
                grpRangeOptions.Hide();

                return;
            }

            grpRangeOptions.Show();
            pnlColor.BackColor = System.Drawing.Color.FromArgb(
                255, mYTime.DaylightHues[lstTimes.SelectedIndex].R, mYTime.DaylightHues[lstTimes.SelectedIndex].G,
                mYTime.DaylightHues[lstTimes.SelectedIndex].B
            );

            scrlAlpha.Value = mYTime.DaylightHues[lstTimes.SelectedIndex].A;
            var brightness = (int) ((255 - scrlAlpha.Value) / 255f * 100);
            lblBrightness.Text = Strings.TimeEditor.brightness.ToString(brightness);
            pnlColor.Refresh();
            Core.Graphics.LightColor = mYTime.DaylightHues[lstTimes.SelectedIndex];
        }

        private void chkSync_CheckedChanged(object sender, EventArgs e)
        {
            mYTime.SyncTime = chkSync.Checked;
            txtTimeRate.Enabled = !mYTime.SyncTime;
        }

        private void txtTimeRate_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(txtTimeRate.Text, out var val))
            {
                mYTime.Rate = val;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            PacketSender.SendSaveTime(mYTime.GetInstanceJson());
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mYTime.LoadFromJson(mBackupTime.GetInstanceJson());
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

    }

}
