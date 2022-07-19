using System;
using System.Linq;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.Skin.Texturing;

using Single = Intersect.Client.Framework.Gwen.Skin.Texturing.Single;

namespace Intersect.Client.Framework.Gwen.Skin
{

    #region UI element textures

    public partial struct SkinTextures
    {

        public Bordered StatusBar;

        public Bordered Selection;

        public Bordered Shadow;

        public Bordered Tooltip;

        public partial struct _Panel
        {

            public Bordered Normal;

            public Bordered Bright;

            public Bordered Dark;

            public Bordered Highlight;

        }

        public partial struct _Window
        {

            public Bordered Normal;

            public Bordered Inactive;

            public Single Close;

            public Single CloseHover;

            public Single CloseDown;

            public Single CloseDisabled;

        }
        public partial struct _FillableButton
        {
            public Single Box;

            public Single Fill;
        }

        public partial struct _CheckBox
        {

            public partial struct _Baked
            {
                public Single Normal;

                public Single Checked;
            }

            public _Baked Active_Baked;

            public _Baked Disabled_Baked;

            public _FillableButton Default;

            public _FillableButton Active;

            public _FillableButton Hovered;

            public _FillableButton Disabled;
        }

        public partial struct _RadioButton
        {
            public partial struct _Baked
            {
                public Single Normal;

                public Single Checked;
            }

            public _Baked Active_Baked;

            public _Baked Disabled_Baked;

            public _FillableButton Default;

            public _FillableButton Active;

            public _FillableButton Hovered;

            public _FillableButton Disabled;
        }

        public partial struct _TextBox
        {

            public Bordered Normal;

            public Bordered Focus;

            public Bordered Disabled;

        }

        public partial struct _Tree
        {

            public Bordered Background;

            public Single Minus;

            public Single Plus;

        }

        public partial struct _ProgressBar
        {

            public Bordered Back;

            public Bordered Front;

        }

        public partial struct _Scroller
        {

            public Bordered TrackV;

            public Bordered TrackH;

            public Bordered ButtonVNormal;

            public Bordered ButtonVHover;

            public Bordered ButtonVDown;

            public Bordered ButtonVDisabled;

            public Bordered ButtonHNormal;

            public Bordered ButtonHHover;

            public Bordered ButtonHDown;

            public Bordered ButtonHDisabled;

            public partial struct _Button
            {

                public Bordered[] Normal;

                public Bordered[] Hover;

                public Bordered[] Down;

                public Bordered[] Disabled;

            }

            public _Button Button;

        }

        public partial struct _Menu
        {

            public Single RightArrow;

            public Single Check;

            public Bordered Strip;

            public Bordered Background;

            public Bordered BackgroundWithMargin;

            public Bordered Hover;

        }

        public partial struct _Input
        {

            public partial struct _Button
            {

                public Bordered Normal;

                public Bordered Hovered;

                public Bordered Disabled;

                public Bordered Pressed;

            }

            public partial struct _ComboBox
            {

                public Bordered Normal;

                public Bordered Hover;

                public Bordered Down;

                public Bordered Disabled;

                public partial struct _Button
                {

                    public Single Normal;

                    public Single Hover;

                    public Single Down;

                    public Single Disabled;

                }

                public _Button Button;

            }

            public partial struct _Slider
            {

                public partial struct _H
                {

                    public Single Normal;

                    public Single Hover;

                    public Single Down;

                    public Single Disabled;

                }

                public partial struct _V
                {

                    public Single Normal;

                    public Single Hover;

                    public Single Down;

                    public Single Disabled;

                }

                public _H H;

                public _V V;

            }

            public partial struct _ListBox
            {

                public Bordered Background;

                public Bordered Hovered;

                public Bordered EvenLine;

                public Bordered OddLine;

                public Bordered EvenLineSelected;

                public Bordered OddLineSelected;

            }

            public partial struct _UpDown
            {

                public partial struct _Up
                {

                    public Single Normal;

                    public Single Hover;

                    public Single Down;

                    public Single Disabled;

                }

                public partial struct _Down
                {

                    public Single Normal;

                    public Single Hover;

                    public Single Down;

                    public Single Disabled;

                }

                public _Up Up;

                public _Down Down;

            }

            public _Button Button;

            public _ComboBox ComboBox;

            public _Slider Slider;

            public _ListBox ListBox;

            public _UpDown UpDown;

        }

        public partial struct _Tab
        {

            public partial struct _Bottom
            {

                public Bordered Inactive;

                public Bordered Active;

            }

            public partial struct _Top
            {

                public Bordered Inactive;

                public Bordered Active;

            }

            public partial struct _Left
            {

                public Bordered Inactive;

                public Bordered Active;

            }

            public partial struct _Right
            {

                public Bordered Inactive;

                public Bordered Active;

            }

            public _Bottom Bottom;

            public _Top Top;

            public _Left Left;

            public _Right Right;

            public Bordered Control;

            public Bordered HeaderBar;

        }

        public partial struct _CategoryList
        {

            public Bordered Outer;

            public Bordered Inner;

            public Bordered Header;

        }

        public _Panel Panel;

        public _Window Window;

        public _CheckBox CheckBox;

        public _RadioButton RadioButton;

        public _TextBox TextBox;

        public _Tree Tree;

        public _ProgressBar ProgressBar;

        public _Scroller Scroller;

        public _Menu Menu;

        public _Input Input;

        public _Tab Tab;

        public _CategoryList CategoryList;

    }

    #endregion

    /// <summary>
    ///     Base textured skin.
    /// </summary>
    public partial class TexturedBase : Skin.Base
    {

        public static TexturedBase FindSkin(Renderer.Base renderer, GameContentManager contentManager, string skinName)
        {
            var skinMap = typeof(TexturedBase).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(TexturedBase).IsAssignableFrom(type))
                .ToDictionary(type => type.Name.ToLowerInvariant(), type => type);

            if (skinMap.TryGetValue(skinName.ToLowerInvariant(), out var skinType))
            {
                try
                {
                    var skin = Activator.CreateInstance(skinType, renderer, contentManager) as TexturedBase;
                    return skin;
                }
                catch
                {
                    // ignore
                }
            }

            return new TexturedBase(renderer, contentManager, skinName);
        }

        protected readonly GameTexture mTexture;

        protected SkinTextures mTextures;

