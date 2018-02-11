using Intersect.Client.Classes.UI.Menu;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;

namespace Intersect_Client.Classes.UI.Menu
{
    public class CreditsWindow
    {
        private Button mBackBtn;

        //Content
        private ScrollControl mCreditsContent;
        private RichLabel mRichLabel;

        //Parent
        private Label mCreditsHeader;

        //Controls
        private ImagePanel mCreditsWindow;

        private MainMenu mMainMenu;

        //Init
        public CreditsWindow(Canvas parent, MainMenu mainMenu)
        {
            //Assign References
            mMainMenu = mainMenu;

            //Main Menu Window
            mCreditsWindow = new ImagePanel(parent, "CreditsWindow");

            //Menu Header
            mCreditsHeader = new Label(mCreditsWindow, "CreditsHeader");
            mCreditsHeader.SetText(Strings.Get("credits", "title"));

            mCreditsContent = new ScrollControl(mCreditsWindow, "CreditsScrollview");
            mCreditsContent.EnableScroll(false, true);

            var creditsParser = new CreditsParser();
            mRichLabel = new RichLabel(mCreditsContent, "CreditsLabel");
            foreach (var line in creditsParser.Credits)
            {
                if (line.Text.Trim().Length == 0)
                {
                    mRichLabel.AddLineBreak();
                }
                else
                {
                    mRichLabel.AddText(line.Text, new Color(line.Clr.A, line.Clr.R, line.Clr.G, line.Clr.B),
                        line.Alignment, GameContentManager.Current.GetFont(line.Font, line.Size));
                    mRichLabel.AddLineBreak();
                }
            }

            //Back Button
            mBackBtn = new Button(mCreditsWindow, "BackButton");
            mBackBtn.SetText(Strings.Get("credits", "back"));
            mBackBtn.Clicked += BackBtn_Clicked;
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
            mRichLabel.Width = mCreditsContent.Width;
            mRichLabel.SizeToChildren(false, true);
        }
    }
}