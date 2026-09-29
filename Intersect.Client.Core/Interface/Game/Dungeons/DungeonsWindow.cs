using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Dungeons;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Network.Packets.Server;
using Intersect.Framework.Core.GameObjects.Titles;

namespace Intersect.Client.Interface.Game.Dungeons;

/// <summary>
/// Lists every dungeon defined in the editor with the player's personal records, and lets them start a run.
/// </summary>
public partial class DungeonsWindow
{
    private const string FontName = "sourcesansproblack";

    private readonly Base mParent;

    private WindowControl mWindow;

    private ListBox mDungeonList;

    private Label mTitleLabel;

    private ScrollControl mDetailsArea;

    private RichLabel mDetailsLabel;

    private Label mDetailsTemplateLabel;

    private Button mEnterButton;

    private DungeonDescriptor? mSelected;

    private Guid mLeaderboardDungeonId = Guid.Empty;

    private DungeonLeaderboardEntryPacket[] mLeaderboardEntries = [];

    public DungeonsWindow(Canvas gameCanvas)
    {
        mParent = gameCanvas;

        GenerateControls();
    }

    private void GenerateControls()
    {
        mWindow = new WindowControl(mParent, Strings.Dungeons.Title, false, "DungeonsWindow")
        {
            IsHidden = true,
        };
        mWindow.SetSize(520, 360);
        mWindow.SetPosition(200, 120);
        mWindow.DisableResizing();

        mDungeonList = new ListBox(mWindow, "DungeonList")
        {
            FontName = FontName,
            FontSize = 10,
        };
        mDungeonList.SetBounds(8, 32, 190, 316);
        mDungeonList.EnableScroll(false, true);
        mDungeonList.RowSelected += DungeonList_RowSelected;

        mTitleLabel = new Label(mWindow, "DungeonTitle")
        {
            FontName = FontName,
            FontSize = 14,
        };
        mTitleLabel.SetBounds(206, 32, 306, 24);

        mDetailsArea = new ScrollControl(mWindow, "DungeonDetails");
        mDetailsArea.SetBounds(206, 60, 306, 250);
        mDetailsArea.EnableScroll(false, true);

        mDetailsTemplateLabel = new Label(mWindow, "DungeonDetailsTemplate")
        {
            FontName = FontName,
            FontSize = 10,
            IsHidden = true,
        };

        mDetailsLabel = new RichLabel(mDetailsArea);

        mEnterButton = new Button(mWindow, "EnterButton")
        {
            FontName = FontName,
            FontSize = 10,
        };
        mEnterButton.SetText(Strings.Dungeons.Enter);
        mEnterButton.SetBounds(206, 316, 306, 30);
        mEnterButton.Clicked += EnterButton_Clicked;

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Show() => mWindow.Show();

    public void Hide() => mWindow.Hide();

    public void Update()
    {
        // Cooldowns and "in dungeon" state change over time: keep the button in sync while the window is open.
        if (IsVisible && mSelected != null)
        {
            UpdateEnterButton();
        }
    }

    public void UpdateList()
    {
        var previouslySelected = mSelected?.Id ?? Guid.Empty;
        mDungeonList.RemoveAllRows();

        var dungeons = DungeonDescriptor.Lookup.Values
            .OfType<DungeonDescriptor>()
            .OrderBy(dungeon => dungeon.Folder)
            .ThenBy(dungeon => dungeon.RequiredLevel)
            .ThenBy(dungeon => dungeon.Name)
            .ToList();

        ListBoxRow? rowToSelect = null;
        foreach (var dungeon in dungeons)
        {
            var row = mDungeonList.AddRow(dungeon.Name, name: null, userData: dungeon.Id);
            row.SetTextColor(GetAvailabilityColor(dungeon));
            if (dungeon.Id == previouslySelected)
            {
                rowToSelect = row;
            }
        }

        if (rowToSelect != null)
        {
            rowToSelect.IsSelected = true;
        }
        else
        {
            mSelected = dungeons.FirstOrDefault();
        }

        if (mSelected != null)
        {
            PacketSender.SendRequestDungeonLeaderboard(mSelected.Id);
        }

        UpdateDetails();
    }

    private void DungeonList_RowSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        if (arguments.SelectedItem?.UserData is not Guid dungeonId)
        {
            return;
        }

        mSelected = DungeonDescriptor.Get(dungeonId);
        UpdateDetails();

        if (mSelected != null)
        {
            PacketSender.SendRequestDungeonLeaderboard(mSelected.Id);
        }
    }

