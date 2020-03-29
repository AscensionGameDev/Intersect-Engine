using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    public class VerticalSplitter : Base
    {

        private readonly SplitterBar mHSplitter;

        private readonly Base[] mSections;

        private int mBarSize; // pixels

        private float mHVal; // 0-1

        private int mZoomedSection; // 0-3

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossSplitter" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public VerticalSplitter(Base parent) : base(parent)
        {
            mSections = new Base[2];

            mHSplitter = new SplitterBar(this);
            mHSplitter.SetPosition(128, 0);
            mHSplitter.Dragged += OnHorizontalMoved;
            mHSplitter.Cursor = Cursors.SizeWe;

            mHVal = 0.5f;

            SetPanel(0, null);
            SetPanel(1, null);

            SplitterSize = 5;
            SplittersVisible = false;

            mZoomedSection = -1;
        }

        /// <summary>
        ///     Indicates whether any of the panels is zoomed.
        /// </summary>
        public bool IsZoomed => mZoomedSection != -1;

        /// <summary>
        ///     Gets or sets a value indicating whether splitters should be visible.
        /// </summary>
        public bool SplittersVisible
        {
            get => mHSplitter.ShouldDrawBackground;
            set => mHSplitter.ShouldDrawBackground = value;
        }

        /// <summary>
        ///     Gets or sets the size of the splitter.
        /// </summary>
        public int SplitterSize
        {
            get => mBarSize;
            set => mBarSize = value;
        }

        /// <summary>
        ///     Invoked when one of the panels has been zoomed (maximized).
        /// </summary>
        public event GwenEventHandler<EventArgs> PanelZoomed;

        /// <summary>
        ///     Invoked when one of the panels has been unzoomed (restored).
        /// </summary>
        public event GwenEventHandler<EventArgs> PanelUnZoomed;

        /// <summary>
        ///     Invoked when the zoomed panel has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ZoomChanged;

        /// <summary>
        ///     Centers the panels so that they take even amount of space.
        /// </summary>
        public void CenterPanels()
        {
            mHVal = 0.5f;
            Invalidate();
        }

        public void SetHValue(float value)
        {
            if (value <= 1f || value >= 0)
            {
                mHVal = value;
            }
        }

        private void UpdateHSplitter()
        {
            mHSplitter.MoveTo((Width - mHSplitter.Width) * mHVal, mHSplitter.Y);
        }

        protected void OnHorizontalMoved(Base control, EventArgs args)
        {
            mHVal = CalculateValueHorizontal();
            Invalidate();
        }

        private float CalculateValueHorizontal()
        {
            return mHSplitter.X / (float) (Width - mHSplitter.Width);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            mHSplitter.SetSize(mBarSize, Height);

            UpdateHSplitter();

            if (mZoomedSection == -1)
            {
                if (mSections[0] != null)
                {
                    mSections[0].SetBounds(0, 0, mHSplitter.X, Height);
                }

                if (mSections[1] != null)
                {
                    mSections[1].SetBounds(mHSplitter.X + mBarSize, 0, Width - (mHSplitter.X + mBarSize), Height);
                }
            }
            else
            {
                //This should probably use Fill docking instead
                mSections[mZoomedSection].SetBounds(0, 0, Width, Height);
            }
        }

        /// <summary>
        ///     Assigns a control to the specific inner section.
        /// </summary>
        /// <param name="index">Section index (0-3).</param>
        /// <param name="panel">Control to assign.</param>
        public void SetPanel(int index, Base panel)
        {
            mSections[index] = panel;

            if (panel != null)
            {
                panel.Dock = Pos.None;
                panel.Parent = this;
            }

            Invalidate();
        }

        /// <summary>
        ///     Gets the specific inner section.
        /// </summary>
        /// <param name="index">Section index (0-3).</param>
        /// <returns>Specified section.</returns>
        public Base GetPanel(int index)
        {
            return mSections[index];
        }

        /// <summary>
        ///     Internal handler for the zoom changed event.
        /// </summary>
        protected void OnZoomChanged()
        {
            if (ZoomChanged != null)
            {
                ZoomChanged.Invoke(this, EventArgs.Empty);
            }

            if (mZoomedSection == -1)
            {
                if (PanelUnZoomed != null)
                {
                    PanelUnZoomed.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (PanelZoomed != null)
                {
                    PanelZoomed.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Maximizes the specified panel so it fills the entire control.
        /// </summary>
        /// <param name="section">Panel index (0-3).</param>
        public void Zoom(int section)
        {
            UnZoom();

            if (mSections[section] != null)
            {
                for (var i = 0; i < 2; i++)
                {
                    if (i != section && mSections[i] != null)
                    {
                        mSections[i].IsHidden = true;
                    }
                }

                mZoomedSection = section;

                Invalidate();
            }

            OnZoomChanged();
        }

        /// <summary>
        ///     Restores the control so all panels are visible.
        /// </summary>
        public void UnZoom()
        {
            mZoomedSection = -1;

            for (var i = 0; i < 2; i++)
            {
                if (mSections[i] != null)
                {
                    mSections[i].IsHidden = false;
                }
            }

            Invalidate();
            OnZoomChanged();
        }

    }

}
