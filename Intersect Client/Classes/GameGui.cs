using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Gwen;
using Gwen.Control;

namespace Intersect_Client.Classes
{
    public class GameGui
    {
        private readonly Canvas _gameCanvas;
        private static readonly List<Base> FocusElements = new List<Base>();
        public bool FocusChat;

        public GameGui(Canvas myCanvas)
        {
            _gameCanvas = myCanvas;
            InitGameGui();
        }

        //Game GUI Elements
        public WindowControl EventDialogWindow;
        public ListBox EventDialog;
        public Button EventResponse1;
        public Button EventResponse2;
        public Button EventResponse3;
        public Button EventResponse4;

        //ChatBox
        public WindowControl ChatboxWindow;
        public ListBox ChatBoxMessages;
        public TextBox ChatBoxInput;
        public Button ChatBoxSendBtn;

        //Menu
        public WindowControl GameMenu;
        public Button InventoryBtn;
        public Button SkillsBtn;
        public Button CharacterBtn;
        public Button QuestsBtn;
        public Button OptionBtn;
        public Button CloseBtn;

        //Options Menu
        public WindowControl OMenu;
        public Label OResolutionLabel;
        public ComboBox OResolutionList;
        public LabeledCheckBox OFullscreen;
        public Label OSoundLabel;
        public LabeledCheckBox OEnableSound;
        public LabeledCheckBox ODisableSound;
        public Label OMusicLabel;
        public LabeledCheckBox OEnableMusic;
        public LabeledCheckBox ODisableMusic;
        public Button OApplyBtn;
        public Button OBackBtn;

        //Player Box (HP/Mana)
        public WindowControl PBox;
        public Label PHpLbl;
        public Label PMpLbl;
        public Label PExpLbl;
        public ImagePanel PHpBackground;
        public ImagePanel PHpBar;
        public ImagePanel PMpBackground;
        public ImagePanel PMpBar;
        public ImagePanel PExpBackground;
        public ImagePanel PExpBar;


