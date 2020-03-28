using System.IO;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;

using Newtonsoft.Json;

namespace Intersect.Client.Interface.Menu
{

    public class CreditsWindow
    {

        private Button mBackBtn;

        //Content
        private ScrollControl mCreditsContent;

        //Parent
        private Label mCreditsHeader;

        //Controls
        private ImagePanel mCreditsWindow;

        private MainMenu mMainMenu;

        private RichLabel mRichLabel;

        //Init
        public CreditsWindow(Canvas parent, MainMenu mainMenu)
        {
            //Assign References
            mMainMenu = mainMenu;

            //Main Menu Window
            mCreditsWindow = new ImagePanel(parent, "CreditsWindow");

            //Menu Header
            mCreditsHeader = new Label(mCreditsWindow, "CreditsHeader");
            mCreditsHeader.SetText(Strings.Credits.title);

            mCreditsContent = new ScrollControl(mCreditsWindow, "CreditsScrollview");
            mCreditsContent.EnableScroll(false, true);

            mRichLabel = new RichLabel(mCreditsContent, "CreditsLabel");

            //Back Button
            mBackBtn = new Button(mCreditsWindow, "BackButton");
            mBackBtn.SetText(Strings.Credits.back);
            mBackBtn.Clicked += BackBtn_Clicked;

            mCreditsWindow.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());
        }

        private void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mMainMenu.Show();
        }

        //Methods
        public void Update()
        {
        }

        public void Hide()
        {
            mCreditsWindow.IsHidden = true;
        }

        public void Show()
        {
            mCreditsWindow.IsHidden = false;
            mRichLabel.ClearText();
            var credits = new Credits();
            var creditsFile = Path.Combine("resources", "credits.json");
            if (File.Exists(creditsFile))
            {
                credits = JsonConvert.DeserializeObject<Credits>(File.ReadAllText(creditsFile));
            }
            else
            {
                var line = new Credits.CreditsLine();
                line.Text = "Insert your credits here!";
                line.Alignment = "center";
                line.Size = 12;
                line.Clr = Intersect.Color.White;
                line.Font = "sourcesansproblack";
                credits.Lines.Add(line);
            }

            File.WriteAllText(creditsFile, JsonConvert.SerializeObject(credits, Formatting.Indented));

            foreach (var line in credits.Lines)
            {
                if (line.Text.Trim().Length == 0)
                {
                    mRichLabel.AddLineBreak();
                }
                else
                {
                    mRichLabel.AddText(
                        line.Text, new Color(line.Clr.A, line.Clr.R, line.Clr.G, line.Clr.B), line.GetAlignment(),
                        GameContentManager.Current.GetFont(line.Font, line.Size)
                    );

                    mRichLabel.AddLineBreak();
                }
            }

            mRichLabel.SizeToChildren(false, true);
        }

    }

}
