/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System.Windows.Forms.VisualStyles;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class EventWindow : IGUIElement
    {
        //Window Controls
        private WindowControl _eventDialogWindow;
        private ImagePanel _eventFace;
        private ListBox _eventDialog;
        private Button _eventResponse1;
        private Button _eventResponse2;
        private Button _eventResponse3;
        private Button _eventResponse4;

        //Init
        public EventWindow(Canvas _gameCanvas)
        {
            //Event Dialog Window
            _eventDialogWindow = new WindowControl(_gameCanvas, "Event Dialog");
            _eventDialogWindow.SetSize(500, 156);
            _eventDialogWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - 500/2, GameGraphics.Renderer.GetScreenHeight() / 2 - 156 / 2);
            _eventDialogWindow.IsClosable = false;
            _eventDialogWindow.DisableResizing();
            _eventDialogWindow.Margin = Margin.Zero;
            _eventDialogWindow.Padding = Padding.Zero;
            _eventDialogWindow.IsHidden = true;

            _eventFace = new ImagePanel(_eventDialogWindow);
            _eventFace.SetPosition(6, 6);
            _eventFace.SetSize(80, 80);
            _eventFace.IsHidden = true;

            _eventDialog = new ListBox(_eventDialogWindow);
            _eventDialog.IsDisabled = true;
            _eventDialog.SetPosition(92, 6);
            _eventDialog.SetSize(402, 80);
            _eventDialog.ShouldDrawBackground = false;

            _eventResponse1 = new Button(_eventDialogWindow);
            _eventResponse1.SetSize(120, 32);
            _eventResponse1.SetPosition(_eventDialogWindow.Width / 2 - 120 / 2, 94);
            _eventResponse1.SetText("Response 1");
            _eventResponse1.Clicked += EventResponse1_Clicked;

            _eventResponse2 = new Button(_eventDialogWindow);
            _eventResponse2.SetSize(120, 32);
            _eventResponse2.SetPosition(_eventDialogWindow.Width / 2 - 120 / 2, 94);
            _eventResponse2.SetText("Response 2");
            _eventResponse2.Clicked += EventResponse2_Clicked;

            _eventResponse3 = new Button(_eventDialogWindow);
            _eventResponse3.SetSize(120, 32);
            _eventResponse3.SetPosition(_eventDialogWindow.Width / 2 - 120 / 2, 94);
            _eventResponse3.SetText("Response 3");
            _eventResponse3.Clicked += EventResponse3_Clicked;

            _eventResponse4 = new Button(_eventDialogWindow);
            _eventResponse4.SetSize(120, 32);
            _eventResponse4.SetPosition(_eventDialogWindow.Width / 2 - 120 / 2, 94);
            _eventResponse4.SetText("Response 4");
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
                    _eventDialog.Clear();
                    if (GameGraphics.FaceFileNames.IndexOf(Globals.EventDialogs[0].Face) > -1)
                    {
                        _eventFace.Show();
                        _eventFace.Texture =
                            Gui.ToGwenTexture(
                                GameGraphics.FaceTextures[
                                    GameGraphics.FaceFileNames.IndexOf(Globals.EventDialogs[0].Face)]);
                        _eventDialog.Width = 402;
                        _eventDialog.X = 96;
                    }
                    else
                    {
                        _eventFace.Hide();
                        _eventDialog.Width = 488;
                        _eventDialog.X = 6;

                    }
                    var myText = Gui.WrapText(Globals.EventDialogs[0].Prompt, _eventDialog.Width - 12);
                    foreach (var t in myText)
                    {
                        var rw = _eventDialog.AddRow(t);
                        rw.MouseInputEnabled = false;
                    }

                    int responseCount = 0;
                    int buttonsAdded = 0;
                    if (Globals.EventDialogs[0].Opt1.Length > 0) responseCount++;
                    if (Globals.EventDialogs[0].Opt2.Length > 0) responseCount++;
                    if (Globals.EventDialogs[0].Opt3.Length > 0) responseCount++;
                    if (Globals.EventDialogs[0].Opt4.Length > 0) responseCount++;
                    FloatRect rect = new FloatRect(0, 0, responseCount*(_eventResponse1.Width + 12), 32);
                    rect.X = _eventDialogWindow.Width/2 - rect.Width/2;

                    if (responseCount == 0)
                    {
                        _eventResponse1.Show();
                        _eventResponse1.SetText("Continue");
                        _eventResponse1.X = _eventDialogWindow.Width/2 - _eventResponse1.Width/2;
                        _eventResponse2.Hide();
                        _eventResponse3.Hide();
                        _eventResponse4.Hide();
                    }
                    else
                    {
                        if (Globals.EventDialogs[0].Opt1 != "")
                        {
                            _eventResponse1.Show();
                            _eventResponse1.X = (int)rect.X + 6 + (buttonsAdded++ * (_eventResponse1.Width + 6));
                            _eventResponse1.SetText(Globals.EventDialogs[0].Opt1);
                        }
                        else
                        {
                            _eventResponse1.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt2 != "")
                        {
                            _eventResponse2.Show();
                            _eventResponse2.X = (int)rect.X + 6 + (buttonsAdded++ * (_eventResponse1.Width + 6));
                            _eventResponse2.SetText(Globals.EventDialogs[0].Opt2);
                        }
                        else
                        {
                            _eventResponse2.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt3 != "")
                        {
                            _eventResponse3.Show();
                            _eventResponse3.X = (int)rect.X + 6 + (buttonsAdded++ * (_eventResponse1.Width + 6));
                            _eventResponse3.SetText(Globals.EventDialogs[0].Opt3);
                        }
                        else
                        {
                            _eventResponse3.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt4 != "")
                        {
                            _eventResponse4.Show();
                            _eventResponse4.X = (int)rect.X + 6 + (buttonsAdded++ * (_eventResponse1.Width + 6));
                            _eventResponse4.SetText(Globals.EventDialogs[0].Opt4);
                        }
                        else
                        {
                            _eventResponse4.Hide();
                        }
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
            _eventDialogWindow.ToggleHidden();
            ed.ResponseSent = 1;
        }
        void EventResponse3_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(3, ed);
            _eventDialogWindow.RemoveModal();
            _eventDialogWindow.ToggleHidden();
            ed.ResponseSent = 1;
        }
        void EventResponse2_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(2, ed);
            _eventDialogWindow.RemoveModal();
            _eventDialogWindow.ToggleHidden();
            ed.ResponseSent = 1;
        }
        void EventResponse1_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ed = Globals.EventDialogs[0];
            if (ed.ResponseSent != 0) return;
            PacketSender.SendEventResponse(1, ed);
            _eventDialogWindow.RemoveModal();
            _eventDialogWindow.ToggleHidden();
            ed.ResponseSent = 1;
        }
    }
}
