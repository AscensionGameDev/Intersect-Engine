using System.Collections.Generic;

using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Game.DescriptionWindows.Components;

namespace Intersect.Client.Interface.Game.DescriptionWindows
{
    public partial class DescriptionWindowBase : ComponentBase
    {
        // Track current Y height for placing components.
        private int mComponentY = 0;

        // Our internal list of components.
        private List<ComponentBase> mComponents;

        private bool mCenterOnPosition;

        public DescriptionWindowBase(Base parent, string name, bool centerOnPosition = false) : base(parent, name)
        {
            // Set up our internal component list we use for re-ordering.
            mComponents = new List<ComponentBase>();

            // Set up whether we should center on our desired position.
            mCenterOnPosition = centerOnPosition;

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
            mComponents.Add(component);
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
            mComponents.Add(component);
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
            mComponents.Add(component);
            return component;
        }

        /// <summary>
        /// Adds a <see cref="RowContainerComponent"/> to the current window.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <returns>Returns a new instance of the <see cref="DescriptionComponent"/> class</returns>
        public RowContainerComponent AddRowContainer(string name = "DescriptionWindowRowContainer")
        {
            var component = new RowContainerComponent(mContainer, name);
            mComponents.Add(component);
            return component;
        }

        /// <summary>
        /// Positions a component correctly on the current window.
        /// </summary>
        /// <param name="component">The <see cref="ComponentBase"/> to place.</param>
        private void PositionComponent(ComponentBase component)
        {
            if (component == null)
            {
                return;
            }

            component.SetPosition(component.X, mComponentY);
            mComponentY += component.Height;
        }

        /// <summary>
        /// Position and resize all components properly for display.
        /// </summary>
        protected void FinalizeWindow()
        {
            // Reset our componentY so we start from scratch!
            mComponentY = 0;

            // Correctly set our container width to the largest child start with, this way our other child components will have a width to work with.
            mContainer.SizeToChildren(true, false);

            // Resize and relocate our components to properly display on our window.
            foreach (var component in mComponents)
            {
                component.CorrectWidth();
                PositionComponent(component);
            }

            // Correctly set our container height so we display everything.
            mContainer.SizeToChildren(false, true);
        }

        /// <inheritdoc/>
        public override void SetPosition(int x, int y)
        {
            var newX = x - mContainer.Width - mContainer.Padding.Right;
            var newY = y + mContainer.Padding.Top;

            // Center on the desired position if requested.
            if (mCenterOnPosition)
            {
                newX += (mContainer.Width - mContainer.Padding.Right) / 2;
            }
            
            // Do not allow it to render outside of the screen canvas.
            if (newX < 0)
            {
                newX = 0;
            }
            else if (newX > mContainer.Canvas.Width - mContainer.Width)
            {
                newX = mContainer.Canvas.Width - mContainer.Width;
            }

            if (newY < 0)
            {
                newY = 0;
            }
            else if (newY > mContainer.Canvas.Height - mContainer.Height)
            {
                newY = mContainer.Canvas.Height - mContainer.Height;
            }

            mContainer.MoveTo(newX, newY);
        }
    }
}
