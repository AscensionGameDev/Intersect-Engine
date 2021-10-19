using System.Collections.Generic;

using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Game.DescriptionWindows.Components;

namespace Intersect.Client.Interface.Game.DescriptionWindows
{
    public class DescriptionWindowBase : ComponentBase
    {
        // Track current Y height for placing components.
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

        /// <summary>
        /// Adds a <see cref="HeaderComponent"/> to the current window.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="loadLayout">Should we load the layour json file automatically?</param>
        /// <returns>Returns a new instance of the <see cref="HeaderComponent"/> class</returns>
        public HeaderComponent AddHeader(string name = "DescriptionWindowHeader", bool loadLayout = true)
        {
            var component = new HeaderComponent(mContainer, name);
            if (loadLayout)
            {
                component.LoadLayout();
            }
            return component;
        }

        /// <summary>
        /// Adds a <see cref="DividerComponent"/> to the current window.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="loadLayout">Should we load the layour json file automatically?</param>
        /// <returns>Returns a new instance of the <see cref="DividerComponent"/> class</returns>
        public DividerComponent AddDivider(string name = "DescriptionWindowDivider", bool loadLayout = true)
        {
            var component = new DividerComponent(mContainer, name);
            if (loadLayout)
            {
                component.LoadLayout();
            }
            return component;
        }

        /// <summary>
        /// Adds a <see cref="DescriptionComponent"/> to the current window.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="loadLayout">Should we load the layour json file automatically?</param>
        /// <returns>Returns a new instance of the <see cref="DescriptionComponent"/> class</returns>
        public DescriptionComponent AddDescription(string name = "DescriptionWindowDescription", bool loadLayout = true)
        {
            var component = new DescriptionComponent(mContainer, name);
            if (loadLayout)
            {
                component.LoadLayout();
            }
            return component;
        }

        /// <summary>
        /// Adds a <see cref="RowContainerComponent"/> to the current window.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <returns>Returns a new instance of the <see cref="DescriptionComponent"/> class</returns>
        public RowContainerComponent AddRowContainer(string name = "DescriptionWindowRowContainer")
        {
            return new RowContainerComponent(mContainer, name);
        }

        /// <summary>
        /// Positions a component correctly on the current window.
        /// </summary>
        /// <param name="component">The <see cref="ComponentBase"/> to place.</param>
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
