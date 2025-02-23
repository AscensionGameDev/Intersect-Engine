using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Newtonsoft.Json;

namespace Intersect.Client.Interface.Menu;

public partial class CreditsWindow : Window, IMainMenuWindow
{
    private readonly IFont? _defaultFont;

    private readonly MainMenu _mainMenu;
    private readonly RichLabel _credits;
    private readonly ScrollControl _creditsScroller;

    public CreditsWindow(Canvas parent, MainMenu mainMenu) : base(
        parent,
        title: Strings.Credits.Title,
        modal: false,
        name: nameof(CreditsWindow)
    )
    {
        _mainMenu = mainMenu;

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 640, y: 400);
        IsResizable = false;
        IsClosable = true;

        Titlebar.MouseInputEnabled = false;
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        _defaultFont = GameContentManager.Current.GetFont(name: TitleLabel.FontName);

        _creditsScroller = new ScrollControl(this, name: nameof(_creditsScroller))
        {
            Dock = Pos.Fill,
        };
        _creditsScroller.EnableScroll(false, true);

        _credits = new RichLabel(_creditsScroller, nameof(_credits))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
            FontSize = 12,
            Padding = new Padding(16),
        };

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
    }

    protected override void OnClose(Base control, EventArgs args)
    {
        base.OnClose(control, args);

        _mainMenu.Show();
    }

    protected override void EnsureInitialized()
    {
        if (_credits.FormattedLabels.Count > 0)
        {
            return;
        }

        _credits.ClearText();
        var credits = new Credits();
        var creditsFile = Path.Combine(ClientConfiguration.ResourcesDirectory, "credits.json");

        if (File.Exists(creditsFile))
        {
            credits = JsonConvert.DeserializeObject<Credits>(File.ReadAllText(creditsFile));
        }
        else
        {
            var line = new Credits.CreditsLine
            {
                Text = "Insert your credits here!",
                Alignment = "center",
                Size = 12,
                TextColor = Color.White,
                Font = "sourcesansproblack",
            };

            credits.Lines.Add(line);
        }

        File.WriteAllText(creditsFile, JsonConvert.SerializeObject(credits, Formatting.Indented));

        foreach (var line in credits?.Lines ?? [])
        {
            var lineText = line.Text.Trim();
            if (lineText.Length > 0)
            {
                var lineFont = GameContentManager.Current.GetFont(line.Font);
                _credits.AddText(
                    text: lineText,
                    color: new Color(
                        line.TextColor?.A ?? 255,
                        line.TextColor?.R ?? 255,
                        line.TextColor?.G ?? 255,
                        line.TextColor?.B ?? 255
                    ),
                    alignment: line.GetAlignment(),
                    font: lineFont,
                    fontSize: line.Size
                );
            }

            _credits.AddLineBreak();
        }

        _ = _credits.SizeToChildren(false, true);
    }
}