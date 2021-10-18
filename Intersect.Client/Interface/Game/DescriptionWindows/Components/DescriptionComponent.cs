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

        public void SetText(string description, Color color)
        {
            mDescription.AddText(description, color);
            mDescription.SizeToChildren(false, true);
            mContainer.SizeToChildren(false, true);
        }

        public override void CorrectWidth()
        {
            base.CorrectWidth();
            mDescription.SetSize(mContainer.InnerWidth - mDescription.Margin.Right, mContainer.InnerHeight);
            mDescription.SizeToChildren(false, true);
            mContainer.SizeToChildren(false, true);
        }
    }
}