        public string TextureName { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TexturedBase" /> class.
        /// </summary>
        /// <param name="renderer">Renderer to use.</param>
        /// <param name="textureName">Name of the skin texture map.</param>
        public TexturedBase(Renderer.Base renderer, GameTexture texture) : base(renderer)
        {
            mTexture = texture;

            InitializeColors();
            InitializeTextures();
        }

        public TexturedBase(Renderer.Base renderer, GameContentManager contentManager)
            : this(renderer, contentManager, "defaultskin.png")
        {
        }

        protected TexturedBase(Renderer.Base renderer, GameContentManager contentManager, string textureName)
            : this(renderer, contentManager?.GetTexture(Framework.Content.TextureType.Gui, textureName))
        {
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #region Initialization

        protected virtual void InitializeColors()
        {
            Colors.Window.TitleActive = Renderer.PixelColor(mTexture, 4 + 8 * 0, 508, Color.Red);
            Colors.Window.TitleInactive = Renderer.PixelColor(mTexture, 4 + 8 * 1, 508, Color.Yellow);

            Colors.Button.Normal = Renderer.PixelColor(mTexture, 4 + 8 * 2, 508, Color.Yellow);
            Colors.Button.Hover = Renderer.PixelColor(mTexture, 4 + 8 * 3, 508, Color.Yellow);
            Colors.Button.Down = Renderer.PixelColor(mTexture, 4 + 8 * 2, 500, Color.Yellow);
            Colors.Button.Disabled = Renderer.PixelColor(mTexture, 4 + 8 * 3, 500, Color.Yellow);

            Colors.Tab.Active.Normal = Renderer.PixelColor(mTexture, 4 + 8 * 4, 508, Color.Yellow);
            Colors.Tab.Active.Hover = Renderer.PixelColor(mTexture, 4 + 8 * 5, 508, Color.Yellow);
            Colors.Tab.Active.Down = Renderer.PixelColor(mTexture, 4 + 8 * 4, 500, Color.Yellow);
            Colors.Tab.Active.Disabled = Renderer.PixelColor(mTexture, 4 + 8 * 5, 500, Color.Yellow);
            Colors.Tab.Inactive.Normal = Renderer.PixelColor(mTexture, 4 + 8 * 6, 508, Color.Yellow);
            Colors.Tab.Inactive.Hover = Renderer.PixelColor(mTexture, 4 + 8 * 7, 508, Color.Yellow);
            Colors.Tab.Inactive.Down = Renderer.PixelColor(mTexture, 4 + 8 * 6, 500, Color.Yellow);
            Colors.Tab.Inactive.Disabled = Renderer.PixelColor(mTexture, 4 + 8 * 7, 500, Color.Yellow);

            Colors.Label.Default = Renderer.PixelColor(mTexture, 4 + 8 * 8, 508, Color.Yellow);
            Colors.Label.Bright = Renderer.PixelColor(mTexture, 4 + 8 * 9, 508, Color.Yellow);
            Colors.Label.Dark = Renderer.PixelColor(mTexture, 4 + 8 * 8, 500, Color.Yellow);
            Colors.Label.Highlight = Renderer.PixelColor(mTexture, 4 + 8 * 9, 500, Color.Yellow);

            Colors.Tree.Lines = Renderer.PixelColor(mTexture, 4 + 8 * 10, 508, Color.Yellow);
            Colors.Tree.Normal = Renderer.PixelColor(mTexture, 4 + 8 * 11, 508, Color.Yellow);
            Colors.Tree.Hover = Renderer.PixelColor(mTexture, 4 + 8 * 10, 500, Color.Yellow);
            Colors.Tree.Selected = Renderer.PixelColor(mTexture, 4 + 8 * 11, 500, Color.Yellow);

            Colors.Properties.LineNormal = Renderer.PixelColor(mTexture, 4 + 8 * 12, 508, Color.Yellow);
            Colors.Properties.LineSelected = Renderer.PixelColor(mTexture, 4 + 8 * 13, 508, Color.Yellow);
            Colors.Properties.LineHover = Renderer.PixelColor(mTexture, 4 + 8 * 12, 500, Color.Yellow);
            Colors.Properties.Title = Renderer.PixelColor(mTexture, 4 + 8 * 13, 500, Color.Yellow);
            Colors.Properties.ColumnNormal = Renderer.PixelColor(mTexture, 4 + 8 * 14, 508, Color.Yellow);
            Colors.Properties.ColumnSelected = Renderer.PixelColor(mTexture, 4 + 8 * 15, 508, Color.Yellow);
            Colors.Properties.ColumnHover = Renderer.PixelColor(mTexture, 4 + 8 * 14, 500, Color.Yellow);
            Colors.Properties.Border = Renderer.PixelColor(mTexture, 4 + 8 * 15, 500, Color.Yellow);
            Colors.Properties.LabelNormal = Renderer.PixelColor(mTexture, 4 + 8 * 16, 508, Color.Yellow);
            Colors.Properties.LabelSelected = Renderer.PixelColor(mTexture, 4 + 8 * 17, 508, Color.Yellow);
            Colors.Properties.LabelHover = Renderer.PixelColor(mTexture, 4 + 8 * 16, 500, Color.Yellow);

            Colors.ModalBackground = Renderer.PixelColor(mTexture, 4 + 8 * 18, 508, Color.Yellow);

            Colors.TooltipText = Renderer.PixelColor(mTexture, 4 + 8 * 19, 508, Color.Yellow);

            Colors.Category.Header = Renderer.PixelColor(mTexture, 4 + 8 * 18, 500, Color.Yellow);
            Colors.Category.HeaderClosed = Renderer.PixelColor(mTexture, 4 + 8 * 19, 500, Color.Yellow);
            Colors.Category.Line.Text = Renderer.PixelColor(mTexture, 4 + 8 * 20, 508, Color.Yellow);
            Colors.Category.Line.TextHover = Renderer.PixelColor(mTexture, 4 + 8 * 21, 508, Color.Yellow);
            Colors.Category.Line.TextSelected = Renderer.PixelColor(mTexture, 4 + 8 * 20, 500, Color.Yellow);
            Colors.Category.Line.Button = Renderer.PixelColor(mTexture, 4 + 8 * 21, 500, Color.Yellow);
            Colors.Category.Line.ButtonHover = Renderer.PixelColor(mTexture, 4 + 8 * 22, 508, Color.Yellow);
            Colors.Category.Line.ButtonSelected = Renderer.PixelColor(mTexture, 4 + 8 * 23, 508, Color.Yellow);
            Colors.Category.LineAlt.Text = Renderer.PixelColor(mTexture, 4 + 8 * 22, 500, Color.Yellow);
            Colors.Category.LineAlt.TextHover = Renderer.PixelColor(mTexture, 4 + 8 * 23, 500, Color.Yellow);
            Colors.Category.LineAlt.TextSelected = Renderer.PixelColor(mTexture, 4 + 8 * 24, 508, Color.Yellow);
            Colors.Category.LineAlt.Button = Renderer.PixelColor(mTexture, 4 + 8 * 25, 508, Color.Yellow);
            Colors.Category.LineAlt.ButtonHover = Renderer.PixelColor(mTexture, 4 + 8 * 24, 500, Color.Yellow);
            Colors.Category.LineAlt.ButtonSelected = Renderer.PixelColor(mTexture, 4 + 8 * 25, 500, Color.Yellow);
        }

        protected virtual void InitializeTextures()
        {
            mTextures.Shadow = new Bordered(mTexture, 448, 0, 31, 31, Margin.Eight);
            mTextures.Tooltip = new Bordered(mTexture, 128, 320, 127, 31, Margin.Eight);
            mTextures.StatusBar = new Bordered(mTexture, 128, 288, 127, 31, Margin.Eight);
            mTextures.Selection = new Bordered(mTexture, 384, 32, 31, 31, Margin.Four);

            mTextures.Panel.Normal = new Bordered(mTexture, 256, 0, 63, 63, new Margin(16, 16, 16, 16));
            mTextures.Panel.Bright = new Bordered(mTexture, 256 + 64, 0, 63, 63, new Margin(16, 16, 16, 16));
            mTextures.Panel.Dark = new Bordered(mTexture, 256, 64, 63, 63, new Margin(16, 16, 16, 16));
            mTextures.Panel.Highlight = new Bordered(mTexture, 256 + 64, 64, 63, 63, new Margin(16, 16, 16, 16));

            mTextures.Window.Normal = new Bordered(mTexture, 0, 0, 127, 127, new Margin(8, 32, 8, 8));
            mTextures.Window.Inactive = new Bordered(mTexture, 128, 0, 127, 127, new Margin(8, 32, 8, 8));

            mTextures.CheckBox.Active_Baked.Checked = new Single(mTexture, 448, 32, 15, 15);
            mTextures.CheckBox.Active_Baked.Normal = new Single(mTexture, 464, 32, 15, 15);
            mTextures.CheckBox.Disabled_Baked.Normal = new Single(mTexture, 448, 48, 15, 15);
            mTextures.CheckBox.Disabled_Baked.Normal = new Single(mTexture, 464, 48, 15, 15);

            mTextures.RadioButton.Active_Baked.Checked = new Single(mTexture, 448, 64, 15, 15);
            mTextures.RadioButton.Active_Baked.Normal = new Single(mTexture, 464, 64, 15, 15);
            mTextures.RadioButton.Disabled_Baked.Normal = new Single(mTexture, 448, 80, 15, 15);
            mTextures.RadioButton.Disabled_Baked.Normal = new Single(mTexture, 464, 80, 15, 15);

            mTextures.TextBox.Normal = new Bordered(mTexture, 0, 150, 127, 21, Margin.Four);
            mTextures.TextBox.Focus = new Bordered(mTexture, 0, 172, 127, 21, Margin.Four);
            mTextures.TextBox.Disabled = new Bordered(mTexture, 0, 193, 127, 21, Margin.Four);

            mTextures.Menu.Strip = new Bordered(mTexture, 0, 128, 127, 21, Margin.One);
            mTextures.Menu.BackgroundWithMargin = new Bordered(mTexture, 128, 128, 127, 63, new Margin(24, 8, 8, 8));
            mTextures.Menu.Background = new Bordered(mTexture, 128, 192, 127, 63, Margin.Eight);
            mTextures.Menu.Hover = new Bordered(mTexture, 128, 256, 127, 31, Margin.Eight);
            mTextures.Menu.RightArrow = new Single(mTexture, 464, 112, 15, 15);
            mTextures.Menu.Check = new Single(mTexture, 448, 112, 15, 15);

            mTextures.Tab.Control = new Bordered(mTexture, 0, 256, 127, 127, Margin.Eight);
            mTextures.Tab.Bottom.Active = new Bordered(mTexture, 0, 416, 63, 31, Margin.Eight);
            mTextures.Tab.Bottom.Inactive = new Bordered(mTexture, 0 + 128, 416, 63, 31, Margin.Eight);
            mTextures.Tab.Top.Active = new Bordered(mTexture, 0, 384, 63, 31, Margin.Eight);
            mTextures.Tab.Top.Inactive = new Bordered(mTexture, 0 + 128, 384, 63, 31, Margin.Eight);
            mTextures.Tab.Left.Active = new Bordered(mTexture, 64, 384, 31, 63, Margin.Eight);
            mTextures.Tab.Left.Inactive = new Bordered(mTexture, 64 + 128, 384, 31, 63, Margin.Eight);
            mTextures.Tab.Right.Active = new Bordered(mTexture, 96, 384, 31, 63, Margin.Eight);
            mTextures.Tab.Right.Inactive = new Bordered(mTexture, 96 + 128, 384, 31, 63, Margin.Eight);
            mTextures.Tab.HeaderBar = new Bordered(mTexture, 128, 352, 127, 31, Margin.Four);

            mTextures.Window.Close = new Single(mTexture, 0, 224, 24, 24);
            mTextures.Window.CloseHover = new Single(mTexture, 32, 224, 24, 24);
            mTextures.Window.CloseHover = new Single(mTexture, 64, 224, 24, 24);
            mTextures.Window.CloseHover = new Single(mTexture, 96, 224, 24, 24);

            mTextures.Scroller.TrackV = new Bordered(mTexture, 384, 208, 15, 127, Margin.Four);
            mTextures.Scroller.ButtonVNormal = new Bordered(mTexture, 384 + 16, 208, 15, 127, Margin.Four);
            mTextures.Scroller.ButtonVHover = new Bordered(mTexture, 384 + 32, 208, 15, 127, Margin.Four);
            mTextures.Scroller.ButtonVDown = new Bordered(mTexture, 384 + 48, 208, 15, 127, Margin.Four);
            mTextures.Scroller.ButtonVDisabled = new Bordered(mTexture, 384 + 64, 208, 15, 127, Margin.Four);
            mTextures.Scroller.TrackH = new Bordered(mTexture, 384, 128, 127, 15, Margin.Four);
            mTextures.Scroller.ButtonHNormal = new Bordered(mTexture, 384, 128 + 16, 127, 15, Margin.Four);
            mTextures.Scroller.ButtonHHover = new Bordered(mTexture, 384, 128 + 32, 127, 15, Margin.Four);
            mTextures.Scroller.ButtonHDown = new Bordered(mTexture, 384, 128 + 48, 127, 15, Margin.Four);
            mTextures.Scroller.ButtonHDisabled = new Bordered(mTexture, 384, 128 + 64, 127, 15, Margin.Four);

            mTextures.Scroller.Button.Normal = new Bordered[4];
            mTextures.Scroller.Button.Disabled = new Bordered[4];
            mTextures.Scroller.Button.Hover = new Bordered[4];
            mTextures.Scroller.Button.Down = new Bordered[4];

            mTextures.Tree.Background = new Bordered(mTexture, 256, 128, 127, 127, new Margin(16, 16, 16, 16));
            mTextures.Tree.Plus = new Single(mTexture, 448, 96, 15, 15);
            mTextures.Tree.Minus = new Single(mTexture, 464, 96, 15, 15);

            mTextures.Input.Button.Normal = new Bordered(mTexture, 480, 0, 31, 31, Margin.Eight);
            mTextures.Input.Button.Hovered = new Bordered(mTexture, 480, 32, 31, 31, Margin.Eight);
            mTextures.Input.Button.Disabled = new Bordered(mTexture, 480, 64, 31, 31, Margin.Eight);
            mTextures.Input.Button.Pressed = new Bordered(mTexture, 480, 96, 31, 31, Margin.Eight);

            for (var i = 0; i < 4; i++)
            {
                mTextures.Scroller.Button.Normal[i] = new Bordered(mTexture, 464 + 0, 208 + i * 16, 15, 15, Margin.Two);
                mTextures.Scroller.Button.Hover[i] = new Bordered(mTexture, 480, 208 + i * 16, 15, 15, Margin.Two);
                mTextures.Scroller.Button.Down[i] = new Bordered(mTexture, 464, 272 + i * 16, 15, 15, Margin.Two);
                mTextures.Scroller.Button.Disabled[i] = new Bordered(
                    mTexture, 480 + 48, 272 + i * 16, 15, 15, Margin.Two
                );
            }

            mTextures.Input.ListBox.Background = new Bordered(mTexture, 256, 256, 63, 127, Margin.Eight);
            mTextures.Input.ListBox.Hovered = new Bordered(mTexture, 320, 320, 31, 31, Margin.Eight);
            mTextures.Input.ListBox.EvenLine = new Bordered(mTexture, 352, 256, 31, 31, Margin.Eight);
            mTextures.Input.ListBox.OddLine = new Bordered(mTexture, 352, 288, 31, 31, Margin.Eight);
            mTextures.Input.ListBox.EvenLineSelected = new Bordered(mTexture, 320, 270, 31, 31, Margin.Eight);
            mTextures.Input.ListBox.OddLineSelected = new Bordered(mTexture, 320, 288, 31, 31, Margin.Eight);

            mTextures.Input.ComboBox.Normal = new Bordered(mTexture, 384, 336, 127, 31, new Margin(8, 8, 32, 8));
            mTextures.Input.ComboBox.Hover = new Bordered(mTexture, 384, 336 + 32, 127, 31, new Margin(8, 8, 32, 8));
            mTextures.Input.ComboBox.Down = new Bordered(mTexture, 384, 336 + 64, 127, 31, new Margin(8, 8, 32, 8));
            mTextures.Input.ComboBox.Disabled = new Bordered(mTexture, 384, 336 + 96, 127, 31, new Margin(8, 8, 32, 8));

            mTextures.Input.ComboBox.Button.Normal = new Single(mTexture, 496, 272, 15, 15);
            mTextures.Input.ComboBox.Button.Hover = new Single(mTexture, 496, 272 + 16, 15, 15);
            mTextures.Input.ComboBox.Button.Down = new Single(mTexture, 496, 272 + 32, 15, 15);
            mTextures.Input.ComboBox.Button.Disabled = new Single(mTexture, 496, 272 + 48, 15, 15);

            mTextures.Input.UpDown.Up.Normal = new Single(mTexture, 384, 112, 7, 7);
            mTextures.Input.UpDown.Up.Hover = new Single(mTexture, 384 + 8, 112, 7, 7);
            mTextures.Input.UpDown.Up.Down = new Single(mTexture, 384 + 16, 112, 7, 7);
            mTextures.Input.UpDown.Up.Disabled = new Single(mTexture, 384 + 24, 112, 7, 7);
            mTextures.Input.UpDown.Down.Normal = new Single(mTexture, 384, 120, 7, 7);
            mTextures.Input.UpDown.Down.Hover = new Single(mTexture, 384 + 8, 120, 7, 7);
            mTextures.Input.UpDown.Down.Down = new Single(mTexture, 384 + 16, 120, 7, 7);
            mTextures.Input.UpDown.Down.Disabled = new Single(mTexture, 384 + 24, 120, 7, 7);

            mTextures.ProgressBar.Back = new Bordered(mTexture, 384, 0, 31, 31, Margin.Eight);
            mTextures.ProgressBar.Front = new Bordered(mTexture, 384 + 32, 0, 31, 31, Margin.Eight);

            mTextures.Input.Slider.H.Normal = new Single(mTexture, 416, 32, 15, 15);
            mTextures.Input.Slider.H.Hover = new Single(mTexture, 416, 32 + 16, 15, 15);
            mTextures.Input.Slider.H.Down = new Single(mTexture, 416, 32 + 32, 15, 15);
            mTextures.Input.Slider.H.Disabled = new Single(mTexture, 416, 32 + 48, 15, 15);

            mTextures.Input.Slider.V.Normal = new Single(mTexture, 416 + 16, 32, 15, 15);
            mTextures.Input.Slider.V.Hover = new Single(mTexture, 416 + 16, 32 + 16, 15, 15);
            mTextures.Input.Slider.V.Down = new Single(mTexture, 416 + 16, 32 + 32, 15, 15);
            mTextures.Input.Slider.V.Disabled = new Single(mTexture, 416 + 16, 32 + 48, 15, 15);

            mTextures.CategoryList.Outer = new Bordered(mTexture, 256, 384, 63, 63, Margin.Eight);
            mTextures.CategoryList.Inner = new Bordered(mTexture, 256 + 64, 384, 63, 63, new Margin(8, 21, 8, 8));
            mTextures.CategoryList.Header = new Bordered(mTexture, 320, 352, 63, 31, Margin.Eight);
        }

        #endregion

        #region UI elements

        public override void DrawButton(Control.Base control, bool depressed, bool hovered, bool disabled)
        {
            GameTexture renderImg = null;
            var textColor = ((Button) control).GetTextColor(Control.Label.ControlState.Normal);
            if (disabled && ((Button) control).GetTextColor(Control.Label.ControlState.Disabled) != null)
            {
                textColor = ((Button) control).GetTextColor(Control.Label.ControlState.Disabled);
            }
            else if (depressed && ((Button) control).GetTextColor(Control.Label.ControlState.Clicked) != null)
            {
                textColor = ((Button) control).GetTextColor(Control.Label.ControlState.Clicked);
            }
            else if (hovered && ((Button) control).GetTextColor(Control.Label.ControlState.Hovered) != null)
            {
                textColor = ((Button) control).GetTextColor(Control.Label.ControlState.Hovered);
            }

            if (textColor != null)
            {
                ((Button) control).TextColorOverride = textColor;
            }

            if (disabled && ((Button) control).GetImage(Control.Button.ControlState.Disabled) != null)
            {
                renderImg = ((Button) control).GetImage(Control.Button.ControlState.Disabled);
            }
            else if (!disabled && depressed && ((Button) control).GetImage(Control.Button.ControlState.Clicked) != null)
            {
                renderImg = ((Button) control).GetImage(Control.Button.ControlState.Clicked);
            }
            else if (!disabled && hovered && ((Button) control).GetImage(Control.Button.ControlState.Hovered) != null)
            {
                renderImg = ((Button) control).GetImage(Control.Button.ControlState.Hovered);
            }
            else if (((Button) control).GetImage(Control.Button.ControlState.Normal) != null)
            {
                renderImg = ((Button) control).GetImage(Control.Button.ControlState.Normal);
            }

            if (renderImg != null)
            {
                Renderer.DrawColor = control.RenderColor;
                Renderer.DrawTexturedRect(renderImg, control.RenderBounds, control.RenderColor);

                return;
            }

            if (disabled)
            {
                mTextures.Input.Button.Disabled.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (depressed)
            {
                mTextures.Input.Button.Pressed.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (hovered)
            {
                mTextures.Input.Button.Hovered.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            mTextures.Input.Button.Normal.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawMenuRightArrow(Control.Base control)
        {
            mTextures.Menu.RightArrow.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawMenuItem(Control.Base control, bool submenuOpen, bool isChecked)
        {
            if (!control.ShouldDrawBackground)
            {
                return;
            }

            //if (submenuOpen || control.IsHovered)
            //    mTextures.Menu.Hover.Draw(Renderer, control.RenderBounds, control.RenderColor);

            if (isChecked)
            {
                mTextures.Menu.Check.Draw(
                    Renderer, new Rectangle(control.RenderBounds.X + 4, control.RenderBounds.Y + 3, 15, 15),
                    control.RenderColor
                );
            }
        }

        public override void DrawMenuStrip(Control.Base control)
        {
            mTextures.Menu.Strip.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawMenu(Control.Base control, bool paddingDisabled)
        {
            if (!control.ShouldDrawBackground)
            {
                return;
            }

            if (((Menu) control).GetTemplate() != null)
            {
                var renderImg = ((Menu) control).GetTemplate();

                //Draw Top Left Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Y, 2, 2), control.RenderColor,
                    0, 0, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                );

                //Draw Top
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Y, control.RenderBounds.Width - 4, 2
                    ), control.RenderColor, 2f / renderImg.GetWidth(), 0,
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                );

                //Draw Top Right Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Y, 2, 2),
                    control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 0, 1f,
                    2f / renderImg.GetHeight()
                );

                //Draw Left
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X, control.RenderBounds.Y + 2, 2, control.RenderBounds.Height - 4
                    ), control.RenderColor, 0, 2f / renderImg.GetHeight(), 2f / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                //Draw Middle
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Y + 2, control.RenderBounds.Width - 4,
                        control.RenderBounds.Height - 4
                    ), control.RenderColor, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight(),
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                //Draw Right
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.Width - 2, control.RenderBounds.Y + 2, 2, control.RenderBounds.Height - 4
                    ), control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    2f / renderImg.GetHeight(), 1, (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                // Draw Bottom Left Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Bottom - 2, 2, 2),
                    control.RenderColor, 0, (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                    2f / renderImg.GetWidth(), 1f
                );

                //Draw Bottom
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Bottom - 2, control.RenderBounds.Width - 4, 2
                    ), control.RenderColor, 2f / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 1f
                );

                //Draw Bottom Right Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Bottom - 2, 2, 2),
                    control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight(), 1f, 1f
                );

                return;
            }