        #region "Game GUI"
        public void InitGameGui()
        {
            //Event Dialog Window
            EventDialogWindow = new WindowControl(_gameCanvas, "Event Dialog");
            EventDialogWindow.SetSize(200, 260);
            EventDialogWindow.SetPosition(Graphics.ScreenWidth / 2 - 100, Graphics.ScreenHeight / 2 - 260 / 2);
            EventDialogWindow.IsClosable = false;
            EventDialogWindow.DisableResizing();
            EventDialogWindow.Margin = Margin.Zero;
            EventDialogWindow.Padding = Padding.Zero;
            EventDialogWindow.IsHidden = true;

            EventDialog = new ListBox(EventDialogWindow);
            var myText = Gui.WrapText("This is a really long string of text that I really, really want to get working with my text box, please wish me luck and hopefully in 10 minutes I will have something decent.", 180);
            foreach (var t in myText)
            {
                var rw = EventDialog.AddRow(t);
                rw.MouseInputEnabled = false;
            }
            EventDialog.IsDisabled = true;
            EventDialog.SetPosition(6, 6);
            EventDialog.SetSize(188, 80);
            EventDialog.ShouldDrawBackground = false;


            EventResponse1 = new Button(EventDialogWindow);
            EventResponse1.SetSize(120, 32);
            EventResponse1.SetPosition(EventDialogWindow.Width / 2 - 120 / 2, 94);
            EventResponse1.SetText("Response 1");
            EventResponse1.Clicked += EventResponse1_Clicked;

            EventResponse2 = new Button(EventDialogWindow);
            EventResponse2.SetSize(120, 32);
            EventResponse2.SetPosition(EventDialogWindow.Width / 2 - 120 / 2, 130);
            EventResponse2.SetText("Response 2");
            EventResponse2.Clicked += EventResponse2_Clicked;

            EventResponse3 = new Button(EventDialogWindow);
            EventResponse3.SetSize(120, 32);
            EventResponse3.SetPosition(EventDialogWindow.Width / 2 - 120 / 2, 164);
            EventResponse3.SetText("Response 3");
            EventResponse3.Clicked += EventResponse3_Clicked;

            EventResponse4 = new Button(EventDialogWindow);
            EventResponse4.SetSize(120, 32);
            EventResponse4.SetPosition(EventDialogWindow.Width / 2 - 120 / 2, 198);
            EventResponse4.SetText("Response 4");
            EventResponse4.Clicked += EventResponse4_Clicked;

            //Chatbox Window
            ChatboxWindow = new WindowControl(_gameCanvas, "Chatbox") {IsClosable = false};
            ChatboxWindow.SetSize(380, 140);
            ChatboxWindow.DisableResizing();
            ChatboxWindow.SetPosition(0, Graphics.ScreenHeight - 140);
            ChatboxWindow.Margin = Margin.Zero;
            ChatboxWindow.Padding = Padding.Zero;

            ChatBoxMessages = new ListBox(ChatboxWindow) {IsDisabled = true};
            ChatBoxMessages.SetPosition(2, 1);
            ChatBoxMessages.SetSize(376, 90);
            ChatBoxMessages.ShouldDrawBackground = false;

            ChatBoxInput = new TextBox(ChatboxWindow);
            ChatBoxInput.SetPosition(2, 140 - 18 - 24);
            ChatBoxInput.SetSize(338, 16);
            ChatBoxInput.SubmitPressed += ChatBoxInput_SubmitPressed;
            ChatBoxInput.Text = "Press enter to chat.";
            ChatBoxInput.Clicked += ChatBoxInput_Clicked;
            FocusElements.Add(ChatBoxInput);

            ChatBoxSendBtn = new Button(ChatboxWindow);
            ChatBoxSendBtn.SetSize(36, 16);
            ChatBoxSendBtn.SetPosition(342, 140 - 18 - 24);
            ChatBoxSendBtn.Text = "Send";
            ChatBoxSendBtn.Clicked += ChatBoxSendBtn_Clicked;

            //Game Menu Window
            GameMenu = new WindowControl(_gameCanvas, "Game Menu") {IsClosable = false};
            GameMenu.DisableResizing();
            GameMenu.SetSize(166, 84);
            GameMenu.SetPosition(Graphics.ScreenWidth - 116, Graphics.ScreenHeight - 84);
            GameMenu.Margin = Margin.Zero;
            GameMenu.Padding = Padding.Zero;

            InventoryBtn = new Button(GameMenu);
            InventoryBtn.SetSize(50, 24);
            InventoryBtn.SetText("Inventory");
            InventoryBtn.SetPosition(4, 4);

            SkillsBtn = new Button(GameMenu);
            SkillsBtn.SetSize(50, 24);
            SkillsBtn.SetText("Skills");
            SkillsBtn.SetPosition(58, 4);

            CharacterBtn = new Button(GameMenu);
            CharacterBtn.SetSize(50, 24);
            CharacterBtn.SetText("Character");
            CharacterBtn.SetPosition(112, 4);

            QuestsBtn = new Button(GameMenu);
            QuestsBtn.SetSize(50, 24);
            QuestsBtn.SetText("Missions");
            QuestsBtn.SetPosition(4, 32);

            OptionBtn = new Button(GameMenu);
            OptionBtn.SetSize(50, 24);
            OptionBtn.SetText("Options");
            OptionBtn.SetPosition(58, 32);
            OptionBtn.Clicked += OptionBtn_Clicked;

            CloseBtn = new Button(GameMenu);
            CloseBtn.SetSize(50, 24);
            CloseBtn.SetText("Exit");
            CloseBtn.SetPosition(112, 32);
            CloseBtn.Clicked += CloseBtn_Clicked;

            //Options Window
            OMenu = new WindowControl(_gameCanvas, "Options");
            OMenu.SetSize(200, 200);
            OMenu.SetPosition(Graphics.ScreenWidth / 2 - 100, Graphics.ScreenHeight / 2 - 80);
            OMenu.DisableResizing();
            OMenu.Margin = Margin.Zero;
            OMenu.Padding = Padding.Zero;
            OMenu.IsHidden = true;

            OResolutionList = new ComboBox(OMenu);
            var myModes = Graphics.GetValidVideoModes();
            for (var i = 0; i < myModes.Count(); i++)
            {
                OResolutionList.AddItem(myModes[i].Width + "x" + myModes[i].Height);
            }
            OResolutionList.SetSize(120, 14);
            OResolutionList.SetPosition(OMenu.Width / 2 - 120 / 2, 28);

            //Options - Fullscreen Checkbox
            OFullscreen = new LabeledCheckBox(OMenu) {Text = "Fullscreen"};
            OFullscreen.SetSize(120, 14);
            OFullscreen.SetPosition(OMenu.Width / 2 - 120 / 2, 46);

            //Options - Sound Label
            OSoundLabel = new Label(OMenu);
            OSoundLabel.SetText("Sound:");
            OSoundLabel.SetPosition(OMenu.Width / 2 - 120 / 2, 64);

            //Options - Sound On Checkbox
            OEnableSound = new LabeledCheckBox(OMenu) {Text = "On"};
            OEnableSound.SetSize(56, 14);
            OEnableSound.SetPosition(OMenu.Width / 2 - 120 / 2, 82);
            OEnableSound.Checked += OEnableSound_Checked;

            //Options - Sound Off Checkbox
            ODisableSound = new LabeledCheckBox(OMenu) {Text = "Off"};
            ODisableSound.SetSize(56, 14);
            ODisableSound.SetPosition(OMenu.Width / 2 + 4, 82);
            ODisableSound.Checked += ODisableSound_Checked;

            //Options - Music Label
            OMusicLabel = new Label(OMenu);
            OMusicLabel.SetText("Music:");
            OMusicLabel.SetPosition(OMenu.Width / 2 - 120 / 2, 100);

            //Options - Music On Checkbox
            OEnableMusic = new LabeledCheckBox(OMenu) {Text = "On"};
            OEnableMusic.SetSize(56, 14);
            OEnableMusic.SetPosition(OMenu.Width / 2 - 120 / 2, 118);
            OEnableMusic.Checked += OEnableMusic_Checked;

            //Options - Music Off Checkbox
            ODisableMusic = new LabeledCheckBox(OMenu) {Text = "Off"};
            ODisableMusic.SetSize(56, 14);
            ODisableMusic.SetPosition(OMenu.Width / 2 + 4, 118);
            ODisableMusic.Checked += ODisableMusic_Checked;

            //Options - Apply Button
            OApplyBtn = new Button(OMenu);
            OApplyBtn.SetText("Apply");
            OApplyBtn.SetPosition(OMenu.Width / 2 - 120 / 2, 136);
            OApplyBtn.SetSize(120, 32);
            OApplyBtn.Clicked += OApplyBtn_Clicked;

            //Player Box (Hp/mana bars)
            PBox = new WindowControl(_gameCanvas) {Title = "Vitals"};
            PBox.SetSize(173, 80);
            PBox.SetPosition(0, 0);
            PBox.DisableResizing();

            PBox.Margin = Margin.Zero;
            PBox.Padding = Padding.Zero;
            PBox.IsClosable = false;

            PHpBackground = new ImagePanel(PBox) {ImageName = "data/graphics/gui/HPBarEmpty.png"};
            PHpBackground.SetSize(169, 17);
            PHpBackground.SetPosition(2, 0);

            PHpBar = new ImagePanel(PBox) {ImageName = "data/graphics/gui/HPBar.png"};
            PHpBar.SetSize(169, 14);
            PHpBar.SetPosition(2, 3);

            PHpLbl = new Label(PBox);
            PHpLbl.SetText("1000/1000");
            PHpLbl.SetPosition(20, 3);
            PHpLbl.TextColor = Color.Black;

            PMpBackground = new ImagePanel(PBox) {ImageName = "data/graphics/gui/ManaBarEmpty.png"};
            PMpBackground.SetSize(169, 17);
            PMpBackground.SetPosition(2, 18);

            PMpBar = new ImagePanel(PBox) {ImageName = "data/graphics/gui/ManaBar.png"};
            PMpBar.SetSize(169, 14);
            PMpBar.SetPosition(2, 21);

            PMpLbl = new Label(PBox);
            PMpLbl.SetText("1000/1000");
            PMpLbl.SetPosition(20, 21);
            PMpLbl.TextColor = Color.Black;

            PExpBackground = new ImagePanel(PBox) {ImageName = "data/graphics/gui/EXPBarEmpty.png"};
            PExpBackground.SetSize(169, 17);
            PExpBackground.SetPosition(2, 36);

            PExpBar = new ImagePanel(PBox) {ImageName = "data/graphics/gui/EXPBar.png"};
            PExpBar.SetSize(169, 14);
            PExpBar.SetPosition(2, 39);

            PExpLbl = new Label(PBox);
            PExpLbl.SetText("1000/1000");
            PExpLbl.SetPosition(20, 39);
            PExpLbl.TextColor = Color.Black;




        }

