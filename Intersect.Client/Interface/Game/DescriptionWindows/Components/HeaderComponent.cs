using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components
{
    public class HeaderComponent : ComponentBase
    {
        protected ImagePanel mIconContainer;

        protected ImagePanel mIcon;

        protected Label mTitle;

        protected Label mSubtitle;

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
            mTitle = new Label(mContainer, "Title");
            mSubtitle = new Label(mContainer, "Subtitle");
            mDescription = new Label(mContainer, "Description");
        }

        /// <summary>
        /// Set the icon on this header.
        /// </summary>
        /// <param name="texture">The <see cref="GameTexture"/> to use for display purposes.</param>
        /// <param name="color">The <see cref="Color"/> to use to display the texture.</param>
        public void SetIcon(GameTexture texture, Color color)
        {
            mIcon.Texture = texture;
            mIcon.RenderColor = color;
            mIcon.SizeToContents();
            Align.Center(mIcon);
        }

        /// <summary>
        /// Set the title on this header.
        /// </summary>
        /// <param name="title">The title text to use.</param>
        /// <param name="color">The <see cref="Color"/> to use to display this title.</param>
        public void SetTitle(string title, Color color)
        {
            mTitle.SetText(title);
            mTitle.SetTextColor(color, Label.ControlState.Normal);
        }

        /// <summary>
        /// Set the subtitle on this header.
        /// </summary>
        /// <param name="subtitle">The subtitle text to use.</param>
        /// <param name="color">The <see cref="Color"/> to use to display this subtitle.</param>
        public void SetSubtitle(string subtitle, Color color)
        {
            mSubtitle.SetText(subtitle);
            mSubtitle.SetTextColor(color, Label.ControlState.Normal);
        }

        /// <summary>
        /// Set the description on this header.
        /// </summary>
        /// <param name="description">The description text to use.</param>
        /// <param name="color">The <see cref="Color"/> to use to display this description.</param>
        public void SetDescription(string description, Color color)
        {
            mDescription.SetText(description);
            mDescription.SetTextColor(color, Label.ControlState.Normal);
        }
    }
}