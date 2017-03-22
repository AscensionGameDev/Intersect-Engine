using System;
using System.Windows.Forms;

namespace Intersect_Editor.Forms.Controls
{
    public partial class AutoDragPanel : Panel
    {
        private Timer _dragTimer;
        private int MaxDragChange = 2;
        public AutoDragPanel()
        {
            InitializeComponent();
            _dragTimer = new Timer()
            {
                Interval = 1
            };
            _dragTimer.Tick += DragTimer_Tick;
            MouseDown += AutoDragPanel_MouseDown;
            MouseUp += AutoDragPanel_MouseUp;
        }

        public void AutoDragPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _dragTimer.Enabled = false;
        }

        public void AutoDragPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragTimer.Enabled = true;
            }
        }

        private void DragTimer_Tick(object sender, EventArgs e)
        {
            var pos = PointToClient(MousePosition);

            var right = ClientRectangle.Right;
            var bottom = ClientRectangle.Bottom;

            if (VerticalScroll.Visible)
                right = Width - SystemInformation.VerticalScrollBarWidth;

            if (HorizontalScroll.Visible)
                bottom = Height - SystemInformation.HorizontalScrollBarHeight;

            if (VerticalScroll.Visible)
            {
                // Scroll up
                if (pos.Y < ClientRectangle.Top)
                {
                    var difference = (pos.Y - ClientRectangle.Top) * -1;

                    if (MaxDragChange > 0 && difference > MaxDragChange)
                        difference = MaxDragChange;
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

                    if (MaxDragChange > 0 && difference > MaxDragChange)
                        difference = MaxDragChange;

                    VerticalScroll.Value = VerticalScroll.Value + difference;
                }
            }

            if (HorizontalScroll.Visible)
            {
                // Scroll left
                if (pos.X < ClientRectangle.Left)
                {
                    var difference = (pos.X - ClientRectangle.Left) * -1;

                    if (MaxDragChange > 0 && difference > MaxDragChange)
                        difference = MaxDragChange;
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

                    if (MaxDragChange > 0 && difference > MaxDragChange)
                        difference = MaxDragChange;

                    HorizontalScroll.Value = HorizontalScroll.Value + difference;
                }
            }
        }
    }
}