        //Options Menu
        void ODisableMusic_Checked(Base sender, EventArgs arguments)
        {
            OEnableMusic.IsChecked = false;
        }
        void OEnableMusic_Checked(Base sender, EventArgs arguments)
        {
            ODisableMusic.IsChecked = false;
        }
        void ODisableSound_Checked(Base sender, EventArgs arguments)
        {
            OEnableSound.IsChecked = false;
        }
        void OEnableSound_Checked(Base sender, EventArgs arguments)
        {
            ODisableSound.IsChecked = false;
        }
        void OApplyBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var mi = OResolutionList.SelectedItem;
            var myModes = Graphics.GetValidVideoModes();
            for (var i = 0; i < myModes.Count(); i++)
            {
                if (mi.Text != myModes[i].Width + "x" + myModes[i].Height) continue;
                Graphics.DisplayMode = i;
                Graphics.MustReInit = true;
            }
            if (Graphics.FullScreen != OFullscreen.IsChecked)
            {
                Graphics.FullScreen = OFullscreen.IsChecked;
                Graphics.MustReInit = true;
            }
            Globals.MusicEnabled = OEnableMusic.IsChecked;
            Globals.SoundEnabled = OEnableSound.IsChecked;
            Database.SaveOptions();
        }

        //Game Menu
        void CloseBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameMain.IsRunning = false;
        }
        void OptionBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Select current resolution
            OResolutionList.SelectByText(Graphics.GetValidVideoModes()[Graphics.DisplayMode].Width + "x" + Graphics.GetValidVideoModes()[Graphics.DisplayMode].Height);
            OFullscreen.IsChecked = Graphics.FullScreen;
            OEnableSound.IsChecked = Globals.SoundEnabled;
            ODisableSound.IsChecked = !Globals.SoundEnabled;
            OEnableMusic.IsChecked = Globals.MusicEnabled;
            ODisableMusic.IsChecked = !Globals.MusicEnabled;
            OMenu.IsHidden = false;
        }

        //Chatbox Window
        void ChatBoxInput_Clicked(Base sender, ClickedEventArgs arguments)
        {

            if (ChatBoxInput.Text == "Press enter to chat.") { ChatBoxInput.Text = ""; }
        }
        void ChatBoxInput_SubmitPressed(Base sender, EventArgs arguments)
        {
            if (ChatBoxInput.Text.Trim().Length <= 0 || ChatBoxInput.Text == "Press enter to chat.") return;
            PacketSender.SendChatMsg(ChatBoxInput.Text.Trim());
            ChatBoxInput.Text = "Press enter to chat.";
        }
        void ChatBoxSendBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (ChatBoxInput.Text.Trim().Length <= 0 || ChatBoxInput.Text == "Press enter to chat.") return;
            PacketSender.SendChatMsg(ChatBoxInput.Text.Trim());
            ChatBoxInput.Text = "Press enter to chat.";
        }

        //Event Dialog Window
        void EventResponse4_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(4, ed);
            ed.ResponseSent = 1;
        }
        void EventResponse3_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(3, ed);
            ed.ResponseSent = 1;
        }
        void EventResponse2_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(2, ed);
            ed.ResponseSent = 1;
        }
        void EventResponse1_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(1, ed);
            ed.ResponseSent = 1;
        }


        #endregion

        public void Draw()
        {
            while (Gui.MsgboxErrors.Count > 0)
            {
                var msgbox = new MessageBox(_gameCanvas, Gui.MsgboxErrors[0], "Error!");
                msgbox.Resized += Msgbox_Resized;
                Gui.MsgboxErrors.RemoveAt(0);
            }
            if (Globals.EventDialogs.Count > 0)
            {
                if (EventDialogWindow.IsHidden)
                {
                    EventDialogWindow.Show();
                    EventDialog.Clear();
                    var myText = Gui.WrapText(Globals.EventDialogs[0].Prompt, 180);
                    foreach (var t in myText)
                    {
                        var rw = EventDialog.AddRow(t);
                        rw.MouseInputEnabled = false;
                    }
                    if (Globals.EventDialogs[0].Opt1.Length > 0 || Globals.EventDialogs[0].Opt2.Length > 0 || Globals.EventDialogs[0].Opt3.Length > 0 || Globals.EventDialogs[0].Opt4.Length > 0)
                    {
                        if (Globals.EventDialogs[0].Opt1 != "")
                        {
                            EventResponse1.Show();
                            EventResponse1.SetText(Globals.EventDialogs[0].Opt1);
                        }
                        else
                        {
                            EventResponse1.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt2 != "")
                        {
                            EventResponse2.Show();
                            EventResponse2.SetText(Globals.EventDialogs[0].Opt2);
                        }
                        else
                        {
                            EventResponse2.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt3 != "")
                        {
                            EventResponse3.Show();
                            EventResponse3.SetText(Globals.EventDialogs[0].Opt3);
                        }
                        else
                        {
                            EventResponse3.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt4 != "")
                        {
                            EventResponse4.Show();
                            EventResponse4.SetText(Globals.EventDialogs[0].Opt4);
                        }
                        else
                        {
                            EventResponse4.Hide();
                        }
                    }
                    else
                    {
                        EventResponse1.Show();
                        EventResponse1.SetText("Continue");
                        EventResponse2.Hide();
                        EventResponse3.Hide();
                        EventResponse4.Hide();
                    }


                }
            }
            if (Globals.ChatboxContent.Count > 0)
            {
                foreach (var t1 in Globals.ChatboxContent)
                {
                    var myText = Gui.WrapText((string)t1, 360);
                    foreach (var t in myText)
                    {
                        var rw = ChatBoxMessages.AddRow(t);
                        rw.MouseInputEnabled = false;
                    }
                }

                Globals.ChatboxContent.Clear();
                ChatBoxMessages.Redraw();
                ChatBoxMessages.Think();
                ChatBoxMessages.ScrollToBottom();

            }
            if (FocusChat)
            {
                ChatBoxInput.Focus();
                ChatBoxInput.Text = "";
                FocusChat = false;
            }
            _gameCanvas.RenderCanvas();
        }

        protected virtual void Msgbox_Resized(Base sender, EventArgs arguments)
        {
            sender.SetPosition(Graphics.ScreenWidth / 2 - sender.Width / 2, Graphics.ScreenHeight / 2 - sender.Height / 2);
        }
        public bool HasInputFocus()
        {
            for (var i = 0; i < FocusElements.Count(); i++)
            {
                if (FocusElements[i].HasFocus)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
