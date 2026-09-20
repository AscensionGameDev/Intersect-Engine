using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Framework.Core.GameObjects.Dungeons;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Titles;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Interface.Game.Dungeons;

/// <summary>
/// End-of-run screen: result, time (with best-time comparison), the loot handed out, and a "Return" button.
/// </summary>
public partial class DungeonResultWindow
{
    private const string FontName = "sourcesansproblack";

    private static readonly Color SuccessColor = Color.FromArgb(255, 120, 220, 120);

    private static readonly Color FailureColor = Color.FromArgb(255, 255, 90, 90);

    private static readonly Color RecordColor = Color.FromArgb(255, 255, 215, 0);

    private readonly WindowControl mWindow;

    private readonly Label mHeadline;

    private readonly ScrollControl mBodyArea;

    private readonly RichLabel mBody;

    private readonly Label mBodyTemplate;

    private readonly Button mReturnButton;

    public DungeonResultWindow(Canvas gameCanvas)
    {
        mWindow = new WindowControl(gameCanvas, Strings.Dungeons.ResultTitle, false, "DungeonResultWindow")
        {
            IsHidden = true,
            Alignment = [Alignments.Center],
        };
        mWindow.SetSize(380, 300);
        mWindow.DisableResizing();

        mHeadline = new Label(mWindow, "ResultHeadline")
        {
            FontName = FontName,
            FontSize = 16,
            AutoSizeToContents = false,
            TextAlign = Pos.Center,
        };
        mHeadline.SetBounds(10, 30, 360, 30);

        mBodyArea = new ScrollControl(mWindow, "ResultBody");
        mBodyArea.SetBounds(16, 66, 348, 180);
        mBodyArea.EnableScroll(false, true);

        mBodyTemplate = new Label(mWindow, "ResultBodyTemplate")
        {
            FontName = FontName,
            FontSize = 10,
            IsHidden = true,
        };

        mBody = new RichLabel(mBodyArea);

        mReturnButton = new Button(mWindow, "ReturnButton")
        {
            FontName = FontName,
            FontSize = 10,
        };
        mReturnButton.SetText(Strings.Dungeons.ReturnNow);
        mReturnButton.SetBounds(100, 256, 180, 30);
        mReturnButton.Clicked += ReturnButton_Clicked;

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Hide() => mWindow.Hide();

    private void ReturnButton_Clicked(Base sender, MouseButtonState arguments)
    {
        PacketSender.SendLeaveDungeon();
        Hide();
    }

    public void Show(DungeonResultPacket result)
    {
        var dungeonName = DungeonDescriptor.GetName(result.DungeonId);
        mBody.ClearText();

        if (result.Completed)
        {
            mHeadline.SetText(Strings.Dungeons.ResultCompleted.ToString(dungeonName));
            mHeadline.SetTextColor(SuccessColor, ComponentState.Normal);

            mBody.AddText(Strings.Dungeons.ResultTime.ToString(DungeonsWindow.FormatTime(result.ElapsedMs)), mBodyTemplate);
            mBody.AddLineBreak();
            if (result.PreviousBestMs <= 0)
            {
                mBody.AddText(Strings.Dungeons.ResultFirstClear, RecordColor);
            }
            else if (result.ElapsedMs < result.PreviousBestMs)
            {
                mBody.AddText(Strings.Dungeons.ResultNewRecord.ToString(DungeonsWindow.FormatTime(result.PreviousBestMs)), RecordColor);
            }
            else
            {
                mBody.AddText(Strings.Dungeons.ResultBest.ToString(DungeonsWindow.FormatTime(result.PreviousBestMs)), mBodyTemplate);
            }

            mBody.AddLineBreak();
            mBody.AddLineBreak();
            mBody.AddText(Strings.Dungeons.RewardsHeader, mBodyTemplate);
            mBody.AddLineBreak();

            var anyReward = false;
            foreach (var reward in result.Rewards)
            {
                if (ItemDescriptor.Get(reward.ItemId) is not { } item)
                {
                    continue;
                }

                anyReward = true;
                mBody.AddText(Strings.Dungeons.RewardItem.ToString(item.Name, reward.Quantity), mBodyTemplate);
                mBody.AddLineBreak();
            }

            if (result.RewardExperience > 0)
            {
                anyReward = true;
                mBody.AddText(Strings.Dungeons.RewardExperience.ToString(result.RewardExperience), mBodyTemplate);
                mBody.AddLineBreak();
            }

            if (result.RewardTitleId != Guid.Empty && TitleDescriptor.Get(result.RewardTitleId) is { } title)
            {
                anyReward = true;
                mBody.AddText(Strings.Dungeons.RewardTitle.ToString(title.Name), mBodyTemplate);
                mBody.AddLineBreak();
            }

            if (!anyReward)
            {
                mBody.AddText(Strings.Dungeons.None, mBodyTemplate);
                mBody.AddLineBreak();
            }
        }
        else
        {
            mHeadline.SetText(Strings.Dungeons.ResultFailed.ToString(dungeonName));
            mHeadline.SetTextColor(FailureColor, ComponentState.Normal);
            mBody.AddText(Strings.Dungeons.ResultReason.ToString(result.FailReason), mBodyTemplate);
            mBody.AddLineBreak();
            mBody.AddText(Strings.Dungeons.ResultTime.ToString(DungeonsWindow.FormatTime(result.ElapsedMs)), mBodyTemplate);
            mBody.AddLineBreak();
        }

        mBody.AddLineBreak();
        mBody.AddText(Strings.Dungeons.ResultAutoExit.ToString(result.ExitDelaySeconds), mBodyTemplate);

        mBody.Width = mBodyArea.Width - mBodyArea.VerticalScrollBar.Width;
        mBody.SizeToChildren(false, true);

        mWindow.Show();
        mWindow.BringToFront();
    }
}
