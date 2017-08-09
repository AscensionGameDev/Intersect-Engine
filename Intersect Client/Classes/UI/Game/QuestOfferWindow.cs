using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class QuestOfferWindow
    {
        private Button _acceptButton;
        private Button _declineButton;

        private string _questOfferText = "";

        //Controls
        private WindowControl _questOfferWindow;

        private ScrollControl _questPromptArea;
        private RichLabel _questPromptLabel;
        private Label _questPromptTemplate;
        private Label _questTitle;

        public QuestOfferWindow(Canvas _gameCanvas)
        {
            _questOfferWindow = new WindowControl(_gameCanvas, Strings.Get("questoffer", "title"), false,
                "QuestOfferWindow");
            _questOfferWindow.DisableResizing();
            _questOfferWindow.IsClosable = false;

            //Menu Header
            _questTitle = new Label(_questOfferWindow, "QuestTitle");

            _questPromptArea = new ScrollControl(_questOfferWindow, "QuestOfferArea");

            _questPromptTemplate = new Label(_questPromptArea, "QuestOfferTemplate");

            _questPromptLabel = new RichLabel(_questPromptArea);

            //Accept Button
            _acceptButton = new Button(_questOfferWindow, "AcceptButton");
            _acceptButton.SetText(Strings.Get("questoffer", "accept"));
            _acceptButton.Clicked += _acceptButton_Clicked;

            //Decline Button
            _declineButton = new Button(_questOfferWindow, "DeclineButton");
            _declineButton.SetText(Strings.Get("questoffer", "decline"));
            _declineButton.Clicked += _declineButton_Clicked;

            Gui.LoadRootUIData(_questOfferWindow, "InGame.xml");
            Gui.InputBlockingElements.Add(_questOfferWindow);
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
                _questTitle.Text = quest.Name;
                if (_questOfferText != quest.StartDesc)
                {
                    _questPromptLabel.DeleteAllChildren();
                    _questPromptLabel.Width = _questPromptArea.Width -
                                              _questPromptArea.GetVerticalScrollBar().Width;
                    _questPromptLabel.AddText(quest.StartDesc, _questPromptTemplate.TextColor,
                        _questPromptTemplate.CurAlignments.Count > 0
                            ? _questPromptTemplate.CurAlignments[0]
                            : Alignments.Left, _questPromptTemplate.Font);
                    _questPromptLabel.SizeToChildren(false, true);
                    _questOfferText = quest.StartDesc;
                }
            }
        }

        public void Show()
        {
            _questOfferWindow.IsHidden = false;
        }

        public void Close()
        {
            _questOfferWindow.Close();
        }

        public bool IsVisible()
        {
            return !_questOfferWindow.IsHidden;
        }

        public void Hide()
        {
            _questOfferWindow.IsHidden = true;
        }
    }
}