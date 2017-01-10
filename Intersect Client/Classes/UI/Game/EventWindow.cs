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

using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Library.Localization;

namespace Intersect_Client.Classes.UI.Game
{
    public class EventWindow
    {
        //Window Controls
        private ImagePanel _eventDialogWindow;
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
            _eventDialogWindow = new ImagePanel(_gameCanvas);
            _eventDialogWindow.SetSize(530, 300);
            _eventDialogWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - 530/2, GameGraphics.Renderer.GetScreenHeight() / 2 - 300 / 2);
            _eventDialogWindow.Margin = Margin.Zero;
            _eventDialogWindow.Padding = new Padding(16, 8, 9, 11);
            _eventDialogWindow.IsHidden = true;
            _eventDialogWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "event4responses.png");
            Gui.InputBlockingElements.Add(_eventDialogWindow);

            _eventFace = new ImagePanel(_eventDialogWindow);
            _eventFace.SetPosition(6 + _eventDialogWindow.Padding.Left, 6 + _eventDialogWindow.Padding.Top);
            _eventFace.SetSize(80, 80);
            _eventFace.IsHidden = true;

            _eventDialog = new ListBox(_eventDialogWindow);
            _eventDialog.IsDisabled = true;
            _eventDialog.SetPosition(92 + _eventDialogWindow.Padding.Left, 6 + _eventDialogWindow.Padding.Top);
            _eventDialog.SetSize(402, 80);
            _eventDialog.ShouldDrawBackground = false;
            _eventDialogWindow.RenderColor = Color.White;

            var scrollBar = _eventDialog.GetVerticalScrollBar();
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

            _eventResponse1 = new Button(_eventDialogWindow);
            _eventResponse1.SetSize(488, 41);
            _eventResponse1.SetPosition(6 + _eventDialogWindow.Padding.Left, 99 + _eventDialogWindow.Padding.Top);
            _eventResponse1.SetText("Response 1");
            _eventResponse1.Clicked += EventResponse1_Clicked;
            _eventResponse1.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponsenormal.png"), Button.ControlState.Normal);
            _eventResponse1.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponsehover.png"), Button.ControlState.Hovered);
            _eventResponse1.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponseclicked.png"), Button.ControlState.Clicked);
            _eventResponse1.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _eventResponse1.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _eventResponse1.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);

            _eventResponse2 = new Button(_eventDialogWindow);
            _eventResponse2.SetSize(488, 41);
            _eventResponse2.SetPosition(6 + _eventDialogWindow.Padding.Left, 99 + 45 + _eventDialogWindow.Padding.Top);
            _eventResponse2.SetText("Response 2");
            _eventResponse2.Clicked += EventResponse2_Clicked;
            _eventResponse2.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponsenormal.png"), Button.ControlState.Normal);
            _eventResponse2.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponsehover.png"), Button.ControlState.Hovered);
            _eventResponse2.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponseclicked.png"), Button.ControlState.Clicked);
            _eventResponse2.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _eventResponse2.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _eventResponse2.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);

            _eventResponse3 = new Button(_eventDialogWindow);
            _eventResponse3.SetSize(488, 41);
            _eventResponse3.SetPosition(6 + _eventDialogWindow.Padding.Left, 99 + 45 * 2 + _eventDialogWindow.Padding.Top);
            _eventResponse3.SetText("Response 3");
            _eventResponse3.Clicked += EventResponse3_Clicked;
            _eventResponse3.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponsenormal.png"), Button.ControlState.Normal);
            _eventResponse3.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponsehover.png"), Button.ControlState.Hovered);
            _eventResponse3.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponseclicked.png"), Button.ControlState.Clicked);
            _eventResponse3.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _eventResponse3.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _eventResponse3.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);

            _eventResponse4 = new Button(_eventDialogWindow);
            _eventResponse4.SetSize(488, 41);
            _eventResponse4.SetPosition(6 + _eventDialogWindow.Padding.Left, 99 + 45 * 3 + _eventDialogWindow.Padding.Top);
            _eventResponse4.SetText("Response 4");
            _eventResponse4.Clicked += EventResponse4_Clicked;
            _eventResponse4.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponsenormal.png"), Button.ControlState.Normal);
            _eventResponse4.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponsehover.png"), Button.ControlState.Hovered);
            _eventResponse4.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventresponseclicked.png"), Button.ControlState.Clicked);
            _eventResponse4.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _eventResponse4.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _eventResponse4.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
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
                    _eventDialog.ScrollToTop();
                    _eventDialog.Clear();
                    GameTexture faceTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face,
                        Globals.EventDialogs[0].Face);
                    if (faceTex != null)
                    {
                        _eventFace.Show();
                        _eventFace.Texture = faceTex;
                        _eventDialog.Width = 402;
                        _eventDialog.X = 96 + _eventDialogWindow.Padding.Left;
                    }
                    else
                    {
                        _eventFace.Hide();
                        _eventDialog.Width = 488;
                        _eventDialog.X = 6 + _eventDialogWindow.Padding.Left;

                    }
                    var myText = Gui.WrapText(Globals.EventDialogs[0].Prompt, _eventDialog.Width - 12, _eventDialogWindow.Parent.Skin.DefaultFont);
                    foreach (var t in myText)
                    {
                        var rw = _eventDialog.AddRow(t);
                        rw.SetTextColor(Color.White);
                        rw.MouseInputEnabled = false;
                    }

                    int responseCount = 0;
                    if (Globals.EventDialogs[0].Opt1.Length > 0) responseCount++;
                    if (Globals.EventDialogs[0].Opt2.Length > 0) responseCount++;
                    if (Globals.EventDialogs[0].Opt3.Length > 0) responseCount++;
                    if (Globals.EventDialogs[0].Opt4.Length > 0) responseCount++;

                    if (responseCount == 0)
                    {
                        _eventResponse1.Show();
                        _eventResponse1.SetText(Strings.Get("eventwindow","continue"));
                        _eventResponse2.Hide();
                        _eventResponse3.Hide();
                        _eventResponse4.Hide();
                        _eventDialogWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventdefault.png");
                    }
                    else
                    {
                        if (Globals.EventDialogs[0].Opt1 != "")
                        {
                            _eventResponse1.Show();
                            _eventResponse1.SetText(Globals.EventDialogs[0].Opt1);
                            _eventDialogWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "eventdefault.png");
                        }
                        else
                        {
                            _eventResponse1.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt2 != "")
                        {
                            _eventResponse2.Show();
                            _eventResponse2.SetText(Globals.EventDialogs[0].Opt2);
                            _eventDialogWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "event2responses.png");
                        }
                        else
                        {
                            _eventResponse2.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt3 != "")
                        {
                            _eventResponse3.Show();
                            _eventResponse3.SetText(Globals.EventDialogs[0].Opt3);
                            _eventDialogWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "event3responses.png");
                        }
                        else
                        {
                            _eventResponse3.Hide();
                        }
                        if (Globals.EventDialogs[0].Opt4 != "")
                        {
                            _eventResponse4.Show();
                            _eventResponse4.SetText(Globals.EventDialogs[0].Opt4);
                            _eventDialogWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "event4responses.png");
                        }
                        else
                        {
                            _eventResponse4.Hide();
                        }
                    }
                    _eventDialogWindow.SetSize(_eventDialogWindow.Texture.GetWidth(), _eventDialogWindow.Texture.GetHeight());
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
