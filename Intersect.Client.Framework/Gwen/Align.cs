using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen
{

    public enum Alignments
    {

        Top,

        Bottom,

        Left,

        Right,

        Center,

        CenterH,

        CenterV

    }

    /// <summary>
    ///     Utility class for manipulating control's position according to its parent. Rarely needed, use control.Dock.
    /// </summary>
    public static class Align
    {

        /// <summary>
        ///     Centers the control inside its parent.
        /// </summary>
        /// <param name="control">Control to center.</param>
        public static void Center(Base control)
        {
            var parent = control.Parent;
            if (parent == null)
            {
                return;
            }

            control.SetPosition(
                parent.Padding.Left + (parent.Width - parent.Padding.Left - parent.Padding.Right - control.Width) / 2,
                (parent.Height - control.Height) / 2
            );
        }

        /// <summary>
        ///     Moves the control to the left of its parent.
        /// </summary>
        /// <param name="control"></param>
        public static void AlignLeft(Base control)
        {
            var parent = control.Parent;
            if (null == parent)
            {
                return;
            }

            control.SetPosition(parent.Padding.Left + control.Margin.Left, control.Y);
        }

        /// <summary>
        ///     Centers the control horizontally inside its parent.
        /// </summary>
        /// <param name="control"></param>
        public static void CenterHorizontally(Base control)
        {
            var parent = control.Parent;
            if (null == parent)
            {
                return;
            }

            control.SetPosition(
                parent.Padding.Left + (parent.Width - parent.Padding.Left - parent.Padding.Right - control.Width) / 2,
                control.Y
            );
        }

        /// <summary>
        ///     Moves the control to the right of its parent.
        /// </summary>
        /// <param name="control"></param>
        public static void AlignRight(Base control)
        {
            var parent = control.Parent;
            if (null == parent)
            {
                return;
            }

            control.SetPosition(parent.Width - control.Width - parent.Padding.Right - control.Margin.Right, control.Y);
        }

        /// <summary>
        ///     Moves the control to the top of its parent.
        /// </summary>
        /// <param name="control"></param>
        public static void AlignTop(Base control)
        {
            control.SetPosition(control.X, control.Margin.Top);
        }

        /// <summary>
        ///     Centers the control vertically inside its parent.
        /// </summary>
        /// <param name="control"></param>
        public static void CenterVertically(Base control)
        {
            var parent = control.Parent;
            if (null == parent)
            {
                return;
            }

            control.SetPosition(control.X, (parent.Height - control.Height) / 2);
        }

        /// <summary>
        ///     Moves the control to the bottom of its parent.
        /// </summary>
        /// <param name="control"></param>
        public static void AlignBottom(Base control)
        {
            var parent = control.Parent;
            if (null == parent)
            {
                return;
            }

            control.SetPosition(control.X, parent.Height - control.Height - control.Margin.Bottom);
        }

        /// <summary>
        ///     Places the control below other control (left aligned), taking margins into account.
        /// </summary>
        /// <param name="control">Control to place.</param>
        /// <param name="anchor">Anchor control.</param>
        /// <param name="spacing">Optional spacing.</param>
        public static void PlaceDownLeft(Base control, Base anchor, int spacing = 0)
        {
            control.SetPosition(anchor.X, anchor.Bottom + spacing);
        }

        /// <summary>
        ///     Places the control to the right of other control (bottom aligned), taking margins into account.
        /// </summary>
        /// <param name="control">Control to place.</param>
        /// <param name="anchor">Anchor control.</param>
        /// <param name="spacing">Optional spacing.</param>
        public static void PlaceRightBottom(Base control, Base anchor, int spacing = 0)
        {
            control.SetPosition(anchor.Right + spacing, anchor.Y - control.Height + anchor.Height);
        }

    }

}
