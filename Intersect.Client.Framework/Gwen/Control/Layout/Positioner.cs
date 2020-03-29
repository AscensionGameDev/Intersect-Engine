namespace Intersect.Client.Framework.Gwen.Control.Layout
{

    /// <summary>
    ///     Helper control that positions its children in a specific way.
    /// </summary>
    public class Positioner : Base
    {

        private Pos mPos;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Positioner" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Positioner(Base parent) : base(parent)
        {
            Pos = Pos.Left | Pos.Top;
        }

        /// <summary>
        ///     Children position.
        /// </summary>
        public Pos Pos
        {
            get => mPos;
            set => mPos = value;
        }

        /// <summary>
        ///     Function invoked after layout.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void PostLayout(Skin.Base skin)
        {
            foreach (var child in Children) // ok?
            {
                child.Position(mPos);
            }
        }

    }

    /// <summary>
    ///     Helper class that centers all its children.
    /// </summary>
    public class Center : Positioner
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Center" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Center(Base parent) : base(parent)
        {
            Pos = Pos.Center;
        }

    }

}
