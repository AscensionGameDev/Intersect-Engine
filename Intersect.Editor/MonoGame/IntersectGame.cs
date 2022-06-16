using System.Diagnostics;
using System.Reflection;

using ImGuiNET;

using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Gwen.Renderer;
using Intersect.Client.Framework.Input;
using Intersect.Client.Framework.UserInterface;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Configuration;
using Intersect.Editor.Core;
using Intersect.Editor.Core.Controls;
using Intersect.Editor.Fonts;
using Intersect.Editor.General;
using Intersect.Editor.Interface.Game;
using Intersect.Editor.Localization;
using Intersect.Editor.MonoGame.Database;
using Intersect.Editor.MonoGame.File_Management;
using Intersect.Editor.MonoGame.Graphics;
using Intersect.Editor.MonoGame.Input;
using Intersect.Editor.MonoGame.Network;
using Intersect.Editor.MonoGame.UserInterface;
using Intersect.Editor.Platform;
using Intersect.Time;
using Intersect.Updater;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Intersect.Client.Framework.Graphics;

using MainMenu = Intersect.Editor.Interface.Menu.MainMenu;
using Texture = Intersect.Client.Framework.Graphics.Texture;
using Window = Intersect.Client.Framework.UserInterface.Components.Window;
using Menu = Intersect.Client.Framework.UserInterface.Components.Menu;

