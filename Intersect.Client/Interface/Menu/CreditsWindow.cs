using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Newtonsoft.Json;

namespace Intersect.Client.Interface.Menu;

public partial class CreditsWindow : ImagePanel, IMainMenuWindow
{
    private readonly MainMenu _mainMenu;
    private readonly RichLabel _richLabel;

    public CreditsWindow(Canvas parent, MainMenu mainMenu) : base(parent, "CreditsWindow")
    {
        _mainMenu = mainMenu;

        _ = new Label(this, "CreditsHeader")
        {
            Text = Strings.Credits.Title,
        };

        var creditsContent = new ScrollControl(this, "CreditsScrollview");
        creditsContent.EnableScroll(false, true);

        _richLabel = new RichLabel(creditsContent, "CreditsLabel");

        var btnBack = new Button(this, "BackButton")
        {
            Text = Strings.Credits.Back,
        };
        btnBack.Clicked += BackBtn_Clicked;

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
    }

    private void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
    {
        Hide();
        _mainMenu.Show();
    }

    public override void Show()
    {
        base.Show();
        _richLabel.ClearText();
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
            if (line.Text.Trim().Length == 0)
            {
                _richLabel.AddLineBreak();
            }
            else
            {
                _richLabel.AddText(
                    line.Text,
                    new Color(
                        line.TextColor?.A ?? 255,
                        line.TextColor?.R ?? 255,
                        line.TextColor?.G ?? 255,
                        line.TextColor?.B ?? 255
                    ),
                    line.GetAlignment(),
                    GameContentManager.Current.GetFont(line.Font, line.Size)
                );

                _richLabel.AddLineBreak();
            }
        }

        _ = _richLabel.SizeToChildren(false, true);
    }
}