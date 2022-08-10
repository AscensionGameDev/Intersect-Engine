using ImGuiNET;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Platform;
using Intersect.Client.Framework.UserInterface;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows.MainWindow;

internal partial class EditorMainWindow : EditorWindow
{
    private const string DOCK_NAME_STATUS_BAR = "dock_status_bar";

    private readonly ImGuiRenderer _imGuiRenderer;
    private readonly ImFontPtr _customFont;
    private readonly Texture _xnaTexture;

    public EditorMainWindow(
        IContentManager contentManager,
        PlatformWindow platformWindow,
        ImGuiRenderer imGuiRenderer,
        ImFontPtr customFont
    ) : base(contentManager, platformWindow)
    {
        _imGuiRenderer = imGuiRenderer;
        _customFont = customFont;

        _xnaTexture = CreateTexture(300, 150, pixel =>
        {
            var red = (pixel % 300) / 2;
            return new(red, 1, 1);
        });

        var manualComponent = new ManualComponent(_customFont, _imGuiRenderer.BindTexture(_xnaTexture));
        //Components.Add(manualComponent);
    }

    public Texture CreateTexture(int width, int height, Func<int, Intersect.Graphics.Color> paint)
    {
        //initialize a texture
        var texture = GraphicsDevice.CreateTexture(width, height, false);

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

    public IEnumerable<TWindowType> FindWindows<TWindowType>()
        where TWindowType : Window =>
        Components.OfType<TWindowType>();
}


internal class ManualComponent : Component
{
    private IntPtr _imGuiTexture;
    private readonly ImFontPtr _customFont;

    // Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
    private float f = 0.0f;

    private bool show_test_window = false;
    private bool show_another_window = false;
    private System.Numerics.Vector3 clear_color = new System.Numerics.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
    private byte[] _textBuffer = new byte[100];

    private int counter = 0;

    public ManualComponent(ImFontPtr customFont, IntPtr texture)
    {
        _customFont = customFont;
        // Texture loading example
        // Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
        _imGuiTexture = texture;
    }

    protected override bool LayoutBegin(FrameTime frameTime)
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
            ImGui.Begin("Demo Window");
            ImGui.Text("Hello, world!");
            ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
            ImGui.ColorEdit3("clear color", ref clear_color);
            if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
            if (ImGui.Button("Another Window")) show_another_window = !show_another_window;
            ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

            ImGui.InputText("Text input", _textBuffer, 100);

            ImGui.Text("Texture sample");
            ImGui.Image(_imGuiTexture, new System.Numerics.Vector2(300, 150), System.Numerics.Vector2.Zero, System.Numerics.Vector2.One, System.Numerics.Vector4.One, System.Numerics.Vector4.One); // Here, the previously loaded texture is used
            ImGui.End();
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
        return true;
    }

    protected override void LayoutEnd(FrameTime frameTime)
    {
    }
}

