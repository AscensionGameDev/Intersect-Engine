using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Color = IntersectClientExtras.GenericClasses.Color;
using Intersect_Library.GameObjects;
using IntersectClientExtras.Gwen.ControlInternal;
using Intersect_Client.Classes.Networking;
using Intersect_Library.Localization;

namespace Intersect_Client.Classes.UI.Game
{
    public class QuestOfferWindow
    {
        //Controls
        private WindowControl _questOfferWindow;
        private Label _questTitle;
        private ListBox _questOffer;
        private string _questOfferText = "";
        private Button _acceptButton;
        private Button _declineButton;

        public QuestOfferWindow(Canvas _gameCanvas)
        {
            _questOfferWindow = new WindowControl(_gameCanvas, Strings.Get("questoffer","title"));
            _questOfferWindow.SetSize(415, 424);
            _questOfferWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - 200, GameGraphics.Renderer.GetScreenHeight() / 2 - 200);
            _questOfferWindow.DisableResizing();
            _questOfferWindow.Margin = Margin.Zero;
            _questOfferWindow.Padding = new Padding(8, 5, 9, 11);
            _questOfferWindow.IsClosable = false;

            _questOfferWindow.SetTitleBarHeight(24);
            _questOfferWindow.SetCloseButtonSize(20, 20);
            _questOfferWindow.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "questwindow.png"), WindowControl.ControlState.Active);
            _questOfferWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"), Button.ControlState.Normal);
            _questOfferWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"), Button.ControlState.Hovered);
            _questOfferWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"), Button.ControlState.Clicked);
            _questOfferWindow.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont, 14));
            _questOfferWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            //Menu Header
            _questTitle = new Label(_questOfferWindow)
            {
                AutoSizeToContents = false
            };
            _questTitle.SetText("");
            _questTitle.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 18);
            _questTitle.SetSize(_questOfferWindow.Width, _questOfferWindow.Height);
            _questTitle.Alignment = Pos.CenterH;
            _questTitle.TextColorOverride = new Color(255, 200, 200, 200);

            _questOffer = new ListBox(_questOfferWindow)
            {
                IsDisabled = true
            };
            _questOffer.SetPosition(8 + _questOfferWindow.Padding.Left, 32 + _questOfferWindow.Padding.Top);
            _questOffer.SetSize(372, 260);
            _questOffer.ShouldDrawBackground = false;
            _questOffer.RenderColor = Color.White;

            var scrollBar = _questOffer.GetVerticalScrollBar();
            scrollBar.RenderColor = new Color(200, 40, 40, 40);
            scrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"), Dragger.ControlState.Normal);
            scrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"), Dragger.ControlState.Hovered);
            scrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"), Dragger.ControlState.Clicked);

            var upButton = scrollBar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"), Button.ControlState.Normal);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"), Button.ControlState.Clicked);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"), Button.ControlState.Hovered);
            var downButton = scrollBar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"), Button.ControlState.Normal);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"), Button.ControlState.Clicked);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"), Button.ControlState.Hovered);

            //Accept Button
            _acceptButton = new Button(_questOfferWindow);
            _acceptButton.SetText(Strings.Get("questoffer", "accept"));
            _acceptButton.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);
            _acceptButton.Clicked += _acceptButton_Clicked;
            _acceptButton.SetPosition(_questOfferWindow.Width /2 - _acceptButton.Width/2 - _acceptButton.Width, 340);
            _acceptButton.SetSize(86, 39);
            _acceptButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonnormal.png"), Button.ControlState.Normal);
            _acceptButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonhover.png"), Button.ControlState.Hovered);
            _acceptButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonclicked.png"), Button.ControlState.Clicked);
            _acceptButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _acceptButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _acceptButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _acceptButton.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);

            //Decline Button
            _declineButton = new Button(_questOfferWindow);
            _declineButton.SetText(Strings.Get("questoffer", "decline"));
            _declineButton.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);
            _declineButton.Clicked += _declineButton_Clicked;
            _declineButton.SetPosition(_questOfferWindow.Width / 2 + _acceptButton.Width/2, 340);
            _declineButton.SetSize(86, 39);
            _declineButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonnormal.png"), Button.ControlState.Normal);
            _declineButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonhover.png"), Button.ControlState.Hovered);
            _declineButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonclicked.png"), Button.ControlState.Clicked);
            _declineButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _declineButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _declineButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _declineButton.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);



            Gui.InputBlockingElements.Add(_questOfferWindow);
        }

        private void _declineButton_Clicked(Base sender, IntersectClientExtras.Gwen.Control.EventArguments.ClickedEventArgs arguments)
        {
            if (Globals.QuestOffers.Count > 0)
            {
                PacketSender.SendDeclineQuest(Globals.QuestOffers[0]);
                Globals.QuestOffers.RemoveAt(0);
            }
        }

        private void _acceptButton_Clicked(Base sender, IntersectClientExtras.Gwen.Control.EventArguments.ClickedEventArgs arguments)
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
                    var myText = Gui.WrapText(quest.StartDesc, _questOffer.Width - 12, _questOfferWindow.Parent.Skin.DefaultFont);
                    foreach (var t in myText)
                    {
                        var rw = _questOffer.AddRow(t);
                        rw.SetTextColor(Color.White);
                        rw.MouseInputEnabled = false;
                    }
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
