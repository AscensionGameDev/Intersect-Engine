using System;

namespace Intersect.Client.Framework.Gwen.Control.Layout
{

    /// <summary>
    ///     Base splitter class.
    /// </summary>
    public class Splitter : Base
    {

        private readonly Base[] mPanel;

        private readonly bool[] mScale;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Splitter" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Splitter(Base parent) : base(parent)
        {
            mPanel = new Base[2];
            mScale = new bool[2];
            mScale[0] = true;
            mScale[1] = true;
        }

        /// <summary>
        ///     Sets the contents of a splitter panel.
        /// </summary>
        /// <param name="panelIndex">Panel index (0-1).</param>
        /// <param name="panel">Panel contents.</param>
        /// <param name="noScale">Determines whether the content is to be scaled.</param>
        public void SetPanel(int panelIndex, Base panel, bool noScale = false)
        {
            if (panelIndex < 0 || panelIndex > 1)
            {
                throw new ArgumentException("Invalid panel index", "panelIndex");
            }

            mPanel[panelIndex] = panel;
            mScale[panelIndex] = !noScale;

            if (null != mPanel[panelIndex])
            {
                mPanel[panelIndex].Parent = this;
            }
        }

        /// <summary>
        ///     Gets the contents of a secific panel.
        /// </summary>
        /// <param name="panelIndex">Panel index (0-1).</param>
        /// <returns></returns>
        Base GetPanel(int panelIndex)
        {
            if (panelIndex < 0 || panelIndex > 1)
            {
                throw new ArgumentException("Invalid panel index", "panelIndex");
            }

            return mPanel[panelIndex];
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            LayoutVertical(skin);
        }

        protected virtual void LayoutVertical(Skin.Base skin)
        {
            var w = Width;
            var h = Height;

            if (mPanel[0] != null)
            {
                var m = mPanel[0].Margin;
                if (mScale[0])
                {
                    mPanel[0].SetBounds(m.Left, m.Top, w - m.Left - m.Right, h * 0.5f - m.Top - m.Bottom);
                }
                else
                {
                    mPanel[0].Position(Pos.Center, 0, (int) (h * -0.25f));
                }
            }

            if (mPanel[1] != null)
            {
                var m = mPanel[1].Margin;
                if (mScale[1])
                {
                    mPanel[1].SetBounds(m.Left, m.Top + h * 0.5f, w - m.Left - m.Right, h * 0.5f - m.Top - m.Bottom);
                }
                else
                {
                    mPanel[1].Position(Pos.Center, 0, (int) (h * 0.25f));
                }
            }
        }

        protected virtual void LayoutHorizontal(Skin.Base skin)
        {
            throw new NotImplementedException();
        }

    }

}
