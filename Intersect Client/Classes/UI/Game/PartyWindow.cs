using System;
using System.Collections.Generic;
using Intersect;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.UI.Game
{
    public class PartyWindow
    {
        private List<ImagePanel> _bar = new List<ImagePanel>();
        private List<ImagePanel> _barContainer = new List<ImagePanel>();
        private List<Button> _kickButtons = new List<Button>();

        private List<Label> _lblnames = new List<Label>();
        private ImagePanel _leader;
        private Button _leaveButton;
        //Controls
        private WindowControl _partyWindow;

        //Init
        public PartyWindow(Canvas _gameCanvas)
        {
            _partyWindow = new WindowControl(_gameCanvas, Strings.Get("parties", "title"));
            _partyWindow.SetSize(228, 320);
            _partyWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 210,
                GameGraphics.Renderer.GetScreenHeight() - 500);
            _partyWindow.DisableResizing();
            _partyWindow.Margin = Margin.Zero;
            _partyWindow.Padding = new Padding(8, 5, 9, 11);
            _partyWindow.IsHidden = true;

            _partyWindow.SetTitleBarHeight(24);
            _partyWindow.SetCloseButtonSize(20, 20);
            _partyWindow.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "partyactive.png"),
                WindowControl.ControlState.Active);
            _partyWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"),
                Button.ControlState.Normal);
            _partyWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"),
                Button.ControlState.Hovered);
            _partyWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"),
                Button.ControlState.Clicked);
            _partyWindow.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont, 14));
            _partyWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            //Add the icon representing party leader (ALWAYS member 1 in the party list)
            _leader = new ImagePanel(_partyWindow);
            _leader.SetSize(34, 34);
            _leader.SetPosition(_partyWindow.Width - 80, 4);
            _leader.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "partyleader.png");
            _leader.SetToolTipText(Strings.Get("parties", "leader"));
            _leader.Hide();
            if (Globals.Me.Party.Count > 0)
            {
                _leader.Show();
            }

            _lblnames.Clear();
            _kickButtons.Clear();
            _barContainer.Clear();
            _bar.Clear();

            for (int i = 0; i < 4; i++)
            {
                //Labels
                _lblnames.Add(new Label(_partyWindow));
                _lblnames[i].SetPosition(4, 4 + (60 * i));
                if (i < Globals.Me.Party.Count)
                {
                    _lblnames[i].Text = Globals.Entities[Globals.Me.Party[i]].MyName;
                }
                else
                {
                    _lblnames[i].Text = "";
                }
                _lblnames[i].TextColorOverride = Color.White;

                //Health bars
                _barContainer.Add(new ImagePanel(_partyWindow));
                _barContainer[i].SetSize(183, 25);
                _barContainer[i].SetPosition(4, 34 + (60 * i));
                _barContainer[i].Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui,
                    "EmptyBar.png");
                _barContainer[i].Hide();
                if (i < Globals.Me.Party.Count)
                {
                    _barContainer[i].Show();
                }

                _bar.Add(new ImagePanel(_barContainer[i]));
                if (i < Globals.Me.Party.Count)
                {
                    float d =
                        (float)
                        ((float) Globals.Entities[Globals.Me.Party[i]].Vital[(int) Vitals.Health] /
                         (float) Globals.Entities[Globals.Me.Party[i]].MaxVital[(int) Vitals.Health]);
                    _bar[i].SetSize(Convert.ToInt32(d * 183), 25);
                }
                else
                {
                    _bar[i].SetSize(0, 25);
                }
                _bar[i].SetPosition(0, 0);
                _bar[i].Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "lifebar.png");

                if (i == 0)
                {
                    _kickButtons.Add(null);
                }
                else
                {
                    _kickButtons.Add(new Button(_partyWindow));
                    _kickButtons[i].SetSize(34, 34);
                    _kickButtons[i].SetPosition(_partyWindow.Width - 45, 30 + (60 * i));
                    _kickButtons[i].SetImage(
                        Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "partykick.png"),
                        Button.ControlState.Normal);
                    _kickButtons[i].SetImage(
                        Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "partykick.png"),
                        Button.ControlState.Hovered);
                    _kickButtons[i].SetImage(
                        Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "partykick.png"),
                        Button.ControlState.Clicked);
                    _kickButtons[i].Clicked += kick_Clicked;
                    if (i < Globals.Me.Party.Count)
                    {
                        _kickButtons[i].SetToolTipText(Strings.Get("parties", "kick",
                            Globals.Entities[Globals.Me.Party[i]].MyName));
                    }
                    else
                    {
                        _kickButtons[i].SetToolTipText("");
                    }
                    _kickButtons[i].Hide();

                    //Only show the kick buttons if its you or you are the party leader
                    if (i < Globals.Me.Party.Count)
                    {
                        if (Globals.Me.Party[0] == Globals.Me.MyIndex)
                        {
                            _kickButtons[i].Show();
                        }
                    }
                }
            }

            _leaveButton = new Button(_partyWindow);
            _leaveButton.SetBounds(_partyWindow.Width - 50, _partyWindow.Height - 68, 34, 34);
            _leaveButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "partyleave.png"),
                Button.ControlState.Normal);
            _leaveButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "partyleave.png"),
                Button.ControlState.Hovered);
            _leaveButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "partyleave.png"),
                Button.ControlState.Clicked);
            _leaveButton.SetToolTipText(Strings.Get("parties", "leave"));
            _leaveButton.Clicked += leave_Clicked;
        }

        //Methods
        public void Update()
        {
            if (_partyWindow.IsHidden)
            {
                return;
            }

            _leader.Hide();
            _leaveButton.Hide();
            for (int i = 0; i < 4; i++)
            {
                _barContainer[i].Hide();
                _lblnames[i].Text = "";
                if (i > 0) _kickButtons[i].Hide();
            }
            if (Globals.Me.IsInParty())
            {
                _leader.Show();
                _leaveButton.Show();

                for (int i = 0; i < 4; i++)
                {
                    if (i < Globals.Me.Party.Count)
                    {
                        var partyMember = Globals.Entities[Globals.Me.Party[i]];
                        _barContainer[i].Show();
                        _lblnames[i].Text = partyMember.MyName;

                        var vitalHP = partyMember.Vital[(int) Vitals.Health];
                        var vitalMaxHP = partyMember.MaxVital[(int) Vitals.Health];
                        var ratioHP = ((float) vitalHP) / ((float) vitalMaxHP);
                        ratioHP = Math.Min(1, Math.Max(0, ratioHP));
                        _bar[i].SetSize(Convert.ToInt32(ratioHP * 183), 25);
                        if (i > 0) _kickButtons[i].Hide();

                        //Only show the kick buttons if its you or you are the party leader
                        if (Globals.Me.Party[0] == Globals.Me.MyIndex && i > 0)
                        {
                            _kickButtons[i].Show();
                            _kickButtons[i].SetToolTipText(Strings.Get("parties", "kick",
                                Globals.Entities[Globals.Me.Party[i]].MyName));
                        }
                    }
                    else
                    {
                        if (i > 0) _kickButtons[i].SetToolTipText("");
                        _lblnames[i].Text = "";
                        _bar[i].SetSize(0, 25);
                        _barContainer[i].Hide();
                    }
                }
            }
        }

        public void Show()
        {
            _partyWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !_partyWindow.IsHidden;
        }

        public void Hide()
        {
            _partyWindow.IsHidden = true;
        }

        //Input Handlers
        void kick_Clicked(Base sender, ClickedEventArgs arguments)
        {
            for (int i = 1; i < Globals.Me.Party.Count; i++)
            {
                if (_kickButtons[i] == sender)
                {
                    PacketSender.SendPartyKick(i);
                    return;
                }
            }
        }

        void leave_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.Party.Count > 0)
            {
                PacketSender.SendPartyLeave();
            }
        }
    }
}