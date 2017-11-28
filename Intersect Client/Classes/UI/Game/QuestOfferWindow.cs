using Intersect.GameObjects;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class QuestOfferWindow
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
            mQuestOfferWindow = new WindowControl(gameCanvas, Strings.QuestOffer.title, false,
                "QuestOfferWindow");
            mQuestOfferWindow.DisableResizing();
            mQuestOfferWindow.IsClosable = false;

            //Menu Header
            mQuestTitle = new Label(mQuestOfferWindow, "QuestTitle");

            mQuestPromptArea = new ScrollControl(mQuestOfferWindow, "QuestOfferArea");

            mQuestPromptTemplate = new Label(mQuestPromptArea, "QuestOfferTemplate");

            mQuestPromptLabel = new RichLabel(mQuestPromptArea);

            //Accept Button
            mAcceptButton = new Button(mQuestOfferWindow, "AcceptButton");
            mAcceptButton.SetText(Strings.QuestOffer.title,
            mAcceptButton.Clicked += _acceptButton_Clicked;

            //Decline Button
            mDeclineButton = new Button(mQuestOfferWindow, "DeclineButton");
            mDeclineButton.SetText(trings.QuestOffer.decline);
            mDeclineButton.Clicked += _declineButton_Clicked;

            Gui.LoadRootUiData(mQuestOfferWindow, "InGame.xml");
            Gui.InputBlockingElements.Add(mQuestOfferWindow);
        }

        private void _declineButton_Clicked(Base sender,
            IntersectClientExtras.Gwen.Control.EventArguments.ClickedEventArgs arguments)
        {
            if (Globals.QuestOffers.Count > 0)
            {
                PacketSender.SendDeclineQuest(Globals.QuestOffers[0]);
                Globals.QuestOffers.RemoveAt(0);
            }
        }

        private void _acceptButton_Clicked(Base sender,
            IntersectClientExtras.Gwen.Control.EventArguments.ClickedEventArgs arguments)
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
                if (mQuestOfferText != quest.StartDesc)
                {
                    mQuestPromptLabel.ClearText();
                    mQuestPromptLabel.Width = mQuestPromptArea.Width -
                                              mQuestPromptArea.GetVerticalScrollBar().Width;
                    mQuestPromptLabel.AddText(quest.StartDesc, mQuestPromptTemplate.TextColor,
                        mQuestPromptTemplate.CurAlignments.Count > 0
                            ? mQuestPromptTemplate.CurAlignments[0]
                            : Alignments.Left, mQuestPromptTemplate.Font);
                    mQuestPromptLabel.SizeToChildren(false, true);
                    mQuestOfferText = quest.StartDesc;
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