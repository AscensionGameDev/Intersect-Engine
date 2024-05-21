using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game
{

    public partial class QuestOfferWindow
    {

        private Button mAcceptButton;

        private Button mDeclineButton;

        private string mQuestOfferText = "";

        //Controls
        private WindowControl mQuestOfferWindow;

        private ScrollControl mQuestPromptArea;

        private RichLabel mQuestPromptLabel;

        private Label mQuestPromptTemplate;

        private Label mQuestTitle;

        public QuestOfferWindow(Canvas gameCanvas)
        {
            mQuestOfferWindow = new WindowControl(gameCanvas, Strings.QuestOffer.Title, false, "QuestOfferWindow");
            mQuestOfferWindow.DisableResizing();
            mQuestOfferWindow.IsClosable = false;

            //Menu Header
            mQuestTitle = new Label(mQuestOfferWindow, "QuestTitle");

            mQuestPromptArea = new ScrollControl(mQuestOfferWindow, "QuestOfferArea");

            mQuestPromptTemplate = new Label(mQuestPromptArea, "QuestOfferTemplate");

            mQuestPromptLabel = new RichLabel(mQuestPromptArea);

            //Accept Button
            mAcceptButton = new Button(mQuestOfferWindow, "AcceptButton");
            mAcceptButton.SetText(Strings.QuestOffer.Accept);
            mAcceptButton.Clicked += _acceptButton_Clicked;

            //Decline Button
            mDeclineButton = new Button(mQuestOfferWindow, "DeclineButton");
            mDeclineButton.SetText(Strings.QuestOffer.Decline);
            mDeclineButton.Clicked += _declineButton_Clicked;

            mQuestOfferWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            Interface.InputBlockingElements.Add(mQuestOfferWindow);
        }

        private void _declineButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.QuestOffers.Count > 0)
            {
                PacketSender.SendDeclineQuest(Globals.QuestOffers[0]);
                Globals.QuestOffers.RemoveAt(0);
            }
        }

        private void _acceptButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.QuestOffers.Count > 0)
            {
                PacketSender.SendAcceptQuest(Globals.QuestOffers[0]);
                Globals.QuestOffers.RemoveAt(0);
            }
        }

        public void Update(QuestBase quest)
        {
            if (quest == null)
            {
                Hide();
            }
            else
            {
                Show();
                mQuestTitle.Text = quest.Name;
                if (mQuestOfferText != quest.StartDescription)
                {
                    mQuestPromptLabel.ClearText();
                    mQuestPromptLabel.Width = mQuestPromptArea.Width - mQuestPromptArea.GetVerticalScrollBar().Width;
                    mQuestPromptLabel.AddText(quest.StartDescription, mQuestPromptTemplate);

                    mQuestPromptLabel.SizeToChildren(false, true);
                    mQuestOfferText = quest.StartDescription;
                }
            }
        }

        public void Show()
        {
            mQuestOfferWindow.IsHidden = false;
        }

        public void Close()
        {
            mQuestOfferWindow.Close();
        }

        public bool IsVisible()
        {
            return !mQuestOfferWindow.IsHidden;
        }

        public void Hide()
        {
            mQuestOfferWindow.IsHidden = true;
        }

    }

}
