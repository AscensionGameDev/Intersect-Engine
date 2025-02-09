using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Newtonsoft.Json;

namespace Intersect.Client.Interface.Menu;

public partial class CreditsWindow : Window, IMainMenuWindow
{
    private readonly GameFont? _defaultFont;

    private readonly MainMenu _mainMenu;
    private readonly RichLabel _credits;
    private readonly Button _backButton;
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
        IsClosable = false;

        Titlebar.MouseInputEnabled = false;

        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        _defaultFont = GameContentManager.Current.GetFont(name: TitleLabel.FontName, 12);

        _creditsScroller = new ScrollControl(this, nameof(_creditsScroller))
        {
            Dock = Pos.Fill,
        };
        _creditsScroller.EnableScroll(false, true);

        _credits = new RichLabel(_creditsScroller, nameof(_credits))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
            Padding = new Padding(16),
        };

        _backButton = new Button(this, nameof(_backButton))
        {
            Alignment = [Alignments.CenterH],
            AutoSizeToContents = true,
            Dock = Pos.Bottom | Pos.CenterH,
            Font = _defaultFont,
            Margin = new Margin(0, 8),
            Padding = new Padding(8, 4),
            Text = Strings.Credits.Back,
        };
        _backButton.Clicked += BackBtn_Clicked;

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
    }

    private void BackBtn_Clicked(Base sender, MouseButtonState arguments)
    {
        Hide();
        _mainMenu.Show();
    }

    protected override void RecurseLayout(Framework.Gwen.Skin.Base skin)
    {
        base.RecurseLayout(skin);
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
                var lineFont = GameContentManager.Current.GetFont(line.Font, line.Size);
                _credits.AddText(
                    lineText,
                    new Color(
                        line.TextColor?.A ?? 255,
                        line.TextColor?.R ?? 255,
                        line.TextColor?.G ?? 255,
                        line.TextColor?.B ?? 255
                    ),
                    line.GetAlignment(),
                    lineFont
                );
            }

            _credits.AddLineBreak();
        }

        _ = _credits.SizeToChildren(false, true);
    }
}