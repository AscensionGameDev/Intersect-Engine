using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Game.DescriptionWindows.Components;

namespace Intersect.Client.Interface.Game.DescriptionWindows
{
    public class DescriptionWindowBase : ComponentBase
    {
        protected int mComponentY = 0;

        public DescriptionWindowBase(Base parent, string name) : base(parent, name)
        {
            GenerateComponents();
        }

        protected override void GenerateComponents()
        {
            base.GenerateComponents();

            // Load layout prior to adding components so we can retrieve padding information.
            LoadLayout();
        }

        public HeaderComponent AddHeader(string name, bool loadLayout = true)
        {
            var component = new HeaderComponent(mContainer, name);
            if (loadLayout)
            {
                component.LoadLayout();
            }
            return component;
        }

        public DividerComponent AddDivider(string name, bool loadLayout = true)
        {
            var component = new DividerComponent(mContainer, name);
            if (loadLayout)
            {
                component.LoadLayout();
            }
            return component;
        }

        public DescriptionComponent AddDescription(string name, bool loadLayout = true)
        {
            var component = new DescriptionComponent(mContainer, name);
            if (loadLayout)
            {
                component.LoadLayout();
            }
            return component;
        }

        public RowContainerComponent AddRowContainer(string name)
        {
            return new RowContainerComponent(mContainer, name);
        }

        protected void PositionComponent(ComponentBase component)
        {
            if (component == null)
            {
                return;
            }

            component.SetPosition(component.X, mComponentY);
            mComponentY += component.Height;
        }
    }
}
