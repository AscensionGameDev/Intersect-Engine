using System;
using System.Collections.Generic;
using Intersect.Enums;
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
            _partyWindow = new WindowControl(_gameCanvas, Strings.Get("parties", "title"),false, "PartyWindow");
            _partyWindow.DisableResizing();

            //Add the icon representing party leader (ALWAYS member 1 in the party list)
            _leader = new ImagePanel(_partyWindow, "LeaderIcon");
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
                _lblnames.Add(new Label(_partyWindow,"MemberName" + i));
                if (i < Globals.Me.Party.Count)
                {
                    _lblnames[i].Text = Globals.Entities[Globals.Me.Party[i]].MyName;
                }
                else
                {
                    _lblnames[i].Text = "";
                }

                //Health bars
                _barContainer.Add(new ImagePanel(_partyWindow,"HealthBarContainer" + i));
                _barContainer[i].Hide();
                if (i < Globals.Me.Party.Count)
                {
                    _barContainer[i].Show();
                }
                _bar.Add(new ImagePanel(_barContainer[i],"HealthBar" + i));

                if (i == 0)
                {
                    _kickButtons.Add(null);
                }
                else
                {
                    _kickButtons.Add(new Button(_partyWindow, "KickButton" + i));
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

            _leaveButton = new Button(_partyWindow,"LeavePartyButton");
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
                        _bar[i].SetSize(Convert.ToInt32(ratioHP * _barContainer[i].Width), _barContainer[i].Height);
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
                        _bar[i].SetSize(0, _barContainer[i].Height);
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