            if (!paddingDisabled)
            {
                mTextures.Menu.BackgroundWithMargin.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            mTextures.Menu.Background.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawShadow(Control.Base control)
        {
            var r = control.RenderBounds;
            r.X -= 4;
            r.Y -= 4;
            r.Width += 10;
            r.Height += 10;
            mTextures.Shadow.Draw(Renderer, r, control.RenderColor);
        }

        public override void DrawRadioButton(Control.Base control, bool selected, bool depressed)
        {
            if (TryGetOverrideTexture(control as CheckBox, selected, depressed, out var overrideTexture))
            {
                Renderer.DrawColor = control.RenderColor;
                Renderer.DrawTexturedRect(overrideTexture, control.RenderBounds, control.RenderColor, 0, 0);
                return;
            }

            if (selected)
            {
                if (control.IsDisabled)
                {
                    mTextures.RadioButton.Disabled_Baked.Checked.Draw(Renderer, control.RenderBounds, control.RenderColor);
                }
                else
                {
                    mTextures.RadioButton.Active_Baked.Checked.Draw(Renderer, control.RenderBounds, control.RenderColor);
                }
            }
            else
            {
                if (control.IsDisabled)
                {
                    mTextures.RadioButton.Disabled_Baked.Normal.Draw(Renderer, control.RenderBounds, control.RenderColor);
                }
                else
                {
                    mTextures.RadioButton.Active_Baked.Normal.Draw(Renderer, control.RenderBounds, control.RenderColor);
                }
            }
        }

        protected bool TryGetOverrideTexture(CheckBox control, bool selected, bool pressed, out GameTexture overrideTexture)
        {
            CheckBox.ControlState controlState = CheckBox.ControlState.Normal;
            if (selected)
            {
                controlState = control.IsDisabled ? CheckBox.ControlState.CheckedDisabled : CheckBox.ControlState.CheckedNormal;
            }
            else if (control.IsDisabled)
            {
                controlState = CheckBox.ControlState.Disabled;
            }

            overrideTexture = control.GetImage(controlState);
            return overrideTexture != default;
        }

        public override void DrawCheckBox(Control.Base control, bool selected, bool depressed)
        {
            if (TryGetOverrideTexture(control as CheckBox, selected, depressed, out var overrideTexture))
            {
                Renderer.DrawColor = control.RenderColor;
                Renderer.DrawTexturedRect(overrideTexture, control.RenderBounds, control.RenderColor, 0, 0);
                return;
            }

            if (selected)
            {
                if (control.IsDisabled)
                {
                    mTextures.CheckBox.Disabled_Baked.Checked.Draw(Renderer, control.RenderBounds, control.RenderColor);
                }
                else
                {
                    mTextures.CheckBox.Active_Baked.Checked.Draw(Renderer, control.RenderBounds, control.RenderColor);
                }
            }
            else
            {
                if (control.IsDisabled)
                {
                    mTextures.CheckBox.Disabled_Baked.Normal.Draw(Renderer, control.RenderBounds, control.RenderColor);
                }
                else
                {
                    mTextures.CheckBox.Active_Baked.Normal.Draw(Renderer, control.RenderBounds, control.RenderColor);
                }
            }
        }

        public override void DrawGroupBox(Control.Base control, int textStart, int textHeight, int textWidth)
        {
            var rect = control.RenderBounds;

            rect.Y += (int) (textHeight * 0.5f);
            rect.Height -= (int) (textHeight * 0.5f);

            var colDarker = Color.FromArgb(50, 0, 50, 60);
            var colLighter = Color.FromArgb(150, 255, 255, 255);

            Renderer.DrawColor = colLighter;

            Renderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + 1, textStart - 3, 1));
            Renderer.DrawFilledRect(
                new Rectangle(rect.X + 1 + textStart + textWidth, rect.Y + 1, rect.Width - textStart + textWidth - 2, 1)
            );

            Renderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + rect.Height - 1, rect.X + rect.Width - 2, 1));

            Renderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + 1, 1, rect.Height));
            Renderer.DrawFilledRect(new Rectangle(rect.X + rect.Width - 2, rect.Y + 1, 1, rect.Height - 1));

            Renderer.DrawColor = colDarker;

            Renderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y, textStart - 3, 1));
            Renderer.DrawFilledRect(
                new Rectangle(rect.X + 1 + textStart + textWidth, rect.Y, rect.Width - textStart - textWidth - 2, 1)
            );

            Renderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + rect.Height - 1, rect.X + rect.Width - 2, 1));

            Renderer.DrawFilledRect(new Rectangle(rect.X, rect.Y + 1, 1, rect.Height - 1));
            Renderer.DrawFilledRect(new Rectangle(rect.X + rect.Width - 1, rect.Y + 1, 1, rect.Height - 1));
        }

        public override void DrawTextBox(Control.Base control)
        {
            if (control.IsDisabled)
            {
                mTextures.TextBox.Disabled.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (control.HasFocus)
            {
                mTextures.TextBox.Focus.Draw(Renderer, control.RenderBounds, control.RenderColor);
            }
            else
            {
                mTextures.TextBox.Normal.Draw(Renderer, control.RenderBounds, control.RenderColor);
            }
        }

        public override void DrawTabButton(Control.Base control, bool active, Pos dir)
        {
            if (active)
            {
                DrawActiveTabButton(control, dir);

                return;
            }

            if (dir == Pos.Top)
            {
                mTextures.Tab.Top.Inactive.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (dir == Pos.Left)
            {
                mTextures.Tab.Left.Inactive.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (dir == Pos.Bottom)
            {
                mTextures.Tab.Bottom.Inactive.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (dir == Pos.Right)
            {
                mTextures.Tab.Right.Inactive.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }
        }

        private void DrawActiveTabButton(Control.Base control, Pos dir)
        {
            if (dir == Pos.Top)
            {
                mTextures.Tab.Top.Active.Draw(
                    Renderer, control.RenderBounds.Add(new Rectangle(0, 0, 0, 8)), control.RenderColor
                );

                return;
            }

            if (dir == Pos.Left)
            {
                mTextures.Tab.Left.Active.Draw(
                    Renderer, control.RenderBounds.Add(new Rectangle(0, 0, 8, 0)), control.RenderColor
                );

                return;
            }

            if (dir == Pos.Bottom)
            {
                mTextures.Tab.Bottom.Active.Draw(
                    Renderer, control.RenderBounds.Add(new Rectangle(0, -8, 0, 8)), control.RenderColor
                );

                return;
            }

            if (dir == Pos.Right)
            {
                mTextures.Tab.Right.Active.Draw(
                    Renderer, control.RenderBounds.Add(new Rectangle(-8, 0, 8, 0)), control.RenderColor
                );

                return;
            }
        }

        public override void DrawTabControl(Control.Base control)
        {
            mTextures.Tab.Control.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawTabTitleBar(Control.Base control)
        {
            mTextures.Tab.HeaderBar.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawWindow(Control.Base control, int topHeight, bool inFocus)
        {
            GameTexture renderImg = null;
            if (((WindowControl) control).GetImage(Control.WindowControl.ControlState.Active) != null)
            {
                renderImg = ((WindowControl) control).GetImage(Control.WindowControl.ControlState.Active);
            }

            if (((WindowControl) control).GetImage(Control.WindowControl.ControlState.Inactive) != null)
            {
                renderImg = ((WindowControl) control).GetImage(Control.WindowControl.ControlState.Inactive);
            }

            if (renderImg != null)
            {
                Renderer.DrawColor = control.RenderColor;
                Renderer.DrawTexturedRect(renderImg, control.RenderBounds, control.RenderColor);

                return;
            }

            if (inFocus)
            {
                mTextures.Window.Normal.Draw(Renderer, control.RenderBounds, control.RenderColor);
            }
            else
            {
                mTextures.Window.Inactive.Draw(Renderer, control.RenderBounds, control.RenderColor);
            }
        }

        public override void DrawHighlight(Control.Base control)
        {
            var rect = control.RenderBounds;
            Renderer.DrawColor = Color.FromArgb(255, 255, 100, 255);
            Renderer.DrawFilledRect(rect);
        }

        public override void DrawScrollBar(Control.Base control, bool horizontal, bool depressed)
        {
            if (!control.ShouldDrawBackground)
            {
                return;
            }

            if (((ScrollBar) control).GetTemplate() != null)
            {
                var renderImg = ((ScrollBar) control).GetTemplate();
                Renderer.DrawColor = control.RenderColor;

                //Draw Top Left Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Y, 2, 2), control.RenderColor,
                    0, 0, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                );

                //Draw Top
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Y, control.RenderBounds.Width - 4, 2
                    ), control.RenderColor, 2f / renderImg.GetWidth(), 0,
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                );

                //Draw Top Right Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Y, 2, 2),
                    control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 0, 1f,
                    2f / renderImg.GetHeight()
                );

                //Draw Left
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X, control.RenderBounds.Y + 2, 2, control.RenderBounds.Height - 4
                    ), control.RenderColor, 0, 2f / renderImg.GetHeight(), 2f / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                //Draw Middle
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Y + 2, control.RenderBounds.Width - 4,
                        control.RenderBounds.Height - 4
                    ), control.RenderColor, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight(),
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                //Draw Right
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.Width - 2, control.RenderBounds.Y + 2, 2, control.RenderBounds.Height - 4
                    ), control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    2f / renderImg.GetHeight(), 1, (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                // Draw Bottom Left Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Bottom - 2, 2, 2),
                    control.RenderColor, 0, (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                    2f / renderImg.GetWidth(), 1f
                );

                //Draw Bottom
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Bottom - 2, control.RenderBounds.Width - 4, 2
                    ), control.RenderColor, 2f / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 1f
                );

                //Draw Bottom Right Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Bottom - 2, 2, 2),
                    control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight(), 1f, 1f
                );

                return;
            }

            if (horizontal)
            {
                mTextures.Scroller.TrackH.Draw(Renderer, control.RenderBounds, control.RenderColor);
            }
            else
            {
                mTextures.Scroller.TrackV.Draw(Renderer, control.RenderBounds, control.RenderColor);
            }
        }

        public override void DrawScrollBarBar(Control.Base control, bool depressed, bool hovered, bool horizontal)
        {
            GameTexture renderImg = null;
            if (control.IsDisabled && ((ScrollBarBar) control).GetImage(Dragger.ControlState.Disabled) != null)
            {
                renderImg = ((ScrollBarBar) control).GetImage(Dragger.ControlState.Disabled);
            }
            else if (depressed && ((ScrollBarBar) control).GetImage(Dragger.ControlState.Clicked) != null)
            {
                renderImg = ((ScrollBarBar) control).GetImage(Dragger.ControlState.Clicked);
            }
            else if (hovered && ((ScrollBarBar) control).GetImage(Dragger.ControlState.Hovered) != null)
            {
                renderImg = ((ScrollBarBar) control).GetImage(Dragger.ControlState.Hovered);
            }
            else if (((ScrollBarBar) control).GetImage(Dragger.ControlState.Normal) != null)
            {
                renderImg = ((ScrollBarBar) control).GetImage(Dragger.ControlState.Normal);
            }

            if (!horizontal)
            {
                if (renderImg != null)
                {
                    Renderer.DrawColor = control.RenderColor;

                    //Draw Top Left Corner
                    Renderer.DrawTexturedRect(
                        renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Y, 2, 2),
                        control.RenderColor, 0, 0, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                    );

                    //Draw Top
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.X + 2, control.RenderBounds.Y, control.RenderBounds.Width - 4, 2
                        ), control.RenderColor, 2f / renderImg.GetWidth(), 0,
                        (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                    );

                    //Draw Top Right Corner
                    Renderer.DrawTexturedRect(
                        renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Y, 2, 2),
                        control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 0, 1f,
                        2f / renderImg.GetHeight()
                    );

                    //Draw Left
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.X, control.RenderBounds.Y + 2, 2, control.RenderBounds.Height - 4
                        ), control.RenderColor, 0, 2f / renderImg.GetHeight(), 2f / renderImg.GetWidth(),
                        (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                    );

                    //Draw Middle
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.X + 2, control.RenderBounds.Y + 2, control.RenderBounds.Width - 4,
                            control.RenderBounds.Height - 4
                        ), control.RenderColor, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight(),
                        (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                        (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                    );

                    //Draw Right
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.Width - 2, control.RenderBounds.Y + 2, 2,
                            control.RenderBounds.Height - 4
                        ), control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                        2f / renderImg.GetHeight(), 1, (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                    );

                    // Draw Bottom Left Corner
                    Renderer.DrawTexturedRect(
                        renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Bottom - 2, 2, 2),
                        control.RenderColor, 0, (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                        2f / renderImg.GetWidth(), 1f
                    );

                    //Draw Bottom
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.X + 2, control.RenderBounds.Bottom - 2, control.RenderBounds.Width - 4,
                            2
                        ), control.RenderColor, 2f / renderImg.GetWidth(),
                        (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                        (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 1f
                    );

                    //Draw Bottom Right Corner
                    Renderer.DrawTexturedRect(
                        renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Bottom - 2, 2, 2),
                        control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                        (renderImg.GetHeight() - 2f) / renderImg.GetHeight(), 1f, 1f
                    );

                    return;
                }

                if (control.IsDisabled)
                {
                    mTextures.Scroller.ButtonVDisabled.Draw(Renderer, control.RenderBounds, control.RenderColor);

                    return;
                }

                if (depressed)
                {
                    mTextures.Scroller.ButtonVDown.Draw(Renderer, control.RenderBounds, control.RenderColor);

                    return;
                }

                if (hovered)
                {
                    mTextures.Scroller.ButtonVHover.Draw(Renderer, control.RenderBounds, control.RenderColor);

                    return;
                }

                mTextures.Scroller.ButtonVNormal.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (renderImg != null)
            {
                Renderer.DrawColor = control.RenderColor;

                //Draw Top Left Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Y, 2, 2), control.RenderColor,
                    0, 0, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                );

                //Draw Top
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Y, control.RenderBounds.Width - 4, 2
                    ), control.RenderColor, 2f / renderImg.GetWidth(), 0,
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                );

                //Draw Top Right Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Y, 2, 2),
                    control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 0, 1f,
                    2f / renderImg.GetHeight()
                );

                //Draw Left
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X, control.RenderBounds.Y + 2, 2, control.RenderBounds.Height - 4
                    ), control.RenderColor, 0, 2f / renderImg.GetHeight(), 2f / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                //Draw Middle
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Y + 2, control.RenderBounds.Width - 4,
                        control.RenderBounds.Height - 4
                    ), control.RenderColor, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight(),
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                //Draw Right
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.Width - 2, control.RenderBounds.Y + 2, 2, control.RenderBounds.Height - 4
                    ), control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    2f / renderImg.GetHeight(), 1, (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                // Draw Bottom Left Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Bottom - 2, 2, 2),
                    control.RenderColor, 0, (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                    2f / renderImg.GetWidth(), 1f
                );

                //Draw Bottom
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Bottom - 2, control.RenderBounds.Width - 4, 2
                    ), control.RenderColor, 2f / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 1f
                );

                //Draw Bottom Right Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Bottom - 2, 2, 2),
                    control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight(), 1f, 1f
                );

                return;
            }

            if (control.IsDisabled)
            {
                mTextures.Scroller.ButtonHDisabled.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (depressed)
            {
                mTextures.Scroller.ButtonHDown.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (hovered)
            {
                mTextures.Scroller.ButtonHHover.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            mTextures.Scroller.ButtonHNormal.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawProgressBar(Control.Base control, bool horizontal, float progress)
        {
            var rect = control.RenderBounds;

            if (horizontal)
            {
                mTextures.ProgressBar.Back.Draw(Renderer, rect, control.RenderColor);
                rect.Width = (int) (rect.Width * progress);
                mTextures.ProgressBar.Front.Draw(Renderer, rect, control.RenderColor);
            }
            else
            {
                mTextures.ProgressBar.Back.Draw(Renderer, rect, control.RenderColor);
                rect.Y = (int) (rect.Y + rect.Height * (1 - progress));
                rect.Height = (int) (rect.Height * progress);
                mTextures.ProgressBar.Front.Draw(Renderer, rect, control.RenderColor);
            }
        }

        public override void DrawListBox(Control.Base control)
        {
            if (!control.ShouldDrawBackground)
            {
                return;
            }

            mTextures.Input.ListBox.Background.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawListBoxLine(Control.Base control, bool selected, bool even)
        {
            if (!control.ShouldDrawBackground)
            {
                return;
            }

            if (selected)
            {
                if (even)
                {
                    mTextures.Input.ListBox.EvenLineSelected.Draw(Renderer, control.RenderBounds, control.RenderColor);

                    return;
                }

                mTextures.Input.ListBox.OddLineSelected.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (control.IsHovered)
            {
                mTextures.Input.ListBox.Hovered.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (even)
            {
                mTextures.Input.ListBox.EvenLine.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            mTextures.Input.ListBox.OddLine.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public void DrawSliderNotchesH(Rectangle rect, int numNotches, float dist)
        {
            if (numNotches == 0)
            {
                return;
            }

            var iSpacing = rect.Width / (float) numNotches;
            for (var i = 0; i < numNotches + 1; i++)
            {
                Renderer.DrawFilledRect(Util.FloatRect(rect.X + iSpacing * i, rect.Y + dist - 2, 1, 5));
            }
        }

        public void DrawSliderNotchesV(Rectangle rect, int numNotches, float dist)
        {
            if (numNotches == 0)
            {
                return;
            }

            var iSpacing = rect.Height / (float) numNotches;
            for (var i = 0; i < numNotches + 1; i++)
            {
                Renderer.DrawFilledRect(Util.FloatRect(rect.X + dist - 2, rect.Y + iSpacing * i, 5, 1));
            }
        }

        public override void DrawSlider(Control.Base control, bool horizontal, int numNotches, int barSize)
        {
            if (((Slider) control).GetImage() != null)
            {
                var renderImg = ((Slider) control).GetImage();
                var rect = control.RenderBounds;
                Renderer.DrawColor = control.RenderColor;
                if (horizontal)
                {
                    //rect.X += (int) (barSize * 0.5);
                    //rect.Width -= barSize;
                    //rect.Y += (int) (rect.Height * 0.5 - 1);
                    //rect.Height = 1;
                    //DrawSliderNotchesH(rect, numNotches, barSize * 0.5f);
                    //Renderer.DrawFilledRect(rect);
                    Renderer.DrawColor = control.RenderColor;
                    Renderer.DrawTexturedRect(renderImg, rect, control.RenderColor);

                    return;
                }

                //rect.Y += (int) (barSize * 0.5);
                //rect.Height -= barSize;
                //rect.X += (int) (rect.Width * 0.5 - 1);
                //rect.Width = 1;
                //DrawSliderNotchesV(rect, numNotches, barSize * 0.4f);
                //Renderer.DrawFilledRect(rect);
                Renderer.DrawColor = control.RenderColor;
                Renderer.DrawTexturedRect(renderImg, rect, control.RenderColor);
            }
            else
            {
                var rect = control.RenderBounds;
                Renderer.DrawColor = Color.FromArgb(100, 0, 0, 0);

                if (horizontal)
                {
                    rect.X += (int) (barSize * 0.5);
                    rect.Width -= barSize;
                    rect.Y += (int) (rect.Height * 0.5 - 1);
                    rect.Height = 1;
                    DrawSliderNotchesH(rect, numNotches, barSize * 0.5f);
                    Renderer.DrawFilledRect(rect);

                    return;
                }

                rect.Y += (int) (barSize * 0.5);
                rect.Height -= barSize;
                rect.X += (int) (rect.Width * 0.5 - 1);
                rect.Width = 1;
                DrawSliderNotchesV(rect, numNotches, barSize * 0.4f);
                Renderer.DrawFilledRect(rect);
            }
        }

        public override void DrawComboBox(Control.Base control, bool down, bool open)
        {
            if (!control.ShouldDrawBackground)
            {
                return;
            }

            if (control.IsDisabled)
            {
                mTextures.Input.ComboBox.Disabled.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (down || open)
            {
                mTextures.Input.ComboBox.Down.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (control.IsHovered)
            {
                mTextures.Input.ComboBox.Down.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            mTextures.Input.ComboBox.Normal.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawKeyboardHighlight(Control.Base control, Rectangle r, int offset)
        {
            var rect = r;

            rect.X += offset;
            rect.Y += offset;
            rect.Width -= offset * 2;
            rect.Height -= offset * 2;

            //draw the top and bottom
            var skip = true;
            for (var i = 0; i < rect.Width * 0.5; i++)
            {
                mRenderer.DrawColor = Color.Black;
                if (!skip)
                {
                    Renderer.DrawPixel(rect.X + i * 2, rect.Y);
                    Renderer.DrawPixel(rect.X + i * 2, rect.Y + rect.Height - 1);
                }
                else
                {
                    skip = false;
                }
            }

            for (var i = 0; i < rect.Height * 0.5; i++)
            {
                Renderer.DrawColor = Color.Black;
                Renderer.DrawPixel(rect.X, rect.Y + i * 2);
                Renderer.DrawPixel(rect.X + rect.Width - 1, rect.Y + i * 2);
            }
        }

        public override void DrawToolTip(Control.Base control)
        {
            if (control is Label tooltip)
            {
                if (tooltip.ToolTipBackground != null)
                {
                    var renderImg = tooltip.ToolTipBackground;

                    //Draw from custom bg
                    //Draw Top Left Corner
                    Renderer.DrawTexturedRect(
                        renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Y, 8, 8),
                        control.RenderColor, 0, 0, 8f / renderImg.GetWidth(), 8f / renderImg.GetHeight()
                    );

                    //Draw Top
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.X + 8, control.RenderBounds.Y, control.RenderBounds.Width - 16, 8
                        ), control.RenderColor, 8f / renderImg.GetWidth(), 0,
                        (renderImg.GetWidth() - 8f) / renderImg.GetWidth(), 8f / renderImg.GetHeight()
                    );

                    //Draw Top Right Corner
                    Renderer.DrawTexturedRect(
                        renderImg, new Rectangle(control.RenderBounds.Right - 8, control.RenderBounds.Y, 8, 8),
                        control.RenderColor, (renderImg.GetWidth() - 8f) / renderImg.GetWidth(), 0, 1f,
                        8f / renderImg.GetHeight()
                    );

                    //Draw Left
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.X, control.RenderBounds.Y + 8, 8, control.RenderBounds.Height - 16
                        ), control.RenderColor, 0, 8f / renderImg.GetHeight(), 8f / renderImg.GetWidth(),
                        (renderImg.GetHeight() - 8f) / renderImg.GetHeight()
                    );

                    //Draw Middle
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.X + 8, control.RenderBounds.Y + 8, control.RenderBounds.Width - 16,
                            control.RenderBounds.Height - 16
                        ), control.RenderColor, 8f / renderImg.GetWidth(), 8f / renderImg.GetHeight(),
                        (renderImg.GetWidth() - 8f) / renderImg.GetWidth(),
                        (renderImg.GetHeight() - 8f) / renderImg.GetHeight()
                    );

                    //Draw Right
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.Width - 8, control.RenderBounds.Y + 8, 8,
                            control.RenderBounds.Height - 16
                        ), control.RenderColor, (renderImg.GetWidth() - 8f) / renderImg.GetWidth(),
                        8f / renderImg.GetHeight(), 1, (renderImg.GetHeight() - 8f) / renderImg.GetHeight()
                    );

                    // Draw Bottom Left Corner
                    Renderer.DrawTexturedRect(
                        renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Bottom - 8, 8, 8),
                        control.RenderColor, 0, (renderImg.GetHeight() - 8f) / renderImg.GetHeight(),
                        8f / renderImg.GetWidth(), 1f
                    );

                    //Draw Bottom
                    Renderer.DrawTexturedRect(
                        renderImg,
                        new Rectangle(
                            control.RenderBounds.X + 8, control.RenderBounds.Bottom - 8,
                            control.RenderBounds.Width - 16, 8
                        ), control.RenderColor, 8f / renderImg.GetWidth(),
                        (renderImg.GetHeight() - 8f) / renderImg.GetHeight(),
                        (renderImg.GetWidth() - 8f) / renderImg.GetWidth(), 1f
                    );

                    //Draw Bottom Right Corner
                    Renderer.DrawTexturedRect(
                        renderImg, new Rectangle(control.RenderBounds.Right - 8, control.RenderBounds.Bottom - 8, 8, 8),
                        control.RenderColor, (renderImg.GetWidth() - 8f) / renderImg.GetWidth(),
                        (renderImg.GetHeight() - 8f) / renderImg.GetHeight(), 1f, 1f
                    );

                    return;
                }
            }

            mTextures.Tooltip.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawScrollButton(
            Control.Base control,
            Pos direction,
            bool depressed,
            bool hovered,
            bool disabled
        )
        {
            if (!(control is Button button))
            {
                return;
            }

            var i = 0;
            if (direction == Pos.Top)
            {
                i = 1;
            }

            if (direction == Pos.Right)
            {
                i = 2;
            }

            if (direction == Pos.Bottom)
            {
                i = 3;
            }

            GameTexture renderImg = null;

            if (disabled && button.GetImage(Button.ControlState.Disabled) != null)
            {
                renderImg = button.GetImage(Button.ControlState.Disabled);
            }
            else if (depressed && button.GetImage(Button.ControlState.Clicked) != null)
            {
                renderImg = button.GetImage(Button.ControlState.Clicked);
            }
            else if (hovered && button.GetImage(Button.ControlState.Hovered) != null)
            {
                renderImg = button.GetImage(Button.ControlState.Hovered);
            }
            else if (button.GetImage(Button.ControlState.Normal) != null)
            {
                renderImg = button.GetImage(Button.ControlState.Normal);
            }

            if (renderImg != null)
            {
                Renderer.DrawColor = button.RenderColor;
                Renderer.DrawTexturedRect(renderImg, button.RenderBounds, button.RenderColor);

                return;
            }

            if (disabled)
            {
                mTextures.Scroller.Button.Disabled[i].Draw(Renderer, button.RenderBounds, button.RenderColor);

                return;
            }

            if (depressed)
            {
                mTextures.Scroller.Button.Down[i].Draw(Renderer, button.RenderBounds, button.RenderColor);

                return;
            }

            if (hovered)
            {
                mTextures.Scroller.Button.Hover[i].Draw(Renderer, button.RenderBounds, button.RenderColor);

                return;
            }

            mTextures.Scroller.Button.Normal[i].Draw(Renderer, button.RenderBounds, button.RenderColor);
        }

        public override void DrawComboBoxArrow(Control.Base control, bool hovered, bool down, bool open, bool disabled)
        {
            if (disabled)
            {
                mTextures.Input.ComboBox.Button.Disabled.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (down || open)
            {
                mTextures.Input.ComboBox.Button.Down.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (hovered)
            {
                mTextures.Input.ComboBox.Button.Hover.Draw(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            mTextures.Input.ComboBox.Button.Normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawNumericUpDownButton(Control.Base control, bool depressed, bool up)
        {
            if (up)
            {
                if (control.IsDisabled)
                {
                    mTextures.Input.UpDown.Up.Disabled.DrawCenter(Renderer, control.RenderBounds, control.RenderColor);

                    return;
                }

                if (depressed)
                {
                    mTextures.Input.UpDown.Up.Down.DrawCenter(Renderer, control.RenderBounds, control.RenderColor);

                    return;
                }

                if (control.IsHovered)
                {
                    mTextures.Input.UpDown.Up.Hover.DrawCenter(Renderer, control.RenderBounds, control.RenderColor);

                    return;
                }

                mTextures.Input.UpDown.Up.Normal.DrawCenter(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (control.IsDisabled)
            {
                mTextures.Input.UpDown.Down.Disabled.DrawCenter(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (depressed)
            {
                mTextures.Input.UpDown.Down.Down.DrawCenter(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            if (control.IsHovered)
            {
                mTextures.Input.UpDown.Down.Hover.DrawCenter(Renderer, control.RenderBounds, control.RenderColor);

                return;
            }

            mTextures.Input.UpDown.Down.Normal.DrawCenter(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawStatusBar(Control.Base control)
        {
            mTextures.StatusBar.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawTreeButton(Control.Base control, bool open)
        {
            var rect = control.RenderBounds;

            if (open)
            {
                mTextures.Tree.Minus.Draw(Renderer, rect);
            }
            else
            {
                mTextures.Tree.Plus.Draw(Renderer, rect);
            }
        }

        public override void DrawTreeControl(Control.Base control)
        {
            mTextures.Tree.Background.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawTreeNode(
            Control.Base ctrl,
            bool open,
            bool selected,
            int labelHeight,
            int labelWidth,
            int halfWay,
            int lastBranch,
            bool isRoot
        )
        {
            if (selected)
            {
                mTextures.Selection.Draw(
                    Renderer, new Rectangle(17, 0, labelWidth + 2, labelHeight - 1), ctrl.RenderColor
                );
            }

            base.DrawTreeNode(ctrl, open, selected, labelHeight, labelWidth, halfWay, lastBranch, isRoot);
        }

        public override void DrawColorDisplay(Control.Base control, Color color)
        {
            var rect = control.RenderBounds;

            if (color.A != 255)
            {
                Renderer.DrawColor = Color.FromArgb(255, 255, 255, 255);
                Renderer.DrawFilledRect(rect);

                Renderer.DrawColor = Color.FromArgb(128, 128, 128, 128);

                Renderer.DrawFilledRect(Util.FloatRect(0, 0, rect.Width * 0.5f, rect.Height * 0.5f));
                Renderer.DrawFilledRect(
                    Util.FloatRect(rect.Width * 0.5f, rect.Height * 0.5f, rect.Width * 0.5f, rect.Height * 0.5f)
                );
            }

            Renderer.DrawColor = color;
            Renderer.DrawFilledRect(rect);

            Renderer.DrawColor = Color.Black;
            Renderer.DrawLinedRect(rect);
        }

        public override void DrawModalControl(Control.Base control)
        {
            if (!control.ShouldDrawBackground)
            {
                return;
            }

            var rect = control.RenderBounds;
            Renderer.DrawColor = new Color(200, 20, 20, 20);
            Renderer.DrawFilledRect(rect);
        }

        public override void DrawMenuDivider(Control.Base control)
        {
            var rect = control.RenderBounds;
            Renderer.DrawColor = Color.FromArgb(100, 0, 0, 0);
            Renderer.DrawFilledRect(rect);
        }

        public override void DrawWindowCloseButton(Control.Base control, bool depressed, bool hovered, bool disabled)
        {
            if (!(control is Button button))
            {
                return;
            }

            GameTexture renderImg = null;
            if (disabled && button.GetImage(Button.ControlState.Disabled) != null)
            {
                renderImg = button.GetImage(Button.ControlState.Disabled);
            }
            else if (depressed && button.GetImage(Button.ControlState.Clicked) != null)
            {
                renderImg = button.GetImage(Button.ControlState.Clicked);
            }
            else if (hovered && button.GetImage(Button.ControlState.Hovered) != null)
            {
                renderImg = button.GetImage(Button.ControlState.Hovered);
            }
            else if (button.GetImage(Button.ControlState.Normal) != null)
            {
                renderImg = button.GetImage(Button.ControlState.Normal);
            }

            if (renderImg != null)
            {
                Renderer.DrawColor = button.RenderColor;
                Renderer.DrawTexturedRect(renderImg, button.RenderBounds, button.RenderColor);

                return;
            }

            if (disabled)
            {
                mTextures.Window.CloseDisabled.Draw(Renderer, button.RenderBounds);

                return;
            }

            if (depressed)
            {
                mTextures.Window.CloseDown.Draw(Renderer, button.RenderBounds);

                return;
            }

            if (hovered)
            {
                mTextures.Window.CloseHover.Draw(Renderer, button.RenderBounds);

                return;
            }

            mTextures.Window.Close.Draw(Renderer, button.RenderBounds);
        }

        public override void DrawSliderButton(Control.Base control, bool depressed, bool horizontal)
        {
            GameTexture renderImg = null;
            renderImg = ((Dragger) control).GetImage(Dragger.ControlState.Normal);
            if (control.IsDisabled && ((Dragger) control).GetImage(Dragger.ControlState.Disabled) != null)
            {
                renderImg = ((Dragger) control).GetImage(Dragger.ControlState.Disabled);
            }
            else if (depressed && ((Dragger) control).GetImage(Dragger.ControlState.Clicked) != null)
            {
                renderImg = ((Dragger) control).GetImage(Dragger.ControlState.Clicked);
            }
            else if (control.IsHovered && ((Dragger) control).GetImage(Dragger.ControlState.Hovered) != null)
            {
                renderImg = ((Dragger) control).GetImage(Dragger.ControlState.Hovered);
            }

            if (renderImg != null)
            {
                Renderer.DrawColor = control.RenderColor;
                Renderer.DrawTexturedRect(renderImg, control.RenderBounds, control.RenderColor);

                return;
            }

            if (!horizontal)
            {
                if (control.IsDisabled)
                {
                    mTextures.Input.Slider.V.Disabled.DrawCenter(Renderer, control.RenderBounds);

                    return;
                }

                if (depressed)
                {
                    mTextures.Input.Slider.V.Down.DrawCenter(Renderer, control.RenderBounds);

                    return;
                }

                if (control.IsHovered)
                {
                    mTextures.Input.Slider.V.Hover.DrawCenter(Renderer, control.RenderBounds);

                    return;
                }

                mTextures.Input.Slider.V.Normal.DrawCenter(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsDisabled)
            {
                mTextures.Input.Slider.H.Disabled.DrawCenter(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                mTextures.Input.Slider.H.Down.DrawCenter(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsHovered)
            {
                mTextures.Input.Slider.H.Hover.DrawCenter(Renderer, control.RenderBounds);

                return;
            }

            mTextures.Input.Slider.H.Normal.DrawCenter(Renderer, control.RenderBounds);
        }

        public override void DrawCategoryHolder(Control.Base control)
        {
            mTextures.CategoryList.Outer.Draw(Renderer, control.RenderBounds, control.RenderColor);
        }

        public override void DrawCategoryInner(Control.Base control, bool collapsed)
        {
            if (collapsed)
            {
                mTextures.CategoryList.Header.Draw(Renderer, control.RenderBounds, control.RenderColor);
            }
            else
            {
                mTextures.CategoryList.Inner.Draw(Renderer, control.RenderBounds, control.RenderColor);
            }
        }

        public override void DrawLabel(Control.Base control)
        {
            if (((Label) control).GetTemplate() != null)
            {
                var renderImg = ((Label) control).GetTemplate();
                Renderer.DrawColor = control.RenderColor;

                //Draw Top Left Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Y, 2, 2), control.RenderColor,
                    0, 0, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                );

                //Draw Top
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Y, control.RenderBounds.Width - 4, 2
                    ), control.RenderColor, 2f / renderImg.GetWidth(), 0,
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 2f / renderImg.GetHeight()
                );

                //Draw Top Right Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Y, 2, 2),
                    control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 0, 1f,
                    2f / renderImg.GetHeight()
                );

                //Draw Left
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X, control.RenderBounds.Y + 2, 2, control.RenderBounds.Height - 4
                    ), control.RenderColor, 0, 2f / renderImg.GetHeight(), 2f / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                //Draw Middle
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Y + 2, control.RenderBounds.Width - 4,
                        control.RenderBounds.Height - 4
                    ), control.RenderColor, 2f / renderImg.GetWidth(), 2f / renderImg.GetHeight(),
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                //Draw Right
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.Width - 2, control.RenderBounds.Y + 2, 2, control.RenderBounds.Height - 4
                    ), control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    2f / renderImg.GetHeight(), 1, (renderImg.GetHeight() - 2f) / renderImg.GetHeight()
                );

                // Draw Bottom Left Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.X, control.RenderBounds.Bottom - 2, 2, 2),
                    control.RenderColor, 0, (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                    2f / renderImg.GetWidth(), 1f
                );

                //Draw Bottom
                Renderer.DrawTexturedRect(
                    renderImg,
                    new Rectangle(
                        control.RenderBounds.X + 2, control.RenderBounds.Bottom - 2, control.RenderBounds.Width - 4, 2
                    ), control.RenderColor, 2f / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight(),
                    (renderImg.GetWidth() - 2f) / renderImg.GetWidth(), 1f
                );

                //Draw Bottom Right Corner
                Renderer.DrawTexturedRect(
                    renderImg, new Rectangle(control.RenderBounds.Right - 2, control.RenderBounds.Bottom - 2, 2, 2),
                    control.RenderColor, (renderImg.GetWidth() - 2f) / renderImg.GetWidth(),
                    (renderImg.GetHeight() - 2f) / renderImg.GetHeight(), 1f, 1f
                );

                return;
            }
        }

        #endregion

    }

}
