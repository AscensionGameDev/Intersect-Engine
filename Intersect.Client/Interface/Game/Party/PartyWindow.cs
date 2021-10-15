using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
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
        private Canvas mCanvas;

        private WindowControl mPartyWindow;

        private Button mLeaveButton;

        private Button mInviteButton;

        private ScrollControl mMemberList;

        private ImagePanel mMemberAnchor;

        private List<PartyMemberRow> mMembers;

        
        private int mPrePartyCount = 0;

        public PartyWindow(Canvas gameCanvas)
        {
            mCanvas = gameCanvas;

            GenerateControls();
            GenerateMemberList();
        }

        private void GenerateControls()
        {
            mPartyWindow = new WindowControl(mCanvas, Strings.Parties.Title, false, "PartyWindow");
            mInviteButton = new Button(mPartyWindow, "InviteButton");
            mLeaveButton = new Button(mPartyWindow, "LeaveButton");
            mMemberList = new ScrollControl(mPartyWindow, "Memberlist");
            mMemberAnchor = new ImagePanel(mMemberList, "MemberAnchor");

            mInviteButton.SetText(Strings.Parties.InviteButton);
            mLeaveButton.SetText(Strings.Parties.LeaveButton);

            mPartyWindow.DisableResizing();
            mMemberList.EnableScroll(false, true);

            mInviteButton.Clicked += MInviteButton_Clicked;
            mLeaveButton.Clicked += MLeaveButton_Clicked;

            mPartyWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        private void GenerateMemberList()
        {
            mMembers = new List<PartyMemberRow>();

            var pos = 0;
            for (var member = 0; member < Options.Party.MaximumMembers; member++)
            {
                var control = new PartyMemberRow(mMemberList, member);
                control.SetPosition(mMemberAnchor.X, mMemberAnchor.Y + (pos * control.Height));

                mMembers.Add(control);
                pos++;
            }
        }

        private void MLeaveButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.IsInParty())
            {
                PacketSender.SendPartyLeave();
            }
        }

        private void MInviteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.IsPartyLeader)
            {
                // TODO: Add invite option
            }
        }

        public void Update()
        {
            if (mPartyWindow.IsHidden)
            {
                return;
            }

            
            foreach (var member in mMembers)
            {
                if (Globals.Me.IsInParty())
                {
                    if (member.Index < Globals.Me.Party.Count)
                    {
                        member.Show();
                        member.Update();
                    }
                    else if (member.IsVisible)
                    {
                        member.Hide();
                    }
                }
                else if (member.IsVisible)
                {
                    member.Hide();
                }    
            }

        }

        public void Show() => mPartyWindow.Show();

        public bool IsVisible => !mPartyWindow.IsHidden;

        public void Hide() => mPartyWindow.Hide();

    }

}
