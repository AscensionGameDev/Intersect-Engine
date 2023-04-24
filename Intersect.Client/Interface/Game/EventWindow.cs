using System.Linq;
using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Typewriting;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game
{

    public partial class EventWindow : Base
    {

        private ScrollControl mEventDialogArea;

        private ScrollControl mEventDialogAreaNoFace;

        private RichLabel mEventDialogLabel;

        private RichLabel mEventDialogLabelNoFace;

        private Label mEventDialogLabelNoFaceTemplate;

        private Label mEventDialogLabelTemplate;

        //Window Controls
        private ImagePanel mEventDialogWindow;

        private ImagePanel mEventFace;

        private Button mEventResponse1;

        private Button mEventResponse2;

        private Button mEventResponse3;

        private Button mEventResponse4;

        private bool _typewriting = false;

        private readonly Typewriter _writer;

        private readonly long _typewriterResponseDelay = ClientConfiguration.Instance.TypewriterResponseDelay;

        //Init
        public EventWindow(Canvas gameCanvas)
        {
            //Event Dialog Window
            mEventDialogWindow = new ImagePanel(gameCanvas, "EventDialogueWindow");
            mEventDialogWindow.Hide();
            Interface.InputBlockingElements.Add(mEventDialogWindow);

            mEventFace = new ImagePanel(mEventDialogWindow, "EventFacePanel");

            mEventDialogArea = new ScrollControl(mEventDialogWindow, "EventDialogArea");
            mEventDialogLabelTemplate = new Label(mEventDialogArea, "EventDialogLabel");
            mEventDialogLabel = new RichLabel(mEventDialogArea);

            mEventDialogAreaNoFace = new ScrollControl(mEventDialogWindow, "EventDialogAreaNoFace");
            mEventDialogLabelNoFaceTemplate = new Label(mEventDialogAreaNoFace, "EventDialogLabel");
            mEventDialogLabelNoFace = new RichLabel(mEventDialogAreaNoFace);

            mEventResponse1 = new Button(mEventDialogWindow, "EventResponse1");
            mEventResponse1.Clicked += EventResponse1_Clicked;

            mEventResponse2 = new Button(mEventDialogWindow, "EventResponse2");
            mEventResponse2.Clicked += EventResponse2_Clicked;

            mEventResponse3 = new Button(mEventDialogWindow, "EventResponse3");
            mEventResponse3.Clicked += EventResponse3_Clicked;

            mEventResponse4 = new Button(mEventDialogWindow, "EventResponse4");
            mEventResponse4.Clicked += EventResponse4_Clicked;

            _writer = new Typewriter();

            mEventDialogWindow.Clicked += Dialog_Clicked;
        }

        private void Dialog_Clicked(Base sender, ClickedEventArgs arguments)
        {
            SkipTypewriting();
        }

        //Update
        public void Update()
        {
            if (mEventDialogWindow.IsHidden)
            {
                Interface.InputBlockingElements.Remove(this);
            }
            else
            {
                if (!Interface.InputBlockingElements.Contains(this))
                {
                    Interface.InputBlockingElements.Add(this);
                }
            }

            if (Globals.EventDialogs.Count > 0)
            {
                if (mEventDialogWindow.IsHidden)
                {
                    base.Show();
                    mEventDialogWindow.Show();
                    mEventDialogWindow.MakeModal();
                    mEventDialogArea.ScrollToTop();
                    mEventDialogWindow.BringToFront();
                    var faceTex = Globals.ContentManager.GetTexture(
                        Framework.Content.TextureType.Face, Globals.EventDialogs[0].Face
                    );

                    var responseCount = 0;
                    var maxResponse = 1;
                    if (Globals.EventDialogs[0].Opt1.Length > 0)
                    {
                        responseCount++;
                    }

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

                    _typewriting = ClientConfiguration.Instance.TypewriterEnabled && Globals.Database.TypewriterBehavior == Enums.TypewriterBehavior.Word;

                    mEventResponse1.Name = "";
                    mEventResponse2.Name = "";
                    mEventResponse3.Name = "";
                    mEventResponse4.Name = "";
                    switch (maxResponse)
                    {
                        case 1:
                            mEventDialogWindow.Name = "EventDialogWindow_1Response";
                            mEventResponse1.Name = "Response1Button";

                            break;
                        case 2:
                            mEventDialogWindow.Name = "EventDialogWindow_2Responses";
                            mEventResponse1.Name = "Response1Button";
                            mEventResponse2.Name = "Response2Button";

                            break;
                        case 3:
                            mEventDialogWindow.Name = "EventDialogWindow_3Responses";
                            mEventResponse1.Name = "Response1Button";
                            mEventResponse2.Name = "Response2Button";
                            mEventResponse3.Name = "Response3Button";

                            break;
                        case 4:
                            mEventDialogWindow.Name = "EventDialogWindow_4Responses";
                            mEventResponse1.Name = "Response1Button";
                            mEventResponse2.Name = "Response2Button";
                            mEventResponse3.Name = "Response3Button";
                            mEventResponse4.Name = "Response4Button";

                            break;
                    }

                    mEventDialogWindow.LoadJsonUi(
                        GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString()
                    );

                    if (faceTex != null)
                    {
                        mEventFace.Show();
                        mEventFace.Texture = faceTex;
                        mEventDialogArea.Show();
                        mEventDialogAreaNoFace.Hide();
                    }
                    else
                    {
                        mEventFace.Hide();
                        mEventDialogArea.Hide();
                        mEventDialogAreaNoFace.Show();
                    }

                    if (responseCount == 0)
                    {
                        mEventResponse1.Show();
                        mEventResponse1.SetText(Strings.EventWindow.Continue);
                        mEventResponse2.Hide();
                        mEventResponse3.Hide();
                        mEventResponse4.Hide();
                    }
                    else
                    {
                        if (Globals.EventDialogs[0].Opt1 != "")
                        {
                            mEventResponse1.Show();
                            mEventResponse1.SetText(Globals.EventDialogs[0].Opt1);
                        }
                        else
                        {
                            mEventResponse1.Hide();
                        }

                        if (Globals.EventDialogs[0].Opt2 != "")
                        {
                            mEventResponse2.Show();
                            mEventResponse2.SetText(Globals.EventDialogs[0].Opt2);
                        }
                        else
                        {
                            mEventResponse2.Hide();
                        }

                        if (Globals.EventDialogs[0].Opt3 != "")
                        {
                            mEventResponse3.Show();
                            mEventResponse3.SetText(Globals.EventDialogs[0].Opt3);
                        }
                        else
                        {
                            mEventResponse3.Hide();
                        }

                        if (Globals.EventDialogs[0].Opt4 != "")
                        {
                            mEventResponse4.Show();
                            mEventResponse4.SetText(Globals.EventDialogs[0].Opt4);
                        }
                        else
                        {
                            mEventResponse4.Hide();
                        }
                    }

                    mEventDialogWindow.SetSize(
                        mEventDialogWindow.Texture.GetWidth(), mEventDialogWindow.Texture.GetHeight()
                    );

                    var prompt = Globals.EventDialogs[0].Prompt;
                    if (faceTex != null)
                    {
                        ShowDialog(mEventDialogLabel, 
                            mEventDialogLabelTemplate, 
                            mEventDialogArea, 
                            prompt);
                    }
                    else
                    {
                        ShowDialog(mEventDialogLabelNoFace,
                            mEventDialogLabelNoFaceTemplate,
                            mEventDialogAreaNoFace,
                            prompt);
                    }
                }
                else if (_typewriting)
                {
                    var voiceIdx = Randomization.Next(0, ClientConfiguration.Instance.TypewriterSounds.Count);

                    var dialog = Globals.EventDialogs[0];

                    // Always show option 1 ("continue" if options empty)
                    mEventResponse1.IsHidden = !_writer.IsDone; 
                    mEventResponse2.IsHidden = !_writer.IsDone || string.IsNullOrEmpty(dialog.Opt2);
                    mEventResponse3.IsHidden = !_writer.IsDone || string.IsNullOrEmpty(dialog.Opt3);
                    mEventResponse4.IsHidden = !_writer.IsDone || string.IsNullOrEmpty(dialog.Opt4);

                    _writer.Write(ClientConfiguration.Instance.TypewriterSounds.ElementAtOrDefault(voiceIdx));
                    if (_writer.IsDone)
                    {
                        var disableResponse = Timing.Global.MillisecondsUtc - _writer.DoneAtMilliseconds < _typewriterResponseDelay;
                        mEventResponse1.IsDisabled = disableResponse;
                        mEventResponse2.IsDisabled = disableResponse;
                        mEventResponse3.IsDisabled = disableResponse;
                        mEventResponse4.IsDisabled = disableResponse;
                    }
                    else if (Controls.KeyDown(Control.AttackInteract))
                    {
                        SkipTypewriting();
                    }
                }
            }
        }

        private void ShowDialog(RichLabel dialogLabel, Label dialogLabelTemplate, ScrollControl dialogArea, string prompt)
        {
            if (dialogLabel == default || dialogLabelTemplate == default || dialogArea == default)
            {
                return;
            }

            dialogLabel.ClearText();
            dialogLabel.Width = dialogArea.Width - dialogArea.GetVerticalScrollBar().Width;

            dialogLabel.AddText(prompt, dialogLabelTemplate);

            dialogLabel.SizeToChildren(false, true);

            // Do this _after_ sizing so we have lines broken up
            if (_typewriting)
            {
                _writer.Initialize(dialogLabel.FormattedLabels);
                mEventResponse1.Hide();
                mEventResponse2.Hide();
                mEventResponse3.Hide();
                mEventResponse4.Hide();
            }

            dialogArea.ScrollToTop();
        }

        //Input Handlers
        void EventResponse4_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0)
            {
                return;
            }

            PacketSender.SendEventResponse(4, ed);
            mEventDialogWindow.RemoveModal();
            mEventDialogWindow.IsHidden = true;
            ed.ResponseSent = 1;
            base.Hide();
        }

        void EventResponse3_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0)
            {
                return;
            }

            PacketSender.SendEventResponse(3, ed);
            mEventDialogWindow.RemoveModal();
            mEventDialogWindow.IsHidden = true;
            ed.ResponseSent = 1;
            base.Hide();
        }

        void EventResponse2_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0)
            {
                return;
            }

            PacketSender.SendEventResponse(2, ed);
            mEventDialogWindow.RemoveModal();
            mEventDialogWindow.IsHidden = true;
            ed.ResponseSent = 1;
            base.Hide();
        }

        public void EventResponse1_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0)
            {
                return;
            }

            PacketSender.SendEventResponse(1, ed);
            mEventDialogWindow.RemoveModal();
            mEventDialogWindow.IsHidden = true;
            ed.ResponseSent = 1;
            base.Hide();
        }

        private void SkipTypewriting()
        {
            if (_writer?.IsDone ?? true)
            {
                return;
            }

            _writer.End();
        }
    }
}
