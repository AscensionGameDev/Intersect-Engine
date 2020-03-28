using System;
using System.Windows.Forms;

namespace Intersect.Editor.Forms.Controls
{

    public partial class AutoDragPanel : Panel
    {

        private Timer mDragTimer;

        private int mMaxDragChange = 2;

        public AutoDragPanel()
        {
            InitializeComponent();
            mDragTimer = new Timer()
            {
                Interval = 1
            };

            mDragTimer.Tick += DragTimer_Tick;
            MouseDown += AutoDragPanel_MouseDown;
            MouseUp += AutoDragPanel_MouseUp;
        }

        public void AutoDragPanel_MouseUp(object sender, MouseEventArgs e)
        {
            mDragTimer.Enabled = false;
        }

        public void AutoDragPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mDragTimer.Enabled = true;
            }
        }

        private void DragTimer_Tick(object sender, EventArgs e)
        {
            var pos = PointToClient(MousePosition);

            var right = ClientRectangle.Right;
            var bottom = ClientRectangle.Bottom;

            if (VerticalScroll.Visible)
            {
                right = Width - SystemInformation.VerticalScrollBarWidth;
            }

            if (HorizontalScroll.Visible)
            {
                bottom = Height - SystemInformation.HorizontalScrollBarHeight;
            }

            if (VerticalScroll.Visible)
            {
                // Scroll up
                if (pos.Y < ClientRectangle.Top)
                {
                    var difference = (pos.Y - ClientRectangle.Top) * -1;

                    if (mMaxDragChange > 0 && difference > mMaxDragChange)
                    {
                        difference = mMaxDragChange;
                    }

                    if (VerticalScroll.Value < difference)
                    {
                        VerticalScroll.Value = 0;
                    }
                    else
                    {
                        VerticalScroll.Value = VerticalScroll.Value - difference;
                    }
                }

                // Scroll down
                if (pos.Y > bottom)
                {
                    var difference = pos.Y - bottom;

                    if (mMaxDragChange > 0 && difference > mMaxDragChange)
                    {
                        difference = mMaxDragChange;
                    }

                    VerticalScroll.Value = VerticalScroll.Value + difference;
                }
            }

            if (HorizontalScroll.Visible)
            {
                // Scroll left
                if (pos.X < ClientRectangle.Left)
                {
                    var difference = (pos.X - ClientRectangle.Left) * -1;

                    if (mMaxDragChange > 0 && difference > mMaxDragChange)
                    {
                        difference = mMaxDragChange;
                    }

                    if (HorizontalScroll.Value < difference)
                    {
                        HorizontalScroll.Value = 0;
                    }
                    else
                    {
                        HorizontalScroll.Value = HorizontalScroll.Value - difference;
                    }
                }

                // Scroll right
                if (pos.X > right)
                {
                    var difference = pos.X - right;

                    if (mMaxDragChange > 0 && difference > mMaxDragChange)
                    {
                        difference = mMaxDragChange;
                    }

                    HorizontalScroll.Value = HorizontalScroll.Value + difference;
                }
            }
        }

    }

}