    /// <summary>Called by <see cref="MenuContainer"/> when the server answers a leaderboard request.</summary>
    public void SetLeaderboard(DungeonLeaderboardPacket packet)
    {
        mLeaderboardDungeonId = packet.DungeonId;
        mLeaderboardEntries = packet.Entries;

        if (mSelected?.Id == mLeaderboardDungeonId)
        {
            UpdateDetails();
        }
    }

    private void EnterButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (mSelected == null)
        {
            return;
        }

        PacketSender.SendEnterDungeon(mSelected.Id);
        Hide();
    }

    private void UpdateDetails()
    {
        mDetailsLabel.ClearText();

        if (mSelected == null)
        {
            mTitleLabel.SetText(string.Empty);
            mDetailsLabel.AddText(Strings.Dungeons.NoDungeons, mDetailsTemplateLabel);
            mEnterButton.IsDisabled = true;
            FinishDetails();

            return;
        }

        var dungeon = mSelected;
        mTitleLabel.SetText(dungeon.Name);

        if (!string.IsNullOrWhiteSpace(dungeon.Description))
        {
            mDetailsLabel.AddText(dungeon.Description, mDetailsTemplateLabel);
            mDetailsLabel.AddLineBreak();
            mDetailsLabel.AddLineBreak();
        }

        string access = dungeon.AccessType switch
        {
            DungeonAccessType.Solo => Strings.Dungeons.AccessSolo.ToString(),
            DungeonAccessType.Guild => Strings.Dungeons.AccessGuild.ToString(),
            _ => dungeon.MaxPlayers > 0
                ? Strings.Dungeons.AccessParty.ToString(Math.Max(1, dungeon.MinPlayers), dungeon.MaxPlayers)
                : Strings.Dungeons.AccessPartyNoMax.ToString(Math.Max(1, dungeon.MinPlayers)),
        };
        AddLine(Strings.Dungeons.AccessLabel, access);
        AddLine(Strings.Dungeons.LevelLabel, dungeon.RequiredLevel.ToString());
        AddLine(Strings.Dungeons.TimeLimitLabel, dungeon.TimeLimitSeconds > 0 ? FormatTime(dungeon.TimeLimitSeconds * 1000L) : Strings.Dungeons.None);
        AddLine(Strings.Dungeons.LivesLabel, dungeon.Lives > 0 ? dungeon.Lives.ToString() : Strings.Dungeons.Unlimited);
        AddLine(Strings.Dungeons.CooldownLabel, dungeon.CooldownSeconds > 0 ? FormatTime(dungeon.CooldownSeconds * 1000L) : Strings.Dungeons.None);

        mDetailsLabel.AddLineBreak();
        mDetailsLabel.AddText(Strings.Dungeons.RewardsHeader, mDetailsTemplateLabel);
        mDetailsLabel.AddLineBreak();
        var anyReward = false;
        foreach (var reward in dungeon.Rewards)
        {
            if (ItemDescriptor.Get(reward.ItemId) is not { } item)
            {
                continue;
            }

            anyReward = true;
            mDetailsLabel.AddText(Strings.Dungeons.RewardItem.ToString(item.Name, reward.Quantity), mDetailsTemplateLabel);
            mDetailsLabel.AddLineBreak();
        }

        if (dungeon.RewardExperience > 0)
        {
            anyReward = true;
            mDetailsLabel.AddText(Strings.Dungeons.RewardExperience.ToString(dungeon.RewardExperience), mDetailsTemplateLabel);
            mDetailsLabel.AddLineBreak();
        }

        if (dungeon.RewardTitleId != Guid.Empty && TitleDescriptor.Get(dungeon.RewardTitleId) is { } title)
        {
            anyReward = true;
            mDetailsLabel.AddText(Strings.Dungeons.RewardTitle.ToString(title.Name), mDetailsTemplateLabel);
            mDetailsLabel.AddLineBreak();
        }

        if (!anyReward)
        {
            mDetailsLabel.AddText(Strings.Dungeons.None, mDetailsTemplateLabel);
            mDetailsLabel.AddLineBreak();
        }

        mDetailsLabel.AddLineBreak();
        if (Globals.Me != null && Globals.Me.DungeonRecords.TryGetValue(dungeon.Id, out var record) && record.Completions > 0)
        {
            mDetailsLabel.AddText(
                Strings.Dungeons.Record.ToString(record.Completions, FormatTime(record.BestTimeMs)),
                mDetailsTemplateLabel
            );
        }
        else
        {
            mDetailsLabel.AddText(Strings.Dungeons.NeverCompleted, mDetailsTemplateLabel);
        }

        if (mLeaderboardDungeonId == dungeon.Id && mLeaderboardEntries.Length > 0)
        {
            mDetailsLabel.AddLineBreak();
            mDetailsLabel.AddLineBreak();
            mDetailsLabel.AddText(Strings.Dungeons.LeaderboardHeader, mDetailsTemplateLabel);
            mDetailsLabel.AddLineBreak();
            for (var rank = 0; rank < mLeaderboardEntries.Length; rank++)
            {
                var entry = mLeaderboardEntries[rank];
                mDetailsLabel.AddText(
                    Strings.Dungeons.LeaderboardEntry.ToString(rank + 1, entry.PlayerName, FormatTime(entry.TimeMs)),
                    mDetailsTemplateLabel
                );
                mDetailsLabel.AddLineBreak();
            }
        }

        UpdateEnterButton();
        FinishDetails();
    }

    private void AddLine(string label, string value)
    {
        mDetailsLabel.AddText($"{label} {value}", mDetailsTemplateLabel);
        mDetailsLabel.AddLineBreak();
    }

    private void FinishDetails()
    {
        mDetailsLabel.Width = mDetailsArea.Width - mDetailsArea.VerticalScrollBar.Width;
        mDetailsLabel.SizeToChildren(false, true);
    }

    private void UpdateEnterButton()
    {
        if (mSelected == null || Globals.Me == null)
        {
            mEnterButton.IsDisabled = true;

            return;
        }

        if (Globals.ActiveDungeon != null)
        {
            mEnterButton.SetText(Strings.Dungeons.AlreadyInside);
            mEnterButton.IsDisabled = true;

            return;
        }

        if (Globals.Me.Level < mSelected.RequiredLevel)
        {
            mEnterButton.SetText(Strings.Dungeons.RequiresLevel.ToString(mSelected.RequiredLevel));
            mEnterButton.IsDisabled = true;

            return;
        }

        var cooldown = GetCooldownRemainingMs(mSelected);
        if (cooldown > 0)
        {
            mEnterButton.SetText(Strings.Dungeons.AvailableIn.ToString(FormatTime(cooldown)));
            mEnterButton.IsDisabled = true;

            return;
        }

        mEnterButton.SetText(Strings.Dungeons.Enter);
        mEnterButton.IsDisabled = false;
    }

    private static long GetCooldownRemainingMs(DungeonDescriptor dungeon)
    {
        if (Globals.Me == null || !Globals.Me.DungeonRecords.TryGetValue(dungeon.Id, out var record))
        {
            return 0;
        }

        // The server sent the remaining cooldown at a point in time; count it down locally since then.
        var elapsedSinceUpdate = Timing.Global.Milliseconds - Globals.Me.DungeonRecordsReceivedAt;

        return Math.Max(0, record.CooldownRemainingMs - elapsedSinceUpdate);
    }

    private static Color GetAvailabilityColor(DungeonDescriptor dungeon)
    {
        if (Globals.Me == null)
        {
            return Color.White;
        }

        if (Globals.Me.Level < dungeon.RequiredLevel)
        {
            return Color.FromArgb(255, 150, 150, 150);
        }

        return GetCooldownRemainingMs(dungeon) > 0 ? Color.FromArgb(255, 255, 170, 60) : Color.FromArgb(255, 120, 220, 120);
    }

    public static string FormatTime(long ms)
    {
        var time = TimeSpan.FromMilliseconds(Math.Max(0, ms));

        return time.TotalHours >= 1 ? time.ToString(@"h\:mm\:ss") : time.ToString(@"m\:ss");
    }
}
