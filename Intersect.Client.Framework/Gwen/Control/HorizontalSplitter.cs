using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    public class HorizontalSplitter : Base
    {

        private readonly Base[] mSections;

        private readonly SplitterBar mVSplitter;

        private int mBarSize; // pixels

        private float mVVal; // 0-1

        private int mZoomedSection; // 0-1

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossSplitter" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HorizontalSplitter(Base parent) : base(parent)
        {
            mSections = new Base[2];

            mVSplitter = new SplitterBar(this);
            mVSplitter.SetPosition(0, 128);
            mVSplitter.Dragged += OnVerticalMoved;
            mVSplitter.Cursor = Cursors.SizeNs;

            mVVal = 0.5f;

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
            get => mVSplitter.ShouldDrawBackground;
            set => mVSplitter.ShouldDrawBackground = value;
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
            mVVal = 0.5f;
            Invalidate();
        }

        private void UpdateVSplitter()
        {
            mVSplitter.MoveTo(mVSplitter.X, (Height - mVSplitter.Height) * mVVal);
        }

        protected void OnVerticalMoved(Base control, EventArgs args)
        {
            mVVal = CalculateValueVertical();
            Invalidate();
        }

        private float CalculateValueVertical()
        {
            return mVSplitter.Y / (float) (Height - mVSplitter.Height);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            mVSplitter.SetSize(Width, mBarSize);

            UpdateVSplitter();

            if (mZoomedSection == -1)
            {
                if (mSections[0] != null)
                {
                    mSections[0].SetBounds(0, 0, Width, mVSplitter.Y);
                }

                if (mSections[1] != null)
                {
                    mSections[1].SetBounds(0, mVSplitter.Y + mBarSize, Width, Height - (mVSplitter.Y + mBarSize));
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
