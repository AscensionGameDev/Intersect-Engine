using System;
using System.Collections.Generic;
using Intersect.Enums;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class PartyWindow
    {
        private List<ImagePanel> mBar = new List<ImagePanel>();
        private List<ImagePanel> mBarContainer = new List<ImagePanel>();
        private List<Button> mKickButtons = new List<Button>();

        private List<Label> mLblnames = new List<Label>();
        private ImagePanel mLeader;

        private Button mLeaveButton;

        //Controls
        private WindowControl mPartyWindow;

        //Init
        public PartyWindow(Canvas gameCanvas)
        {
            mPartyWindow = new WindowControl(gameCanvas, Strings.Parties.title, false, "PartyWindow");
            mPartyWindow.DisableResizing();

            //Add the icon representing party leader (ALWAYS member 1 in the party list)
            mLeader = new ImagePanel(mPartyWindow, "LeaderIcon");
            mLeader.SetToolTipText(Strings.Parties.leader);
            mLeader.Hide();
            if (Globals.Me.Party.Count > 0)
            {
                mLeader.Show();
            }

            mLblnames.Clear();
            mKickButtons.Clear();
            mBarContainer.Clear();
            mBar.Clear();

            for (int i = 0; i < 4; i++)
            {
                //Labels
                mLblnames.Add(new Label(mPartyWindow, "MemberName" + i));
                if (i < Globals.Me.Party.Count)
                {
                    mLblnames[i].Text = Globals.Me.Party[i].Name;
                }
                else
                {
                    mLblnames[i].Text = "";
                }

                //Health bars
                mBarContainer.Add(new ImagePanel(mPartyWindow, "HealthBarContainer" + i));
                mBarContainer[i].Hide();
                if (i < Globals.Me.Party.Count)
                {
                    mBarContainer[i].Show();
                }
                mBar.Add(new ImagePanel(mBarContainer[i], "HealthBar" + i));

                if (i == 0)
                {
                    mKickButtons.Add(null);
                }
                else
                {
                    mKickButtons.Add(new Button(mPartyWindow, "KickButton" + i));
                    mKickButtons[i].Clicked += kick_Clicked;
                    if (i < Globals.Me.Party.Count)
                    {
                        mKickButtons[i].SetToolTipText(Strings.Parties.kick.ToString(
                            Globals.Entities[Globals.Me.Party[i]].MyName));
                    }
                    else
                    {
                        mKickButtons[i].SetToolTipText("");
                    }
                    mKickButtons[i].Hide();

                    //Only show the kick buttons if its you or you are the party leader
                    if (i < Globals.Me.Party.Count)
                    {
                        if (Globals.Me.Party[0].Index == Globals.Me.MyIndex)
                        {
                            mKickButtons[i].Show();
                        }
                    }
                }
            }

            mLeaveButton = new Button(mPartyWindow, "LeavePartyButton");
            mLeaveButton.SetToolTipText(Strings.Parties.leave);
            mLeaveButton.Clicked += leave_Clicked;

            mPartyWindow.LoadJsonUi(GameContentManager.UI.InGame);
        }

        //Methods
        public void Update()
        {
            if (mPartyWindow.IsHidden)
            {
                return;
            }

            mLeader.Hide();
            mLeaveButton.Hide();
            for (int i = 0; i < 4; i++)
            {
                mBarContainer[i].Hide();
                mLblnames[i].Text = "";
                if (i > 0) mKickButtons[i].Hide();
            }
            if (Globals.Me.IsInParty())
            {
                mLeader.Show();
                mLeaveButton.Show();

                for (int i = 0; i < 4; i++)
                {
                    if (i < Globals.Me.Party.Count)
                    {
                        mBarContainer[i].Show();
                        mLblnames[i].Text = Globals.Me.Party[i].Name;

                        var vitalHp = Globals.Me.Party[i].Vital[(int)Vitals.Health];
                        var vitalMaxHp = Globals.Me.Party[i].MaxVital[(int)Vitals.Health];
                        var ratioHp = ((float)vitalHp) / ((float)vitalMaxHp);
                        ratioHp = Math.Min(1, Math.Max(0, ratioHp));
                        mBar[i].SetSize(Convert.ToInt32(ratioHp * mBarContainer[i].Width), mBarContainer[i].Height);
                        if (i > 0) mKickButtons[i].Hide();

                        //Only show the kick buttons if its you or you are the party leader
                        if (Globals.Me.Party[0].Index == Globals.Me.MyIndex && i > 0)
                        {
                            mKickButtons[i].Show();
                            mKickButtons[i].SetToolTipText(Strings.Parties.kick.ToString(Globals.Me.Party[i].Name));
                        }
                    }
                    else
                    {
                        if (i > 0) mKickButtons[i].SetToolTipText("");
                        mLblnames[i].Text = "";
                        mBar[i].SetSize(0, mBarContainer[i].Height);
                        mBarContainer[i].Hide();
                    }
                }
            }
        }

        public void Show()
        {
            mPartyWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mPartyWindow.IsHidden;
        }

        public void Hide()
        {
            mPartyWindow.IsHidden = true;
        }

        //Input Handlers
        void kick_Clicked(Base sender, ClickedEventArgs arguments)
        {
            for (int i = 1; i < Globals.Me.Party.Count; i++)
            {
                if (mKickButtons[i] == sender)
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