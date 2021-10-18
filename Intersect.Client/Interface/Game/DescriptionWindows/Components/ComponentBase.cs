using System;
using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components
{
    public class ComponentBase : IDisposable
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

        public string Name { get { return mName; } }

        public ImagePanel Container => mContainer;

        public bool IsVisible => mContainer.IsVisible;

        public int X => mContainer.X;

        public int Y => mContainer.Y;

        public int Width => mContainer.Width;

        public int Height => mContainer.Height;

        public void Hide() => mContainer.Hide();

        public void Show() => mContainer.Show();

        public void SetPosition(int x, int y) => mContainer.SetPosition(x, y);

        public void SizeToChildren(bool horizontal = true, bool vertical = true) => mContainer.SizeToChildren(horizontal, vertical);

        public virtual void Dispose() => mParent.RemoveChild(mContainer, true);

        public void LoadLayout() => mContainer.LoadJsonUi(Framework.File_Management.GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        public virtual void CorrectWidth()
        {
            mContainer.SetSize(mParent.InnerWidth, mContainer.InnerHeight);
        }
    }
}
