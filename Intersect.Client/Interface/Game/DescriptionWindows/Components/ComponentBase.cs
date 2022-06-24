using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components
{
    public partial class ComponentBase : IDisposable
    {
        protected Base mParent;

        protected string mName;

        protected ImagePanel mContainer;

        public ComponentBase(Base parent, string name = "")
        {
            mParent = parent;
            mName = name;
        }

        protected virtual void GenerateComponents()
        {
            mContainer = new ImagePanel(mParent, mName);
        }

        /// <summary>
        /// The name of this control.
        /// </summary>
        public string Name { get { return mName; } }

        /// <summary>
        /// The base container of this control.
        /// </summary>
        public ImagePanel Container => mContainer;

        /// <summary>
        /// Is this component current visible?
        /// </summary>
        public bool IsVisible => mContainer.IsVisible;

        /// <summary>
        /// The current X location of the control.
        /// </summary>
        public int X => mContainer.X;

        /// <summary>
        /// The current Y location of the control.
        /// </summary>
        public int Y => mContainer.Y;

        /// <summary>
        /// The current width of the control.
        /// </summary>
        public int Width => mContainer.Width;

        /// <summary>
        /// The current Height of the control.
        /// </summary>
        public int Height => mContainer.Height;

        /// <summary>
        /// Hide the control.
        /// </summary>
        public void Hide() => mContainer.Hide();

        /// <summary>
        /// Show the control.
        /// </summary>
        public void Show() => mContainer.Show();

        /// <summary>
        /// Sets the control position.
        /// </summary>
        /// <param name="x">The X position to move the control to.</param>
        /// <param name="y">The Y position to move the control to.</param>
        public virtual void SetPosition(int x, int y) => mContainer.SetPosition(x, y);

        /// <summary>
        /// Resizes the control to fit its children.
        /// </summary>
        /// <param name="width">Allow the control to resize its width.</param>
        /// <param name="height">Allow the control to resize its height.</param>
        public void SizeToChildren(bool width = true, bool height = true) => mContainer.SizeToChildren(width, height);

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public virtual void Dispose() => mParent.RemoveChild(mContainer, true);

        /// <summary>
        /// Load the Json layout of the current component.
        /// </summary>
        public void LoadLayout() => mContainer.LoadJsonUi(Framework.File_Management.GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        /// <summary>
        /// Corrects the width of the component compared to the parent size.
        /// </summary>
        public virtual void CorrectWidth()
        {
            mContainer.SetSize(mParent.InnerWidth, mContainer.InnerHeight);
        }
    }
}
