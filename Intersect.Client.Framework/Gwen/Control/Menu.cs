using System;
using System.Linq;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.ControlInternal;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Popup menu.
    /// </summary>
    public class Menu : ScrollControl
    {

        private string mBackgroundTemplateFilename;

        private GameTexture mBackgroundTemplateTex;

        private bool mDeleteOnClose;

        private bool mDisableIconMargin;

        private GameFont mItemFont;

        //Menu Item Stuff
        private string mItemFontInfo;

        protected Color mItemHoverTextColor;

        protected Color mItemNormalTextColor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Menu" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Menu(Base parent, string name = "") : base(parent, name)
        {
            SetBounds(0, 0, 10, 10);
            Padding = Padding.Two;
            IconMarginDisabled = false;

            AutoHideBars = true;
            EnableScroll(false, true);
            DeleteOnClose = false;
            Name = name;
        }

        internal override bool IsMenuComponent => true;

        public bool IconMarginDisabled
        {
            get => mDisableIconMargin;
            set => mDisableIconMargin = value;
        }

        /// <summary>
        ///     Determines whether the menu should be disposed on close.
        /// </summary>
        public bool DeleteOnClose
        {
            get => mDeleteOnClose;
            set => mDeleteOnClose = value;
        }

        /// <summary>
        ///     Determines whether the menu should open on mouse hover.
        /// </summary>
        protected virtual bool ShouldHoverOpenMenu => true;

        /// <summary>C:\Users\JC Snider\Desktop\AGD\Intersect-Engine\Intersect.Client.Framework\Gwen\Control\Button.cs
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawMenu(this, IconMarginDisabled);
        }

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderUnder(Skin.Base skin)
        {
            base.RenderUnder(skin);
            skin.DrawShadow(this);
        }

        /// <summary>
        ///     Opens the menu.
        /// </summary>
        /// <param name="pos">Unused.</param>
        public void Open(Pos pos)
        {
            IsHidden = false;
            BringToFront();
            var mouse = Input.InputHandler.MousePosition;
            SetPosition(mouse.X, mouse.Y);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            var childrenHeight = Children.Sum(child => child != null ? child.Height : 0);

            if (childrenHeight > InnerPanel.MaximumSize.Y)
            {
                InnerPanel.MaximumSize = new Point(InnerPanel.MaximumSize.X, childrenHeight);
            }

            if (Y + childrenHeight > GetCanvas().Height)
            {
                childrenHeight = GetCanvas().Height - Y;
            }

            SetSize(Width, childrenHeight);

            base.Layout(skin);
        }

        /// <summary>
        ///     Adds a new menu item.
        /// </summary>
        /// <param name="text">Item text.</param>
        /// <returns>Newly created control.</returns>
        public virtual MenuItem AddItem(string text)
        {
            return AddItem(text, null);
        }

        /// <summary>
        ///     Adds a new menu item.
        /// </summary>
        /// <param name="text">Item text.</param>
        /// <param name="iconName">Icon texture.</param>
        /// <param name="accelerator">Accelerator for this item.</param>
        /// <returns>Newly created control.</returns>
        public virtual MenuItem AddItem(
            string text,
            GameTexture iconTexture,
            string textureFilename = "",
            string accelerator = "",
            GameFont font = null
        )
        {
            var item = new MenuItem(this);
            item.Padding = Padding.Four;
            item.SetText(text);
            item.SetImage(iconTexture, textureFilename, Button.ControlState.Normal);
            item.SetAccelerator(accelerator);
            if (font != null)
            {
                item.Font = font;
            }

            OnAddItem(item);

            return item;
        }

        /// <summary>
        ///     Add item handler.
        /// </summary>
        /// <param name="item">Item added.</param>
        protected virtual void OnAddItem(MenuItem item)
        {
            item.TextPadding = new Padding(IconMarginDisabled ? 0 : 24, 0, 16, 0);
            item.Dock = Pos.Top;
            item.SizeToContents();
            item.Alignment = Pos.CenterV | Pos.Left;
            item.HoverEnter += OnHoverItem;

            // Do this here - after Top Docking these values mean nothing in layout
            var w = item.Width + 10 + 32;
            if (w < Width)
            {
                w = Width;
            }

            SetSize(w, Height);

            UpdateItemStyles();
        }

        /// <summary>
        ///     Closes all submenus.
        /// </summary>
        public virtual void CloseAll()
        {
            //System.Diagnostics.//debug.print("Menu.CloseAll: {0}", this);
            Children.ForEach(
                child =>
                {
                    if (child is MenuItem)
                    {
                        (child as MenuItem).CloseMenu();
                    }
                }
            );
        }

        /// <summary>
        ///     Indicates whether any (sub)menu is open.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsMenuOpen()
        {
            return Children.Any(
                child =>
                {
                    if (child is MenuItem)
                    {
                        return (child as MenuItem).IsMenuOpen;
                    }

                    return false;
                }
            );
        }

        /// <summary>
        ///     Mouse hover handler.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnHoverItem(Base control, EventArgs args)
        {
            if (!ShouldHoverOpenMenu)
            {
                return;
            }

            var item = control as MenuItem;
            if (null == item)
            {
                return;
            }

            if (item.IsMenuOpen)
            {
                return;
            }

            CloseAll();
            item.OpenMenu();
        }

        /// <summary>
        ///     Closes the current menu.
        /// </summary>
        public virtual void Close()
        {
            //System.Diagnostics.//debug.print("Menu.Close: {0}", this);
            IsHidden = true;
            if (DeleteOnClose)
            {
                DelayedDelete();
            }
        }

        /// <summary>
        ///     Closes all submenus and the current menu.
        /// </summary>
        public override void CloseMenus()
        {
            //System.Diagnostics.//debug.print("Menu.CloseMenus: {0}", this);
            base.CloseMenus();
            CloseAll();
            Close();
        }

        /// <summary>
        ///     Adds a divider menu item.
        /// </summary>
        public virtual void AddDivider()
        {
            var divider = new MenuDivider(this);
            divider.Dock = Pos.Top;
            divider.Margin = new Margin(IconMarginDisabled ? 0 : 24, 0, 4, 0);
        }

        public override bool SizeToChildren(bool width = true, bool height = true)
        {
            base.SizeToChildren(width, height);
            if (width)
            {
                var maxWidth = this.Width;
                foreach (var child in Children)
                {
                    if (child.Width > maxWidth)
                    {
                        maxWidth = child.Width;
                    }
                }

                this.SetSize(maxWidth, Height);
            }

            return true;
        }

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            if (this.GetType() != typeof(CheckBox))
            {
                obj.Add("BackgroundTemplate", mBackgroundTemplateFilename);
                obj.Add("ItemTextColor", Color.ToString(mItemNormalTextColor));
                obj.Add("ItemHoveredTextColor", Color.ToString(mItemHoverTextColor));
                obj.Add("ItemFont", mItemFontInfo);
            }

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["BackgroundTemplate"] != null)
            {
                SetBackgroundTemplate(
                    GameContentManager.Current.GetTexture(
                        GameContentManager.TextureType.Gui, (string) obj["BackgroundTemplate"]
                    ), (string) obj["BackgroundTemplate"]
                );
            }

            if (obj["ItemTextColor"] != null)
            {
                mItemNormalTextColor = Color.FromString((string) obj["ItemTextColor"]);
            }

            if (obj["ItemHoveredTextColor"] != null)
            {
                mItemHoverTextColor = Color.FromString((string) obj["ItemHoveredTextColor"]);
            }

            if (obj["ItemFont"] != null && obj["ItemFont"].Type != JTokenType.Null)
            {
                var fontArr = ((string) obj["ItemFont"]).Split(',');
                mItemFontInfo = (string) obj["ItemFont"];
                mItemFont = GameContentManager.Current.GetFont(fontArr[0], int.Parse(fontArr[1]));
            }

            UpdateItemStyles();
        }

        private void UpdateItemStyles()
        {
            var menuItems = Children.Where(x => x.GetType() == typeof(MenuItem)).ToArray();
            foreach (var item in menuItems)
            {
                var itm = (MenuItem) item;
                if (mItemFont != null)
                {
                    itm.Font = mItemFont;
                }

                itm.SetTextColor(mItemNormalTextColor, Label.ControlState.Normal);
                itm.SetTextColor(mItemHoverTextColor, Label.ControlState.Hovered);
            }
        }

        public GameTexture GetTemplate()
        {
            return mBackgroundTemplateTex;
        }

        public void SetBackgroundTemplate(GameTexture texture, string fileName)
        {
            if (texture == null && !string.IsNullOrWhiteSpace(fileName))
            {
                texture = GameContentManager.Current?.GetTexture(GameContentManager.TextureType.Gui, fileName);
            }

            mBackgroundTemplateFilename = fileName;
            mBackgroundTemplateTex = texture;
        }

    }

}
