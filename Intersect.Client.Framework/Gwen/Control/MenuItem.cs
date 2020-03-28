using System;

using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Menu item.
    /// </summary>
    public class MenuItem : Button
    {

        private Label mAccelerator;

        private bool mCheckable;

        private bool mChecked;

        private Menu mMenu;

        private bool mOnStrip;

        private Base mSubmenuArrow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuItem" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public MenuItem(Base parent) : base(parent)
        {
            AutoSizeToContents = true;
            mOnStrip = false;
            IsTabable = false;
            IsCheckable = false;
            IsChecked = false;

            mAccelerator = new Label(this);
        }

        /// <summary>
        ///     Indicates whether the item is on a menu strip.
        /// </summary>
        public bool IsOnStrip
        {
            get => mOnStrip;
            set => mOnStrip = value;
        }

        /// <summary>
        ///     Determines if the menu item is checkable.
        /// </summary>
        public bool IsCheckable
        {
            get => mCheckable;
            set => mCheckable = value;
        }

        /// <summary>
        ///     Indicates if the parent menu is open.
        /// </summary>
        public bool IsMenuOpen
        {
            get
            {
                if (mMenu == null)
                {
                    return false;
                }

                return !mMenu.IsHidden;
            }
        }

        /// <summary>
        ///     Gets or sets the check value.
        /// </summary>
        public bool IsChecked
        {
            get => mChecked;
            set
            {
                if (value == mChecked)
                {
                    return;
                }

                mChecked = value;

                if (CheckChanged != null)
                {
                    CheckChanged.Invoke(this, EventArgs.Empty);
                }

                if (value)
                {
                    if (Checked != null)
                    {
                        Checked.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (UnChecked != null)
                    {
                        UnChecked.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the parent menu.
        /// </summary>
        public Menu Menu
        {
            get
            {
                if (null == mMenu)
                {
                    mMenu = new Menu(GetCanvas());
                    mMenu.IsHidden = true;

                    if (!mOnStrip)
                    {
                        if (mSubmenuArrow != null)
                        {
                            mSubmenuArrow.Dispose();
                        }

                        mSubmenuArrow = new RightArrow(this);
                        mSubmenuArrow.SetSize(15, 15);
                    }

                    Invalidate();
                }

                return mMenu;
            }
        }

        /// <summary>
        ///     Invoked when the item is selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> Selected;

        /// <summary>
        ///     Invoked when the item is checked.
        /// </summary>
        public event GwenEventHandler<EventArgs> Checked;

        /// <summary>
        ///     Invoked when the item is unchecked.
        /// </summary>
        public event GwenEventHandler<EventArgs> UnChecked;

        /// <summary>
        ///     Invoked when the item's check value is changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> CheckChanged;

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawMenuItem(this, IsMenuOpen, mCheckable ? mChecked : false);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            if (mSubmenuArrow != null)
            {
                mSubmenuArrow.Position(Pos.Right | Pos.CenterV, 4, 0);
            }

            base.Layout(skin);
        }

        /// <summary>
        ///     Internal OnPressed implementation.
        /// </summary>
        protected override void OnClicked(int x, int y)
        {
            if (mMenu != null)
            {
                ToggleMenu();
            }
            else if (!mOnStrip)
            {
                IsChecked = !IsChecked;
                if (Selected != null)
                {
                    Selected.Invoke(this, new ItemSelectedEventArgs(this));
                }

                GetCanvas().CloseMenus();
            }

            base.OnClicked(x, y);
        }

        /// <summary>
        ///     Toggles the menu open state.
        /// </summary>
        public void ToggleMenu()
        {
            if (IsMenuOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        /// <summary>
        ///     Opens the menu.
        /// </summary>
        public void OpenMenu()
        {
            if (null == mMenu)
            {
                return;
            }

            mMenu.IsHidden = false;
            mMenu.BringToFront();

            var p = LocalPosToCanvas(Point.Empty);

            // Strip menus open downwards
            if (mOnStrip)
            {
                mMenu.SetPosition(p.X, p.Y + Height + 1);
            }

            // Submenus open sidewards
            else
            {
                mMenu.SetPosition(p.X + Width, p.Y);
            }

            // TODO: Option this.
            // TODO: Make sure on screen, open the other side of the 
            // parent if it's better...
        }

        /// <summary>
        ///     Closes the menu.
        /// </summary>
        public void CloseMenu()
        {
            if (null == mMenu)
            {
                return;
            }

            mMenu.Close();
            mMenu.CloseAll();
        }

        public override void SizeToContents()
        {
            base.SizeToContents();
            if (mAccelerator != null)
            {
                mAccelerator.SizeToContents();
                Width = Width + mAccelerator.Width;
                mAccelerator.Alignment = Pos.Left;
            }

            if (Width < Parent.Width)
            {
                Width = Parent.Width;
            }
        }

        public MenuItem SetAction(
            GwenEventHandler<EventArgs> handler,
            GwenEventHandler<ItemSelectedEventArgs> selHandler
        )
        {
            if (mAccelerator != null)
            {
                AddAccelerator(mAccelerator.Text, handler);
            }

            Selected += selHandler;

            return this;
        }

        public void SetAccelerator(string acc)
        {
            if (mAccelerator != null)
            {
                //m_Accelerator.DelayedDelete(); // to prevent double disposing
                mAccelerator = null;
            }

            if (acc == String.Empty)
            {
                return;
            }

            mAccelerator = new Label(this);
            mAccelerator.Dock = Pos.Right;
            mAccelerator.Alignment = Pos.Right | Pos.CenterV;
            mAccelerator.Text = acc;
            mAccelerator.Margin = new Margin(0, 0, 16, 0);

            // todo
        }

    }

}
