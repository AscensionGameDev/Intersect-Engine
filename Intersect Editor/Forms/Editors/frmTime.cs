using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library.GameObjects;
using Microsoft.Xna.Framework;

namespace Intersect_Editor.Forms.Editors
{
    public partial class frmTime : Form
    {
        private Bitmap tileBackbuffer;
        private TimeBase backupTime;
        private TimeBase myTime;
        public frmTime()
        {
            InitializeComponent();
        }

        public void InitEditor(TimeBase time)
        {
            //Create a backup in case we want to revert
            myTime = time;
            backupTime = new TimeBase();
            backupTime.LoadTimeBase(time.SaveTimeBase());

            tileBackbuffer = new Bitmap(pnlColor.Width,pnlColor.Height);
            UpdateList(TimeBase.GetTimeInterval(cmbIntervals.SelectedIndex));
            typeof(Panel).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, pnlColor, new object[] { true });

            chkSync.Checked = myTime.SyncTime;
            txtTimeRate.Text = myTime.Rate.ToString();
            cmbIntervals.SelectedIndex = TimeBase.GetIntervalIndex(myTime.RangeInterval);
            UpdateList(myTime.RangeInterval);
            txtTimeRate.Enabled = !myTime.SyncTime;
        }

        private void cmbIntervals_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myTime.RangeInterval != TimeBase.GetTimeInterval(cmbIntervals.SelectedIndex))
            {
                myTime.RangeInterval = TimeBase.GetTimeInterval(cmbIntervals.SelectedIndex);
                UpdateList(myTime.RangeInterval);
                myTime.ResetColors();
                grpRangeOptions.Hide();
            }
        }

        private void UpdateList(int duration)
        {
            lstTimes.Items.Clear();
            var time = new DateTime(2000, 1, 1, 0,0,0);
            for (int i = 0; i < 1440; i += duration)
            {
                var addRange = time.ToString("h:mm:ss tt") + " to ";
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
                myTime.RangeColors[lstTimes.SelectedIndex].R = pnlColor.BackColor.R;
                myTime.RangeColors[lstTimes.SelectedIndex].G = pnlColor.BackColor.G;
                myTime.RangeColors[lstTimes.SelectedIndex].B = pnlColor.BackColor.B;
            }
        }

        private void pnlColor_Paint(object sender, PaintEventArgs e)
        {
            var g = Graphics.FromImage(tileBackbuffer);
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(pnlColor.BackgroundImage, new System.Drawing.Point(0, 0));
            Brush brush =
                new SolidBrush(System.Drawing.Color.FromArgb((int)((scrlAlpha.Value) / 100f * 255f), pnlColor.BackColor.R, pnlColor.BackColor.G,
                    pnlColor.BackColor.B));
            g.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, pnlColor.Width, pnlColor.Height));
            e.Graphics.DrawImage(tileBackbuffer, new System.Drawing.Point(0, 0));
        }

        private void scrlAlpha_Scroll(object sender, ScrollEventArgs e)
        {
            lblBrightness.Text = "Brightness: " + (100- scrlAlpha.Value).ToString();
            myTime.RangeColors[lstTimes.SelectedIndex].A = (byte)(scrlAlpha.Value/100f*255f);
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
            pnlColor.BackColor = System.Drawing.Color.FromArgb(255, myTime.RangeColors[lstTimes.SelectedIndex].R, myTime.RangeColors[lstTimes.SelectedIndex].G,
                myTime.RangeColors[lstTimes.SelectedIndex].B);
            scrlAlpha.Value = (byte) (((myTime.RangeColors[lstTimes.SelectedIndex].A)/255f)*100f);
            lblBrightness.Text = "Brightness: " + (100 - scrlAlpha.Value).ToString();
            pnlColor.Refresh();
            EditorGraphics.LightColor = myTime.RangeColors[lstTimes.SelectedIndex];
        }

        private void chkSync_CheckedChanged(object sender, EventArgs e)
        {
            myTime.SyncTime = chkSync.Checked;
            txtTimeRate.Enabled = !myTime.SyncTime;
        }

        private void txtTimeRate_TextChanged(object sender, EventArgs e)
        {
            var val = 0f;
            if (float.TryParse(txtTimeRate.Text, out val))
            {
                myTime.Rate = val;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            PacketSender.SendSaveTime(myTime.SaveTimeBase());
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            myTime.LoadTimeBase(backupTime.SaveTimeBase());
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }
    }
}
