using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components
{
    public class DescriptionComponent : ComponentBase
    {
        protected RichLabel mDescription;

        public DescriptionComponent(Base parent, string name) : base(parent, name)
        {
            GenerateComponents();
        }

        protected override void GenerateComponents()
        {
            base.GenerateComponents();

            mDescription = new RichLabel(mContainer, "Description");
        }

        /// <summary>
        /// Set the text of this component.
        /// </summary>
        /// <param name="description">The description to place on this component.</param>
        /// <param name="color">The <see cref="Color"/> the description text should render with.</param>
        public void SetText(string description, Color color)
        {
            mDescription.AddText(description, color);
            mDescription.SizeToChildren(false, true);
            mContainer.SizeToChildren(false, true);
        }

        /// <inheritdoc/>
        public override void CorrectWidth()
        {
            base.CorrectWidth();
            var margins = mDescription.Margin;

            mDescription.SetSize(mContainer.InnerWidth - margins.Right, mContainer.InnerHeight);
            mDescription.SizeToChildren(false, true);
            mDescription.SetSize(mDescription.Width, mDescription.Height + margins.Bottom);
            mContainer.SizeToChildren(false, true);
        }
    }
}