namespace Intersect.Editor.MonoGame
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    internal class IntersectGame : Game
    {
        private bool mInitialized;

        private double mLastUpdateTime = 0;

        private GraphicsDeviceManager mGraphics;

        #region "Autoupdate Variables"

        private Updater.Updater mUpdater;

        private Texture2D updaterBackground;

        private SpriteFont updaterFont;

        private SpriteFont updaterFontSmall;

        private Texture2D updaterProgressBar;

        private SpriteBatch updateBatch;

        private bool updaterGraphicsReset;

        #endregion

        private IClientContext Context { get; }

        private Action PostStartupAction { get; }

        private ImGuiRenderer _imGuiRenderer;

        private Client.Framework.Graphics.Texture _xnaTexture;
        private IntPtr _imGuiTexture;
        private IntPtr _guiTexture;

        private ImFontPtr _defaultFont;
        private ImFontPtr _customFont;

        private Client.Framework.Graphics.GraphicsDevice _graphicsDevice;

        private IntersectGame(IClientContext context, Action postStartupAction)
        {
            Context = context;
            PostStartupAction = postStartupAction;

            Strings.Load();

            mGraphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 480,
                PreferHalfPixelOffset = true
            };

            mGraphics.PreparingDeviceSettings += (s, args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage =
                    RenderTargetUsage.PreserveContents;
            };

            Content.RootDirectory = "";
            IsMouseVisible = true;
            Globals.ContentManager = new MonoContentManager();
            Globals.Database = new MonoDatabase();

            /* Load configuration */
            ClientConfiguration.LoadAndSave(ClientConfiguration.DefaultPath);

            Globals.Database.LoadPreferences();

            Window.IsBorderless = Context.StartupOptions.BorderlessWindow;
            Window.AllowUserResizing = true;

            var renderer = new MonoRenderer(mGraphics, Content, this)
            {
                OverrideResolution = Context.StartupOptions.ScreenResolution
            };

            Globals.InputManager = new MonoInput(this);
            GameClipboard.Instance = new MonoClipboard();

            Core.Graphics.Renderer = renderer;

            Interface.Interface.GwenRenderer = new IntersectRenderer(null, Core.Graphics.Renderer);
            Interface.Interface.GwenInput = new IntersectInput();
            Controls.Init();

            Window.Position = new Microsoft.Xna.Framework.Point(-20, -2000);
            Window.AllowAltF4 = false;

            // If we're going to be rendering a custom mouse cursor, hide the default one!
            if (!string.IsNullOrWhiteSpace(ClientConfiguration.Instance.MouseCursor))
            {
                IsMouseVisible = false;
            }

            if (!string.IsNullOrWhiteSpace(ClientConfiguration.Instance.UpdateUrl))
            {
                mUpdater = new Updater.Updater(
                    ClientConfiguration.Instance.UpdateUrl, Path.Combine("version.json"), true, 5
                );
            }
        }

        private static unsafe void BuildGlyphRanges(ImFontGlyphRangesBuilderPtr builder, params GlyphRange[] glyphRanges) => BuildGlyphRanges(builder, glyphRanges as IEnumerable<GlyphRange>);

        private static unsafe void BuildGlyphRanges(ImFontGlyphRangesBuilderPtr builder, IEnumerable<GlyphRange> glyphRanges)
        {
            foreach (var glyphRange in glyphRanges)
            {
                for (ushort glyph = glyphRange.Start; glyph <= glyphRange.End; glyph++)
                {
                    builder.AddChar(glyph);
                }
            }
        }

        private static unsafe ImFontPtr BuildFont(float size)
        {
            var io = ImGui.GetIO();

            var fontsDirectory = Path.Combine(Environment.CurrentDirectory, "resources", "fonts");

            var latinRangeBuilder = new ImFontGlyphRangesBuilderPtr(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());

            var ranges = GlyphRange.ComputeRanges("AaÆæÅåǺǻḀḁẚĂăẶặẮắẰằẲẳẴẵȂȃÂâẬậẤấẦầẪẫẨẩẢảǍǎȺⱥȦȧǠǡẠạÄäǞǟÀàȀȁÁáĀāĀ̀ā̀ÃãĄąĄ́ą́Ą̃ą̃A̲a̲ᶏ" +
                "BbɃƀḂḃḄḅḆḇƁɓᵬᶀ" +
                "CcĆćĈĉČčĊċḈḉƇƈC̈c̈ȻȼÇçꟄꞔꞒꞓ©" +
                "DdĐđꟇꟈƊɗḊḋḌḍḐḑḒḓĎďḎḏᵭᶁᶑ" +
                "EeĔĕḜḝȆȇÊêÊ̄ê̄Ê̌ê̌ỀềẾếỂểỄễỆệẺẻḘḙĚěɆɇĖėĖ́ė́Ė̃ė̃ẸẹËëÈèÈ̩è̩ȄȅÉéÉ̩ĒēḔḕḖḗẼẽḚḛĘęĘ́ę́Ę̃ę̃ȨȩE̩e̩ᶒ" +
                "IiỊịĬĭÎîǏǐƗɨÏïḮḯÍíÌìȈȉĮįĮ́Į̃ĪīĪ̀ī̀ᶖỈỉȊȋĨĩḬḭᶤ" +
                "LlĹĺŁłĽľḸḹL̃l̃ĻļĿŀḶḷḺḻḼḽȽƚⱠⱡ" +
                "NnŃńÑñŇňǸǹṄṅṆṇŅņṈṉṊṋꞤꞥᵰᶇ" +
                "OoŒœØøǾǿᶱÖöȪȫÓóÒòÔôỐốỒồỔổỖỗỘộǑǒŐőŎŏȎȏȮȯȰȱỌọƟɵƠơỚớỜờỠỡỢợỞởỎỏŌōṒṓṐṑÕõȬȭṌṍṎṏǪǫȌȍO̩o̩Ó̩ó̩Ò̩ò̩ǬǭO͍o͍" +
                "SsßŚśṠṡẛṨṩṤṥṢṣS̩s̩ꞨꞩꟉꟊŜŝṦṧŠšŞşȘșS̈s̈ᶊⱾȿᵴᶳ" +
                "UuŬŭɄʉᵾᶶꞸꞹỤụÜüǛǜǗǘǙǚǕǖṲṳÚúÙùÛûṶṷǓǔȖȗŰűŬŭƯưỨứỪừỬửỰựỮỮỦủŪūŪ̀ū̀Ū́ū́ṺṻŪ̃ū̃ŨũṸṹṴṵᶙŲųŲ́ų́Ų̃ų̃ȔȕŮů" +
                "YyÝýỲỳŶŷŸÿỸỹẎẏỴỵẙỶỷȲȳɎɏƳƴ" +
                "ZzŹźẐẑŽžŻżẒẓẔẕƵƶᵶꟆᶎⱫⱬ" +
                "FfGgHhJjKkMmPpQqRrTtVvWwXx", new GlyphRange(0x20, 0x7f));

            BuildGlyphRanges(latinRangeBuilder, ranges);

            //latinRangeBuilder.AddText(
            //    "AaÆæÅåǺǻḀḁẚĂăẶặẮắẰằẲẳẴẵȂȃÂâẬậẤấẦầẪẫẨẩẢảǍǎȺⱥȦȧǠǡẠạÄäǞǟÀàȀȁÁáĀāĀ̀ā̀ÃãĄąĄ́ą́Ą̃ą̃A̲a̲ᶏ" +
            //    "BbɃƀḂḃḄḅḆḇƁɓᵬᶀ" +
            //    "CcĆćĈĉČčĊċḈḉƇƈC̈c̈ȻȼÇçꟄꞔꞒꞓ©" +
            //    "DdĐđꟇꟈƊɗḊḋḌḍḐḑḒḓĎďḎḏᵭᶁᶑ" +
            //    "EeĔĕḜḝȆȇÊêÊ̄ê̄Ê̌ê̌ỀềẾếỂểỄễỆệẺẻḘḙĚěɆɇĖėĖ́ė́Ė̃ė̃ẸẹËëÈèÈ̩è̩ȄȅÉéÉ̩ĒēḔḕḖḗẼẽḚḛĘęĘ́ę́Ę̃ę̃ȨȩE̩e̩ᶒ" +
            //    "IiỊịĬĭÎîǏǐƗɨÏïḮḯÍíÌìȈȉĮįĮ́Į̃ĪīĪ̀ī̀ᶖỈỉȊȋĨĩḬḭᶤ" +
            //    "LlĹĺŁłĽľḸḹL̃l̃ĻļĿŀḶḷḺḻḼḽȽƚⱠⱡ" +
            //    "NnŃńÑñŇňǸǹṄṅṆṇŅņṈṉṊṋꞤꞥᵰᶇ" +
            //    "OoŒœØøǾǿᶱÖöȪȫÓóÒòÔôỐốỒồỔổỖỗỘộǑǒŐőŎŏȎȏȮȯȰȱỌọƟɵƠơỚớỜờỠỡỢợỞởỎỏŌōṒṓṐṑÕõȬȭṌṍṎṏǪǫȌȍO̩o̩Ó̩ó̩Ò̩ò̩ǬǭO͍o͍" +
            //    "SsßŚśṠṡẛṨṩṤṥṢṣS̩s̩ꞨꞩꟉꟊŜŝṦṧŠšŞşȘșS̈s̈ᶊⱾȿᵴᶳ" +
            //    "UuŬŭɄʉᵾᶶꞸꞹỤụÜüǛǜǗǘǙǚǕǖṲṳÚúÙùÛûṶṷǓǔȖȗŰűŬŭƯưỨứỪừỬửỰựỮỮỦủŪūŪ̀ū̀Ū́ū́ṺṻŪ̃ū̃ŨũṸṹṴṵᶙŲųŲ́ų́Ų̃ų̃ȔȕŮů" +
            //    "YyÝýỲỳŶŷŸÿỸỹẎẏỴỵẙỶỷȲȳɎɏƳƴ" +
            //    "ZzŹźẐẑŽžŻżẒẓẔẕƵƶᵶꟆᶎⱫⱬ" +
            //    "FfGgHhJjKkMmPpQqRrTtVvWwXx"
            //);

            //latinRangeBuilder.AddRanges(io.Fonts.GetGlyphRangesDefault());

            latinRangeBuilder.BuildRanges(out ImVector latin);

            var font = io.Fonts.AddFontFromFileTTF(Path.Combine(fontsDirectory, "MesloLGS NF Regular.ttf"), size, default, latin.Data);

            ImFontConfigPtr fontConfig = ImGuiNative.ImFontConfig_ImFontConfig();
            fontConfig.MergeMode = true;
            fontConfig.PixelSnapH = true;

            var cyrillicRangeBuilder = new ImFontGlyphRangesBuilderPtr(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());

            BuildGlyphRanges(
                cyrillicRangeBuilder,
                new GlyphRange(0x0400, 0x04ff)
            );

            cyrillicRangeBuilder.BuildRanges(out ImVector cyrillic);

            _ = io.Fonts.AddFontFromFileTTF(Path.Combine(fontsDirectory, "Andika-Regular.ttf"), size, fontConfig, cyrillic.Data);

            var nerdFontsBuilder = new ImFontGlyphRangesBuilderPtr(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
            nerdFontsBuilder.AddChar('');
            nerdFontsBuilder.AddChar('');
            nerdFontsBuilder.AddChar('');
            nerdFontsBuilder.AddChar('');
            nerdFontsBuilder.AddChar('');
            nerdFontsBuilder.AddChar('');
            nerdFontsBuilder.BuildRanges(out ImVector symbols);

            _ = io.Fonts.AddFontFromFileTTF(Path.Combine(fontsDirectory, "MesloLGS NF Regular.ttf"), size, fontConfig, symbols.Data);

            return font;
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected unsafe override void Initialize()
        {
            var libraryHandle = RuntimeHelper.LoadNativeLibrary("cimgui");

            _graphicsDevice = new MonoGameGraphicsDevice(GraphicsDevice);

            //var configPath = Path.Combine(Environment.CurrentDirectory, $"{Process.GetCurrentProcess()?.ProcessName}.ini");
            ////ImGui.LoadIniSettingsFromDisk(configPath);
            ////ImGui.SaveIniSettingsToDisk(configPath);

            //var io = ImGui.GetIO();
            //var pathBytes = Encoding.UTF8.GetBytes(configPath + '\0');
            //fixed (byte* pathPtr = pathBytes)
            //{
            //    var actualIO = *io.NativePtr;
            //    actualIO.IniFilename = pathPtr;
            //}

            _imGuiRenderer = new MonoGameImGuiRenderer(this);
            _customFont = BuildFont(20);
            //_defaultFont = io.FontDefault;
            _imGuiRenderer.RebuildFontAtlas();

            base.Initialize();

            if (mUpdater != null)
            {
                LoadUpdaterContent();

                //Set the size of the updater screen before applying graphic changes.
                //We need to do this here instead of in the constructor for the size change to apply to Linux
                mGraphics.PreferredBackBufferWidth = 800;
                mGraphics.PreferredBackBufferHeight = 480;
            }

            mGraphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            // Texture loading example

            // First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
            _xnaTexture = CreateTexture(300, 150, pixel =>
            {
                var red = (pixel % 300) / 2;
                return new(red, 1, 1);
            });

            // Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
            _imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);

            _canvas = new Canvas("root");

            var exitMenuItem = new MenuItem
            {
                Name = "Exit",
                Shortcut = "x"
            };
            exitMenuItem.Selected += (s, e) => Exit();

            _canvas.MenuBar.Menus.Add(new Menu("File")
            {
                Items = new()
                {
                    exitMenuItem
                }
            });
            //_canvas.Children.Add(new ManualComponent(_customFont));

            var mainMenuWindow = new Window("main_menu");
            _canvas.Children.Add(mainMenuWindow);

            base.LoadContent();
        }

        public Texture CreateTexture(int width, int height, Func<int, Intersect.Graphics.Color> paint)
        {
            //initialize a texture
            var texture = _graphicsDevice.CreateTexture(width, height, false);

            //the array holds the color for each pixel in the texture
            var data = new Intersect.Graphics.Color[width * height];
            for (var pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);

            return texture;
        }

        // Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
        private float f = 0.0f;

        private bool show_test_window = false;
        private bool show_another_window = false;
        private System.Numerics.Vector3 clear_color = new System.Numerics.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
        private byte[] _textBuffer = new byte[100];

        private int counter = 0;

        private Canvas _canvas;

        private class ManualComponent : Component
        {
            private readonly ImFontPtr _customFont;

            private int counter = 0;

            public ManualComponent(ImFontPtr customFont)
            {
                _customFont = customFont;
            }

            protected override bool DrawBegin()
            {
                return true;
            }

            protected override void DrawBehindChildren(FrameTime frameTime)
            {
                ImGui.PushFont(_customFont);
                ImGui.SetCursorPos(new System.Numerics.Vector2(100, 10));
                var canvasHovered = ImGui.IsItemHovered(ImGuiHoveredFlags.ChildWindows);
                ImGui.Text("testing canvas: " + canvasHovered);
                ImGui.Text("AaÆæÅåǺǻḀḁẚĂăẶặẮắẰằẲẳẴẵȂȃÂâẬậẤấẦầẪẫẨẩẢảǍǎȺⱥȦȧǠǡẠạÄäǞǟÀàȀȁÁáĀāĀ̀ā̀ÃãĄąĄ́ą́Ą̃ą̃A̲a̲ᶏ");
                ImGui.Text("BbɃƀḂḃḄḅḆḇƁɓᵬᶀ");
                ImGui.Text("CcĆćĈĉČčĊċḈḉƇƈC̈c̈ȻȼÇçꟄꞔꞒꞓ©");
                ImGui.Text("DdĐđꟇꟈƊɗḊḋḌḍḐḑḒḓĎďḎḏᵭᶁᶑ");
                ImGui.Text("EeĔĕḜḝȆȇÊêÊ̄ê̄Ê̌ê̌ỀềẾếỂểỄễỆệẺẻḘḙĚěɆɇĖėĖ́ė́Ė̃ė̃ẸẹËëÈèÈ̩è̩ȄȅÉéÉ̩ĒēḔḕḖḗẼẽḚḛĘęĘ́ę́Ę̃ę̃ȨȩE̩e̩ᶒ");
                ImGui.Text("IiỊịĬĭÎîǏǐƗɨÏïḮḯÍíÌìȈȉĮįĮ́Į̃ĪīĪ̀ī̀ᶖỈỉȊȋĨĩḬḭᶤ");
                ImGui.Text("LlĹĺŁłĽľḸḹL̃l̃ĻļĿŀḶḷḺḻḼḽȽƚⱠⱡ");
                ImGui.Text("NnŃńÑñŇňǸǹṄṅṆṇŅņṈṉṊṋꞤꞥᵰᶇ");
                ImGui.Text("OoŒœØøǾǿᶱÖöȪȫÓóÒòÔôỐốỒồỔổỖỗỘộǑǒŐőŎŏȎȏȮȯȰȱỌọƟɵƠơỚớỜờỠỡỢợỞởỎỏŌōṒṓṐṑÕõȬȭṌṍṎṏǪǫȌȍO̩o̩Ó̩ó̩Ò̩ò̩ǬǭO͍o͍");
                ImGui.Text("SsßŚśṠṡẛṨṩṤṥṢṣS̩s̩ꞨꞩꟉꟊŜŝṦṧŠšŞşȘșS̈s̈ᶊⱾȿᵴᶳ");
                ImGui.Text("UuŬŭɄʉᵾᶶꞸꞹỤụÜüǛǜǗǘǙǚǕǖṲṳÚúÙùÛûṶṷǓǔȖȗŰűŬŭƯưỨứỪừỬửỰựỮỮỦủŪūŪ̀ū̀Ū́ū́ṺṻŪ̃ū̃ŨũṸṹṴṵᶙŲųŲ́ų́Ų̃ų̃ȔȕŮů");
                ImGui.Text("YyÝýỲỳŶŷŸÿỸỹẎẏỴỵẙỶỷȲȳɎɏƳƴ");
                ImGui.Text("ZzŹźẐẑŽžŻżẒẓẔẕƵƶᵶꟆᶎⱫⱬ");
                ImGui.Text("FfGgHhJjKkMmPpQqRrTtVvWwXx\n    /mnt/c/Users/Me/git/romkatv/nerd-fonts    \nЛорем ипсум долор сит амет\nΛορεμ ιπσθμ δολορ σιτ αμετ");
                ImGui.PopFont();
                ImGui.Begin("test");
                ImGui.Button($"test {counter}");
                if (ImGui.IsItemHovered())
                {
                    ++counter;
                }
                //ImGui.SetWindowPos(new System.Numerics.Vector2(50, 400));
                ImGui.Text("Test1234");
                ImGui.End();
                ImGui.ShowMetricsWindow();
            }

            protected override void DrawEnd()
            {
            }
        }

        protected virtual void ImGuiLayout()
        {

            //{
            //    ImGui.Begin("canvas", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoInputs);
            //    ImGui.SetWindowPos(new System.Numerics.Vector2(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y));
            //    ImGui.SetWindowSize(new System.Numerics.Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            //    ImGui.PushFont(_customFont);
            //    ImGui.SetCursorPos(new System.Numerics.Vector2(100, 10));
            //    var canvasHovered = ImGui.IsItemHovered(ImGuiHoveredFlags.ChildWindows);
            //    ImGui.Text("testing canvas: " + canvasHovered);
            //    ImGui.Text("AaÆæÅåǺǻḀḁẚĂăẶặẮắẰằẲẳẴẵȂȃÂâẬậẤấẦầẪẫẨẩẢảǍǎȺⱥȦȧǠǡẠạÄäǞǟÀàȀȁÁáĀāĀ̀ā̀ÃãĄąĄ́ą́Ą̃ą̃A̲a̲ᶏ");
            //    ImGui.Text("BbɃƀḂḃḄḅḆḇƁɓᵬᶀ");
            //    ImGui.Text("CcĆćĈĉČčĊċḈḉƇƈC̈c̈ȻȼÇçꟄꞔꞒꞓ©");
            //    ImGui.Text("DdĐđꟇꟈƊɗḊḋḌḍḐḑḒḓĎďḎḏᵭᶁᶑ");
            //    ImGui.Text("EeĔĕḜḝȆȇÊêÊ̄ê̄Ê̌ê̌ỀềẾếỂểỄễỆệẺẻḘḙĚěɆɇĖėĖ́ė́Ė̃ė̃ẸẹËëÈèÈ̩è̩ȄȅÉéÉ̩ĒēḔḕḖḗẼẽḚḛĘęĘ́ę́Ę̃ę̃ȨȩE̩e̩ᶒ");
            //    ImGui.Text("IiỊịĬĭÎîǏǐƗɨÏïḮḯÍíÌìȈȉĮįĮ́Į̃ĪīĪ̀ī̀ᶖỈỉȊȋĨĩḬḭᶤ");
            //    ImGui.Text("LlĹĺŁłĽľḸḹL̃l̃ĻļĿŀḶḷḺḻḼḽȽƚⱠⱡ");
            //    ImGui.Text("NnŃńÑñŇňǸǹṄṅṆṇŅņṈṉṊṋꞤꞥᵰᶇ");
            //    ImGui.Text("OoŒœØøǾǿᶱÖöȪȫÓóÒòÔôỐốỒồỔổỖỗỘộǑǒŐőŎŏȎȏȮȯȰȱỌọƟɵƠơỚớỜờỠỡỢợỞởỎỏŌōṒṓṐṑÕõȬȭṌṍṎṏǪǫȌȍO̩o̩Ó̩ó̩Ò̩ò̩ǬǭO͍o͍");
            //    ImGui.Text("SsßŚśṠṡẛṨṩṤṥṢṣS̩s̩ꞨꞩꟉꟊŜŝṦṧŠšŞşȘșS̈s̈ᶊⱾȿᵴᶳ");
            //    ImGui.Text("UuŬŭɄʉᵾᶶꞸꞹỤụÜüǛǜǗǘǙǚǕǖṲṳÚúÙùÛûṶṷǓǔȖȗŰűŬŭƯưỨứỪừỬửỰựỮỮỦủŪūŪ̀ū̀Ū́ū́ṺṻŪ̃ū̃ŨũṸṹṴṵᶙŲųŲ́ų́Ų̃ų̃ȔȕŮů");
            //    ImGui.Text("YyÝýỲỳŶŷŸÿỸỹẎẏỴỵẙỶỷȲȳɎɏƳƴ");
            //    ImGui.Text("ZzŹźẐẑŽžŻżẒẓẔẕƵƶᵶꟆᶎⱫⱬ");
            //    ImGui.Text("FfGgHhJjKkMmPpQqRrTtVvWwXx\n    /mnt/c/Users/Me/git/romkatv/nerd-fonts    \nЛорем ипсум долор сит амет\nΛορεμ ιπσθμ δολορ σιτ αμετ");
            //    ImGui.PopFont();
            //    ImGui.Begin("test");
            //    ImGui.Button("test " + counter);
            //    if (ImGui.IsItemHovered())
            //    {
            //        ++counter;
            //    }
            //    //ImGui.SetWindowPos(new System.Numerics.Vector2(50, 400));
            //    ImGui.Text("Test1234");
            //    ImGui.End();
            //    ImGui.ShowMetricsWindow();
            //    ImGui.End();
            //}

            ImGui.PushFont(_customFont);

            // 1. Show a simple window
            // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
            {
                ImGui.Text("Hello, world!");
                ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
                ImGui.ColorEdit3("clear color", ref clear_color);
                if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
                if (ImGui.Button("Another Window")) show_another_window = !show_another_window;
                ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

                ImGui.InputText("Text input", _textBuffer, 100);

                ImGui.Text("Texture sample");
                ImGui.Image(_imGuiTexture, new System.Numerics.Vector2(300, 150), System.Numerics.Vector2.Zero, System.Numerics.Vector2.One, System.Numerics.Vector4.One, System.Numerics.Vector4.One); // Here, the previously loaded texture is used
            }

            // 2. Show another simple window, this time using an explicit Begin/End pair
            if (show_another_window)
            {
                ImGui.SetNextWindowSize(new System.Numerics.Vector2(200, 100), ImGuiCond.FirstUseEver);
                ImGui.Begin("Another Window", ref show_another_window);
                ImGui.Text("Hello");
                ImGui.End();
            }

            ImGui.PopFont();

            // 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
            if (show_test_window)
            {
                ImGui.SetNextWindowPos(new System.Numerics.Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref show_test_window);
            }
        }

        private void IntersectInit()
        {
            (Core.Graphics.Renderer as MonoRenderer)?.Init(GraphicsDevice);

            // TODO: Remove old netcode
            Networking.Network.Socket = new MonoSocket(Context);
            Networking.Network.Socket.Connected += (sender, connectionEventArgs) =>
                MainMenu.SetNetworkStatus(connectionEventArgs.NetworkStatus);

            Networking.Network.Socket.ConnectionFailed += (sender, connectionEventArgs, denied) =>
                MainMenu.SetNetworkStatus(connectionEventArgs.NetworkStatus);

            Networking.Network.Socket.Disconnected += (sender, connectionEventArgs) =>
                MainMenu.SetNetworkStatus(connectionEventArgs.NetworkStatus);

            Main.Start(Context);

            mInitialized = true;

            PostStartupAction();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (mUpdater != null)
            {
                if (mUpdater.CheckUpdaterContentLoaded())
                {
                    LoadUpdaterContent();
                }

                if (mUpdater.Status == UpdateStatus.Done || mUpdater.Status == UpdateStatus.None)
                {
                    if (updaterGraphicsReset == true)
                    {
                        //Drew a frame, now let's initialize the engine
                        IntersectInit();
                        mUpdater = null;
                    }
                }
                else if (mUpdater.Status == UpdateStatus.Restart)
                {
                    //Auto relaunch on Windows
                    switch (Environment.OSVersion.Platform)
                    {
                        case PlatformID.Win32NT:
                        case PlatformID.Win32S:
                        case PlatformID.Win32Windows:
                        case PlatformID.WinCE:
                            Process.Start(
                                Environment.GetCommandLineArgs()[0],
                                Environment.GetCommandLineArgs().Length > 1
                                    ? string.Join(" ", Environment.GetCommandLineArgs().Skip(1))
                                    : null
                            );

                            Exit();
                            break;
                    }
                }
            }

            if (mUpdater == null)
            {
                if (!mInitialized)
                {
                    IntersectInit();
                }

                if (Globals.IsRunning)
                {
                    if (mLastUpdateTime < gameTime.TotalGameTime.TotalMilliseconds)
                    {
                        lock (Globals.GameLock)
                        {
                            Main.Update(gameTime.ElapsedGameTime);
                        }

                        ///mLastUpdateTime = gameTime.TotalGameTime.TotalMilliseconds + (1000/60f);
                    }
                }
                else
                {
                    Main.DestroyGame();
                    Exit();
                }
            }

            var frameTime = gameTime.FromMonoGame();

            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(frameTime);

            _canvas.Bounds = new(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            _canvas.Draw(frameTime);

            // Draw our UI
            ImGuiLayout();

            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            if (mUpdater != null)
            {
                if (mUpdater.Status == UpdateStatus.Done || mUpdater.Status == UpdateStatus.None)
                {
                    if (updaterGraphicsReset == false)
                    {
                        if (Core.Graphics.Renderer is MonoRenderer monoRenderer)
                        {
                            monoRenderer.Init(GraphicsDevice);
                            monoRenderer.Init();
                            _ = monoRenderer.Begin();
                            monoRenderer.End();
                        }
                        updaterGraphicsReset = true;
                    }
                }
                else
                {
                    DrawUpdater();
                }
            }
            else
            {
                if (Globals.IsRunning && mInitialized)
                {
                    lock (Globals.GameLock)
                    {
                        Core.Graphics.Render(gameTime.ElapsedGameTime);
                    }
                }
            }

            _imGuiRenderer.Draw();

            base.Draw(gameTime);
        }

        private void ExitToDesktop(object sender, EventArgs e)
        {
            if (Globals.Me != null)
            {
                Globals.Me.CombatTimer = 0;
            }

            Globals.IsRunning = false;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (Globals.Me != null && Globals.Me.CombatTimer > Timing.Global?.Milliseconds)
            {
                //Try to prevent SDL Window Close
                var exception = false;
                try
                {
                    var platform = GetType()
                        .GetField("Platform", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(this);

                    var field = platform.GetType()
                        .GetField("_isExiting", BindingFlags.NonPublic | BindingFlags.Instance);

                    field.SetValue(platform, 0);
                }
                catch
                {
                    //TODO: Should we log here? I really don't know if it's necessary.
                    exception = true;
                }

                if (!exception)
                {
                    //Show Message Getting Exit Confirmation From Player to Leave in Combat
                    var box = new InputBox(
                        Strings.Combat.warningtitle, Strings.Combat.warningcharacterselect, true,
                        InputBox.InputType.YesNo, ExitToDesktop, null, null
                    );

                    //Restart the MonoGame RunLoop
                    Run();
                    return;
                }
            }

            try
            {
                mUpdater?.Stop();
            }
            catch
            {
            }

            //Just close if we don't need to show a combat warning
            base.OnExiting(sender, args);
            Networking.Network.Close("quitting");
            Dispose();
        }

        private void DrawUpdater()
        {
            //Draw updating text and show progress bar...

            if (updateBatch == null)
            {
                updateBatch = new SpriteBatch(GraphicsDevice);
            }

            updateBatch.Begin(SpriteSortMode.Immediate);

            //Default Window Size is 800x480
            if (updaterBackground != null)
            {
                updateBatch.Draw(
                    updaterBackground, new Rectangle(0, 0, 800, 480),
                    new Rectangle?(new Rectangle(0, 0, updaterBackground.Width, updaterBackground.Height)),
                    Microsoft.Xna.Framework.Color.White
                );
            }

            var status = "";
            var progressPercent = 0f;
            var progress = "";
            var filesRemaining = "";
            var sizeRemaining = "";

            switch (mUpdater.Status)
            {
                case UpdateStatus.Checking:
                    status = Strings.Update.Checking;
                    break;

                case UpdateStatus.Updating:
                    status = Strings.Update.Updating;
                    progressPercent = mUpdater.Progress / 100f;
                    progress = Strings.Update.Percent.ToString((int) mUpdater.Progress);
                    filesRemaining = mUpdater.FilesRemaining + " Files Remaining";
                    sizeRemaining = mUpdater.GetHumanReadableFileSize(mUpdater.SizeRemaining) + " Left";
                    break;

                case UpdateStatus.Restart:
                    status = Strings.Update.Restart.ToString(Strings.Main.gamename);
                    progressPercent = 100;
                    progress = Strings.Update.Percent.ToString(100);
                    break;

                case UpdateStatus.Done:
                    status = Strings.Update.Done;
                    progressPercent = 100;
                    progress = Strings.Update.Percent.ToString(100);
                    break;

                case UpdateStatus.Error:
                    status = Strings.Update.Error;
                    progress = mUpdater.Exception?.Message ?? "";
                    progressPercent = 100;
                    break;

                case UpdateStatus.None:
                    //Nothing here!
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (updaterFont != null)
            {
                var size = updaterFont.MeasureString(status);
                updateBatch.DrawString(
                    updaterFont, status, new Vector2(800 / 2 - size.X / 2, 360), Microsoft.Xna.Framework.Color.White
                );
            }

            //Bar will exist at 400 to 432
            if (updaterProgressBar != null)
            {
                updateBatch.Draw(
                    updaterProgressBar, new Rectangle(100, 400, (int) (600 * progressPercent), 32),
                    new Rectangle?(
                        new Rectangle(
                            0, 0, (int) (updaterProgressBar.Width * progressPercent), updaterProgressBar.Height
                        )
                    ), Microsoft.Xna.Framework.Color.White
                );
            }

            //Bar will be 600 pixels wide
            if (updaterFontSmall != null)
            {
                //Draw % in center of bar
                var size = updaterFontSmall.MeasureString(progress);
                updateBatch.DrawString(
                    updaterFontSmall, progress, new Vector2(800 / 2 - size.X / 2, 405),
                    Microsoft.Xna.Framework.Color.White
                );

                //Draw files remaining on bottom left
                updateBatch.DrawString(
                    updaterFontSmall, filesRemaining, new Vector2(100, 440), Microsoft.Xna.Framework.Color.White
                );

                //Draw total remaining on bottom right
                size = updaterFontSmall.MeasureString(sizeRemaining);
                updateBatch.DrawString(
                    updaterFontSmall, sizeRemaining, new Vector2(700 - size.X, 440), Microsoft.Xna.Framework.Color.White
                );
            }

            updateBatch.End();
        }

        private void LoadUpdaterContent()
        {
            if (File.Exists(Path.Combine("resources", "updater", "background.png")))
            {
                updaterBackground = Texture2D.FromFile(
                    GraphicsDevice, Path.Combine("resources", "updater", "background.png")
                );
            }

            if (File.Exists(Path.Combine("resources", "updater", "progressbar.png")))
            {
                updaterProgressBar = Texture2D.FromFile(
                    GraphicsDevice, Path.Combine("resources", "updater", "progressbar.png")
                );
            }

            if (File.Exists(Path.Combine("resources", "updater", "font.xnb")))
            {
                updaterFont = Content.Load<SpriteFont>(Path.Combine("resources", "updater", "font"));
            }

            if (File.Exists(Path.Combine("resources", "updater", "fontsmall.xnb")))
            {
                updaterFontSmall = Content.Load<SpriteFont>(Path.Combine("resources", "updater", "fontsmall"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
            {
                return;
            }

            if (!Context.IsDisposed)
            {
                Context.Dispose();
            }
        }

        /// <summary>
        /// Implements <see cref="IPlatformRunner"/> for MonoGame.
        /// </summary>
        internal class MonoGameRunner : IPlatformRunner
        {
            /// <inheritdoc />
            public void Start(IClientContext context, Action postStartupAction)
            {
                using (var game = new IntersectGame(context, postStartupAction))
                {
                    game.Run();
                }
            }
        }
    }
}
