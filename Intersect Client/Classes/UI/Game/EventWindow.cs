using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class EventWindow
    {
        private ScrollControl _eventDialogArea;
        private ScrollControl _eventDialogAreaNoFace;
        private RichLabel _eventDialogLabel;
        private RichLabel _eventDialogLabelNoFace;
        private Label _eventDialogLabelNoFaceTemplate;

        private Label _eventDialogLabelTemplate;

        //Window Controls
        private ImagePanel _eventDialogWindow;

        private ImagePanel _eventFace;
        private Button _eventResponse1;
        private Button _eventResponse2;
        private Button _eventResponse3;
        private Button _eventResponse4;

        //Init
        public EventWindow(Canvas _gameCanvas)
        {
            //Event Dialog Window
            _eventDialogWindow = new ImagePanel(_gameCanvas, "EventDialogueWindow");
            _eventDialogWindow.Hide();
            Gui.InputBlockingElements.Add(_eventDialogWindow);

            _eventFace = new ImagePanel(_eventDialogWindow, "EventFacePanel");

            _eventDialogArea = new ScrollControl(_eventDialogWindow, "EventDialogArea");
            _eventDialogLabelTemplate = new Label(_eventDialogArea, "EventDialogLabel");
            _eventDialogLabel = new RichLabel(_eventDialogArea);

            _eventDialogAreaNoFace = new ScrollControl(_eventDialogWindow, "EventDialogAreaNoFace");
            _eventDialogLabelNoFaceTemplate = new Label(_eventDialogAreaNoFace, "EventDialogLabel");
            _eventDialogLabelNoFace = new RichLabel(_eventDialogAreaNoFace);

            _eventResponse1 = new Button(_eventDialogWindow, "EventResponse1");
            _eventResponse1.Clicked += EventResponse1_Clicked;

            _eventResponse2 = new Button(_eventDialogWindow, "EventResponse2");
            _eventResponse2.Clicked += EventResponse2_Clicked;

            _eventResponse3 = new Button(_eventDialogWindow, "EventResponse3");
            _eventResponse3.Clicked += EventResponse3_Clicked;

            _eventResponse4 = new Button(_eventDialogWindow, "EventResponse4");
            _eventResponse4.Clicked += EventResponse4_Clicked;
        }

        //Update
        public void Update()
        {
            if (Globals.EventDialogs.Count > 0)
            {
                if (_eventDialogWindow.IsHidden)
                {
                    _eventDialogWindow.Show();
                    _eventDialogWindow.MakeModal();
                    _eventDialogArea.ScrollToTop();
                    GameTexture faceTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face,
                        Globals.EventDialogs[0].Face);

                    int responseCount = 0;
                    int maxResponse = 1;
                    if (Globals.EventDialogs[0].Opt1.Length > 0) responseCount++;
                    if (Globals.EventDialogs[0].Opt2.Length > 0)
                    {
                        responseCount++;
                        maxResponse = 2;
                    }
                    if (Globals.EventDialogs[0].Opt3.Length > 0)
                    {
                        responseCount++;
                        maxResponse = 3;
                    }
                    if (Globals.EventDialogs[0].Opt4.Length > 0)
                    {
                        responseCount++;
                        maxResponse = 4;
                    }

                    _eventResponse1.Name = "";
                    _eventResponse2.Name = "";
                    _eventResponse3.Name = "";
                    _eventResponse4.Name = "";
                    switch (maxResponse)
                    {
                        case 1:
                            _eventDialogWindow.Name = "EventDialogWindow_Max1Response";
                            _eventResponse1.Name = "Response1Button";
                            break;
                        case 2:
                            _eventDialogWindow.Name = "EventDialogWindow_Max2Responses";
                            _eventResponse1.Name = "Response1Button";
                            _eventResponse2.Name = "Response2Button";
                            break;
                        case 3:
                            _eventDialogWindow.Name = "EventDialogWindow_Max3Responses";
                            _eventResponse1.Name = "Response1Button";
                            _eventResponse2.Name = "Response2Button";
                            _eventResponse3.Name = "Response3Button";
                            break;
                        case 4:
                            _eventDialogWindow.Name = "EventDialogWindow_Max4Responses";
                            _eventResponse1.Name = "Response1Button";
                            _eventResponse2.Name = "Response2Button";
                            _eventResponse3.Name = "Response3Button";
                            _eventResponse4.Name = "Response4Button";
                            break;
                    }

                    //TODO: LOAD FROM XML HERE
                    Gui.LoadRootUIData(_eventDialogWindow, "InGame.xml");

                    if (faceTex != null)
                    {
                        _eventFace.Show();
                        _eventFace.Texture = faceTex;
                        _eventDialogArea.Show();
                        _eventDialogAreaNoFace.Hide();
                    }
                    else
                    {
                        _eventFace.Hide();
                        _eventDialogArea.Hide();
                        _eventDialogAreaNoFace.Show();
                    }

                    if (responseCount == 0)
                    {
                        _eventResponse1.Show();
                        _eventResponse1.SetText(Strings.Get("eventwindow", "continue"));
                        _eventResponse2.Hide();
                        _eventResponse3.Hide();
                        _eventResponse4.Hide();
                    }
                    else
                    {
                        if (Globals.EventDialogs[0].Opt1 != "")
                        {
                            _eventResponse1.Show();
                            _eventResponse1.SetText(Globals.EventDialogs[0].Opt1);
                        }
                        else
                        {
                            _eventResponse1.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt2 != "")
                        {
                            _eventResponse2.Show();
                            _eventResponse2.SetText(Globals.EventDialogs[0].Opt2);
                        }
                        else
                        {
                            _eventResponse2.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt3 != "")
                        {
                            _eventResponse3.Show();
                            _eventResponse3.SetText(Globals.EventDialogs[0].Opt3);
                        }
                        else
                        {
                            _eventResponse3.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt4 != "")
                        {
                            _eventResponse4.Show();
                            _eventResponse4.SetText(Globals.EventDialogs[0].Opt4);
                        }
                        else
                        {
                            _eventResponse4.Hide();
                        }
                    }
                    _eventDialogWindow.SetSize(_eventDialogWindow.Texture.GetWidth(),
                        _eventDialogWindow.Texture.GetHeight());

                    if (faceTex != null)
                    {
                        _eventDialogLabel.ClearText();
                        _eventDialogLabel.Width = _eventDialogArea.Width -
                                                  _eventDialogArea.GetVerticalScrollBar().Width;
                        _eventDialogLabel.AddText(Globals.EventDialogs[0].Prompt, _eventDialogLabelTemplate.TextColor,
                            _eventDialogLabelTemplate.CurAlignments.Count > 0
                                ? _eventDialogLabelTemplate.CurAlignments[0]
                                : Alignments.Left, _eventDialogLabelTemplate.Font);
                        _eventDialogLabel.SizeToChildren(false, true);
                    }
                    else
                    {
                        _eventDialogLabelNoFace.ClearText();
                        _eventDialogLabelNoFace.Width = _eventDialogAreaNoFace.Width -
                                                        _eventDialogAreaNoFace.GetVerticalScrollBar().Width;
                        _eventDialogLabelNoFace.AddText(Globals.EventDialogs[0].Prompt,
                            _eventDialogLabelNoFaceTemplate.TextColor,
                            _eventDialogLabelNoFaceTemplate.CurAlignments.Count > 0
                                ? _eventDialogLabelNoFaceTemplate.CurAlignments[0]
                                : Alignments.Left, _eventDialogLabelNoFaceTemplate.Font);
                        _eventDialogLabelNoFace.SizeToChildren(false, true);
                    }
                }
            }
        }

        //Input Handlers
        void EventResponse4_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(4, ed);
            _eventDialogWindow.RemoveModal();
            _eventDialogWindow.IsHidden = true;
            ed.ResponseSent = 1;
        }

        void EventResponse3_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(3, ed);
            _eventDialogWindow.RemoveModal();
            _eventDialogWindow.IsHidden = true;
            ed.ResponseSent = 1;
        }

        void EventResponse2_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(2, ed);
            _eventDialogWindow.RemoveModal();
            _eventDialogWindow.IsHidden = true;
            ed.ResponseSent = 1;
        }

        void EventResponse1_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(1, ed);
            _eventDialogWindow.RemoveModal();
            _eventDialogWindow.IsHidden = true;
            ed.ResponseSent = 1;
        }
    }
}