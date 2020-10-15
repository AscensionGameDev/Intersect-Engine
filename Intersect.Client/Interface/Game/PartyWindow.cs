using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Enums;

namespace Intersect.Client.Interface.Game
{

    public class PartyWindow
    {

        private List<ImagePanel> mHpBar = new List<ImagePanel>();

        private List<ImagePanel> mHpBarContainer = new List<ImagePanel>();

        private List<Label> mHpLabel = new List<Label>();

        private List<Label> mHpValue = new List<Label>();

        private List<Button> mKickButtons = new List<Button>();

        private List<Label> mLblnames = new List<Label>();

        private ImagePanel mLeader;

        private Label mLeaderText;

        private Button mLeaveButton;

        private List<ImagePanel> mMpBar = new List<ImagePanel>();

        private List<ImagePanel> mMpBarContainer = new List<ImagePanel>();

        private List<Label> mMpLabel = new List<Label>();

        private List<Label> mMpValue = new List<Label>();

        //Controls
        private WindowControl mPartyWindow;

        //Init
        public PartyWindow(Canvas gameCanvas)
        {
            mPartyWindow = new WindowControl(gameCanvas, Strings.Parties.title, false, "PartyWindow");
            mPartyWindow.DisableResizing();

            //Add the icon representing party leader (ALWAYS member 1 in the party list)
            mLeader = new ImagePanel(mPartyWindow, "LeaderIcon");
            mLeader.SetToolTipText(Strings.Parties.leadertip);
            mLeader.Hide();

            mLeaderText = new Label(mPartyWindow, "LeaderText");
            mLeaderText.SetTextColor(new Color(0, 0, 0, 0), Label.ControlState.Normal);
            mLeaderText.Text = Strings.Parties.leader;
            mLeaderText.Hide();

            if (Globals.Me.Party.Count > 0)
            {
                mLeader.Show();
                mLeaderText.Show();
            }

            mLblnames.Clear();
            mKickButtons.Clear();
            mHpBarContainer.Clear();
            mHpBar.Clear();

            for (var i = 0; i < Options.Party.MaximumMembers; i++)
            {
                //Labels
                mLblnames.Add(new Label(mPartyWindow, "MemberName" + i));
                if (i < Globals.Me.Party.Count)
                {
                    mLblnames[i].Text = Strings.Parties.name.ToString(
                        Globals.Me.Party[i].Name, Globals.Me.Party[i].Level
                    );
                }
                else
                {
                    mLblnames[i].Text = "";
                }

                //Health bars
                mHpBarContainer.Add(new ImagePanel(mPartyWindow, "HealthBarContainer" + i));
                mHpBarContainer[i].Hide();
                mHpLabel.Add(new Label(mPartyWindow, "HealthLabel" + i));
                mHpLabel[i].Hide();
                mHpLabel[i].SetTextColor(new Color(0, 0, 0, 0), Label.ControlState.Normal);
                mHpLabel[i].Text = Strings.Parties.vital0;
                mHpValue.Add(new Label(mPartyWindow, "HealthValue" + i));
                mHpValue[i].Hide();
                mHpValue[i].SetTextColor(new Color(0, 0, 0, 0), Label.ControlState.Normal);
                if (i < Globals.Me.Party.Count)
                {
                    mHpBarContainer[i].Show();
                }

                mHpBar.Add(new ImagePanel(mHpBarContainer[i], "HealthBar" + i));

                //Mana bars
                mMpBarContainer.Add(new ImagePanel(mPartyWindow, "ManaBarContainer" + i));
                mMpBarContainer[i].Hide();
                mMpBarContainer[i].RenderColor = new Color(0, 0, 0, 0);
                mMpLabel.Add(new Label(mPartyWindow, "ManaLabel" + i));
                mMpLabel[i].Hide();
                mMpLabel[i].SetTextColor(new Color(0, 0, 0, 0), Label.ControlState.Normal);
                mMpLabel[i].Text = Strings.Parties.vital1;
                mMpValue.Add(new Label(mPartyWindow, "ManaValue" + i));
                mMpValue[i].Hide();
                mMpValue[i].SetTextColor(new Color(0, 0, 0, 0), Label.ControlState.Normal);
                mMpBar.Add(new ImagePanel(mMpBarContainer[i], "ManaBar" + i));
                mMpBar[i].RenderColor = new Color(0, 0, 0, 0);

                if (i == 0)
                {
                    mKickButtons.Add(null);
                }
                else
                {
                    mKickButtons.Add(new Button(mPartyWindow, "KickButton" + i));
                    mKickButtons[i].Clicked += kick_Clicked;
                    mKickButtons[i].Text = Strings.Parties.kicklbl;
                    if (i < Globals.Me.Party.Count)
                    {
                        mKickButtons[i]
                            .SetToolTipText(
                                Strings.Parties.kick.ToString(Globals.Entities[Globals.Me.Party[i].Id].Name)
                            );
                    }
                    else
                    {
                        mKickButtons[i].SetToolTipText("");
                    }

                    mKickButtons[i].Hide();

                    //Only show the kick buttons if its you or you are the party leader
                    if (i < Globals.Me.Party.Count)
                    {
                        if (Globals.Me.Party[0].Id == Globals.Me.Id)
                        {
                            mKickButtons[i].Show();
                        }
                    }
                }
            }

            mLeaveButton = new Button(mPartyWindow, "LeavePartyButton");
            mLeaveButton.Text = Strings.Parties.leave;
            mLeaveButton.SetToolTipText(Strings.Parties.leavetip);
            mLeaveButton.Clicked += leave_Clicked;

            mPartyWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        //Methods
        public void Update()
        {
            if (mPartyWindow.IsHidden)
            {
                return;
            }

            mLeader.Hide();
            mLeaderText.Hide();
            mLeaveButton.Hide();
            for (var i = 0; i < Options.Instance.PartyOpts.MaximumMembers; i++)
            {
                mHpBarContainer[i].Hide();
                mHpLabel[i].Hide();
                mHpValue[i].Hide();
                mLblnames[i].Text = "";
                mMpBarContainer[i].Hide();
                mMpLabel[i].Hide();
                mMpValue[i].Hide();
                if (i > 0)
                {
                    mKickButtons[i].Hide();
                }
            }

            if (Globals.Me.IsInParty())
            {
                mLeader.Show();
                mLeaderText.Show();
                mLeaveButton.Show();

                for (var i = 0; i < Options.Instance.PartyOpts.MaximumMembers; i++)
                {
                    if (i < Globals.Me.Party.Count)
                    {
                        mHpLabel[i].Show();
                        mHpValue[i].Show();
                        mHpBarContainer[i].Show();

                        mMpLabel[i].Show();
                        mMpValue[i].Show();
                        mMpBarContainer[i].Show();

                        mLblnames[i].Text = Strings.Parties.name.ToString(
                            Globals.Me.Party[i].Name, Globals.Me.Party[i].Level
                        );

                        if (mHpBar[i].Texture != null)
                        {
                            var partyHpWidthRatio = 1f;
                            if (Globals.Me.Party[i].MaxVital[(int)Vitals.Health] > 0)
                            {
                                var vitalHp = Globals.Me.Party[i].Vital[(int)Vitals.Health];
                                var vitalMaxHp = Globals.Me.Party[i].MaxVital[(int)Vitals.Health];
                                var ratioHp = (float)vitalHp / (float)vitalMaxHp;
                                partyHpWidthRatio = Math.Min(1, Math.Max(0, ratioHp));
                            }

                            mHpBar[i]
                                .SetTextureRect(
                                    0, 0, Convert.ToInt32(mHpBar[i].Texture.GetWidth() * partyHpWidthRatio),
                                    mHpBar[i].Texture.GetHeight()
                                );

                            mHpBar[i]
                                .SetSize(
                                    Convert.ToInt32(partyHpWidthRatio * mHpBarContainer[i].Width), mHpBarContainer[i].Height
                                );
                        }

                        mHpValue[i].Text = Strings.Parties.vital0val.ToString(
                            Globals.Me.Party[i].Vital[(int) Vitals.Health],
                            Globals.Me.Party[i].MaxVital[(int) Vitals.Health]
                        );

                        if (mMpBar[i].Texture != null)
                        {
                            var partyMpWidthRatio = 1f;
                            if (Globals.Me.Party[i].MaxVital[(int)Vitals.Mana] > 0)
                            {
                                var vitalMp = Globals.Me.Party[i].Vital[(int)Vitals.Mana];
                                var vitalMaxMp = Globals.Me.Party[i].MaxVital[(int)Vitals.Mana];
                                var ratioMp = (float)vitalMp / (float)vitalMaxMp;
                                partyMpWidthRatio = Math.Min(1, Math.Max(0, ratioMp));
                            }

                            mMpBar[i]
                                .SetTextureRect(
                                    0, 0, Convert.ToInt32(mMpBar[i].Texture.GetWidth() * partyMpWidthRatio),
                                    mMpBar[i].Texture.GetHeight()
                                );

                            mMpBar[i]
                                .SetSize(
                                    Convert.ToInt32(partyMpWidthRatio * mMpBarContainer[i].Width), mMpBarContainer[i].Height
                                );
                        }

                        mMpValue[i].Text = Strings.Parties.vital1val.ToString(
                            Globals.Me.Party[i].Vital[(int) Vitals.Mana],
                            Globals.Me.Party[i].MaxVital[(int) Vitals.Mana]
                        );

                        if (i > 0)
                        {
                            mKickButtons[i].Hide();
                        }

                        //Only show the kick buttons if its you or you are the party leader
                        if (Globals.Me.Party[0].Id == Globals.Me.Id && i > 0)
                        {
                            mKickButtons[i].Show();
                            mKickButtons[i].SetToolTipText(Strings.Parties.kick.ToString(Globals.Me.Party[i].Name));
                        }
                    }
                    else
                    {
                        if (i > 0)
                        {
                            mKickButtons[i].SetToolTipText("");
                        }

                        mLblnames[i].Text = "";
                        mHpBar[i].SetSize(0, mHpBarContainer[i].Height);
                        mHpBarContainer[i].Hide();
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
            for (var i = 1; i < Globals.Me.Party.Count; i++)
            {
                if (mKickButtons[i] == sender)
                {
                    PacketSender.SendPartyKick(Globals.Me.Party[i].Id);

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
