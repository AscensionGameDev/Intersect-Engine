using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components
{
    public class HeaderComponent : ComponentBase
    {
        protected ImagePanel mIconContainer;

        protected ImagePanel mIcon;

        protected Label mHeader;

        protected Label mDescription;

        public HeaderComponent(Base parent, string name) : base(parent, name)
        {
            GenerateComponents();
        }

        protected override void GenerateComponents()
        {
            base.GenerateComponents();

            mIconContainer = new ImagePanel(mContainer, "IconContainer");
            mIcon = new ImagePanel(mIconContainer, "Icon");
            mHeader = new Label(mContainer, "Header");
            mDescription = new Label(mContainer, "Description");
        }

        public void SetIcon(GameTexture texture, Color color)
        {
            mIcon.Texture = texture;
            mIcon.RenderColor = color;
            mIcon.SizeToContents();
            Align.Center(mIcon);
        }

        public void SetHeaderText(string header, Color color)
        {
            mHeader.SetText(header);
            mHeader.SetTextColor(color, Label.ControlState.Normal);
        }

        public void SetDescriptionText(string description, Color color)
        {
            mDescription.SetText(description);
            mDescription.SetTextColor(color, Label.ControlState.Normal);
        }

    }
}
