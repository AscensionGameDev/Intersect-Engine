using System;
using System.Linq;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     ComboBox control.
    /// </summary>
    public class ComboBox : Button
    {

        private readonly Base mButton;

        private string mCloseMenuSound;

        private string mHoverItemSound;

        private string mHoverMenuSound;

        private Menu mMenu;

        private bool mMenuAbove;

        //Sound Effects
        private string mOpenMenuSound;

        private MenuItem mSelectedItem;

        private string mSelectItemSound;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComboBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ComboBox(Base parent, string name = "") : base(parent, name)
        {
            SetSize(100, 20);
            mMenu = new Menu(this);
            mMenu.IsHidden = true;
            mMenu.IconMarginDisabled = true;
            mMenu.IsTabable = false;

            var arrow = new DownArrow(this);
            mButton = arrow;

            Alignment = Pos.Left | Pos.CenterV;
            Text = String.Empty;
            Margin = new Margin(3, 0, 0, 0);

            IsTabable = true;
            KeyboardInputEnabled = true;
        }

        /// <summary>
        ///     Indicates whether the combo menu is open.
        /// </summary>
        public bool IsOpen => mMenu != null && !mMenu.IsHidden;

        /// <summary>
        ///     Selected item.
        /// </summary>
        /// <remarks>Not just String property, because items also have internal names.</remarks>
        public MenuItem SelectedItem
        {
            get => mSelectedItem;
            set
            {
                if (value == null || value.Parent != mMenu)
                {
                    return;
                }

                mSelectedItem = value;
                OnItemSelected(mSelectedItem, new ItemSelectedEventArgs(value, true));
            }
        }

        internal override bool IsMenuComponent => true;

        /// <summary>
        ///     Invoked when the selected item has changed.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> ItemSelected;

        public void SetMenuAbove()
        {
            mMenuAbove = true;
            if (IsOpen)
            {
                Open();
            }
        }

        public void SetMenuBelow()
        {
            mMenuAbove = false;
            if (IsOpen)
            {
                Open();
            }
        }

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("MenuAbove", mMenuAbove);
            obj.Add("Menu", mMenu.GetJson());
            obj.Add("DropDownButton", mButton.GetJson());
            obj.Add("OpenMenuSound", mOpenMenuSound);
            obj.Add("CloseMenuSound", mCloseMenuSound);
            obj.Add("HoverMenuSound", mHoverMenuSound);
            obj.Add("ItemHoverSound", mHoverItemSound);
            obj.Add("SelectItemSound", mSelectItemSound);

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["MenuAbove"] != null)
            {
                mMenuAbove = (bool) obj["MenuAbove"];
            }

            if (obj["Menu"] != null)
            {
                mMenu.LoadJson(obj["Menu"]);
            }

            if (obj["DropDownButton"] != null)
            {
                mButton.LoadJson(obj["DropDownButton"]);
            }

            if (obj["OpenMenuSound"] != null)
            {
                mOpenMenuSound = (string) obj["OpenMenuSound"];
            }

            if (obj["CloseMenuSound"] != null)
            {
                mCloseMenuSound = (string) obj["CloseMenuSound"];
            }

            if (obj["HoverMenuSound"] != null)
            {
                mHoverMenuSound = (string) obj["HoverMenuSound"];
            }

            if (obj["ItemHoverSound"] != null)
            {
                mHoverItemSound = (string) obj["ItemHoverSound"];
            }

            if (obj["SelectItemSound"] != null)
            {
                mSelectItemSound = (string) obj["SelectItemSound"];
            }

            mOpenMenuSound = "octave-tap-warm.wav";
            mCloseMenuSound = "octave-beep-tapped.wav";
            mHoverMenuSound = "octave-tap-resonant.wav";
            mHoverItemSound = "octave-tap-resonant.wav";
            mSelectItemSound = "octave-tap-warm.wav";

            foreach (var mnu in Children)
            {
                if (mnu.GetType() == typeof(Menu))
                {
                    foreach (var itm in mnu.Children)
                    {
                        if (itm.GetType() == typeof(MenuItem))
                        {
                            ((MenuItem) itm).SetHoverSound(mHoverItemSound);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Adds a new item.
        /// </summary>
        /// <param name="label">Item label (displayed).</param>
        /// <param name="name">Item name.</param>
        /// <returns>Newly created control.</returns>
        public virtual MenuItem AddItem(string label, string name = "", object userData = null)
        {
            var item = mMenu.AddItem(label, null, "", "", this.Font);
            item.Name = name;
            item.Selected += OnItemSelected;
            item.UserData = userData;
            item.SetTextColor(GetTextColor(Label.ControlState.Normal), Label.ControlState.Normal);
            item.SetTextColor(GetTextColor(Label.ControlState.Hovered), Label.ControlState.Hovered);
            item.SetTextColor(GetTextColor(Label.ControlState.Clicked), Label.ControlState.Clicked);
            item.SetTextColor(GetTextColor(Label.ControlState.Disabled), Label.ControlState.Disabled);
            item.SetHoverSound(mHoverItemSound);

            if (mSelectedItem == null)
            {
                OnItemSelected(item, new ItemSelectedEventArgs(null, true));
            }

            return item;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawComboBox(this, IsDepressed, IsOpen);
        }

        public override void Disable()
        {
            base.Disable();
            GetCanvas().CloseMenus();
        }

        /// <summary>
        ///     Internal Pressed implementation.
        /// </summary>
        protected override void OnClicked(int x, int y)
        {
            if (IsOpen)
            {
                GetCanvas().CloseMenus();

                return;
            }

            var wasMenuHidden = mMenu.IsHidden;

            GetCanvas().CloseMenus();

            if (wasMenuHidden)
            {
                Open();
            }

            base.OnClicked(x, y);
        }

        /// <summary>
        ///     Removes all items.
        /// </summary>
        public virtual void DeleteAll()
        {
            if (mMenu != null)
            {
                mMenu.DeleteAll();
            }
        }

        /// <summary>
        ///     Internal handler for item selected event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnItemSelected(Base control, ItemSelectedEventArgs args)
        {
            if (!IsDisabled)
            {
                //Convert selected to a menu item
                var item = control as MenuItem;
                if (null == item)
                {
                    return;
                }

                mSelectedItem = item;
                Text = mSelectedItem.Text;
                mMenu.IsHidden = true;

                if (ItemSelected != null)
                {
                    ItemSelected.Invoke(this, args);
                }

                if (!args.Automated)
                {
                    base.PlaySound(mSelectItemSound);
                }

                Focus();
                Invalidate();
            }
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            mButton.Position(Pos.Right | Pos.CenterV, 4, 0);
            base.Layout(skin);
        }

        /// <summary>
        ///     Handler for losing keyboard focus.
        /// </summary>
        protected override void OnLostKeyboardFocus()
        {
            if (GetTextColor(Label.ControlState.Normal) != null)
            {
                TextColor = GetTextColor(Label.ControlState.Normal);

                return;
            }

            TextColor = Color.Black;
        }

        /// <summary>
        ///     Handler for gaining keyboard focus.
        /// </summary>
        protected override void OnKeyboardFocus()
        {
            //Until we add the blue highlighting again
            if (GetTextColor(Label.ControlState.Normal) != null)
            {
                TextColor = GetTextColor(Label.ControlState.Normal);

                return;
            }

            TextColor = Color.Black;
        }

        /// <summary>
        ///     Opens the combo.
        /// </summary>
        public virtual void Open()
        {
            if (!IsDisabled)
            {
                if (null == mMenu)
                {
                    return;
                }

                mMenu.Parent = GetCanvas();
                mMenu.IsHidden = false;
                mMenu.BringToFront();

                var p = LocalPosToCanvas(Point.Empty);
                if (mMenuAbove)
                {
                    mMenu.RestrictToParent = false;
                    mMenu.Height = mMenu.Children.Sum(child => child != null ? child.Height : 0);
                    mMenu.SetBounds(new Rectangle(p.X, p.Y - mMenu.Height, Width, mMenu.Height));
                    mMenu.RestrictToParent = true;
                }
                else
                {
                    mMenu.SetBounds(new Rectangle(p.X, p.Y + Height, Width, mMenu.Height));
                }

                base.PlaySound(mOpenMenuSound);
            }
        }

        /// <summary>
        ///     Closes the combo.
        /// </summary>
        public virtual void Close()
        {
            if (mMenu == null)
            {
                return;
            }

            mMenu.Hide();

            base.PlaySound(mCloseMenuSound);
        }

        /// <summary>
        ///     Handler for Down Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyDown(bool down)
        {
            if (down)
            {
                var it = mMenu.Children.FindIndex(x => x == mSelectedItem);
                if (it + 1 < mMenu.Children.Count)
                {
                    OnItemSelected(this, new ItemSelectedEventArgs(mMenu.Children[it + 1]));
                }
            }

            return true;
        }

        /// <summary>
        ///     Handler for Up Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyUp(bool down)
        {
            if (down)
            {
                var it = mMenu.Children.FindLastIndex(x => x == mSelectedItem);
                if (it - 1 >= 0)
                {
                    OnItemSelected(this, new ItemSelectedEventArgs(mMenu.Children[it - 1]));
                }
            }

            return true;
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderFocus(Skin.Base skin)
        {
        }

        /// <summary>
        ///     Selects the first menu item with the given text it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="label">The label to look for, this is what is shown to the user.</param>
        public void SelectByText(string text)
        {
            foreach (MenuItem item in mMenu.Children)
            {
                if (item.Text == text)
                {
                    SelectedItem = item;

                    return;
                }
            }
        }

        /// <summary>
        ///     Selects the first menu item with the given internal name it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="name">The internal name to look for. To select by what is displayed to the user, use "SelectByText".</param>
        public void SelectByName(string name)
        {
            foreach (MenuItem item in mMenu.Children)
            {
                if (item.Name == name)
                {
                    SelectedItem = item;

                    return;
                }
            }
        }

        /// <summary>
        ///     Selects the first menu item with the given user data it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="userdata">
        ///     The UserData to look for. The equivalency check uses "param.Equals(item.UserData)".
        ///     If null is passed in, it will look for null/unset UserData.
        /// </param>
        public void SelectByUserData(object userdata)
        {
            foreach (MenuItem item in mMenu.Children)
            {
                if (userdata == null)
                {
                    if (item.UserData == null)
                    {
                        SelectedItem = item;

                        return;
                    }
                }
                else if (userdata.Equals(item.UserData))
                {
                    SelectedItem = item;

                    return;
                }
            }
        }

        public void SetMenuBackgroundColor(Color clr)
        {
            mMenu.RenderColor = clr;
        }

        public void SetMenuMaxSize(int w, int h)
        {
            mMenu.MaximumSize = new Point(w, h);
        }

        public override void SetTextColor(Color clr, Label.ControlState state)
        {
            base.SetTextColor(clr, state);
            foreach (MenuItem itm in mMenu.Children)
            {
                itm.SetTextColor(clr, state);
            }
        }

        public void SetMenu(Menu menu)
        {
            mMenu = menu;
        }

        protected override void OnMouseEntered()
        {
            base.OnMouseEntered();

            //Play Mouse Entered Sound
            if (!IsOpen && ShouldDrawHover)
            {
                base.PlaySound(mHoverMenuSound);
            }
        }

    }

}
