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
/// Lists every dungeon defined in the editor, lets the player pick a difficulty (Easy / Normal / Hard) and shows what
/// that difficulty changes: monster level, rewards, personal record and leaderboard - then starts the run.
/// </summary>
public partial class DungeonsWindow
{
    private const string FontName = "sourcesansproblack";

    private const int WindowWidth = 700;

    private const int WindowHeight = 470;

    private static readonly Color EasyColor = Color.FromArgb(255, 120, 220, 120);

    private static readonly Color NormalColor = Color.FromArgb(255, 255, 185, 70);

    private static readonly Color HardColor = Color.FromArgb(255, 255, 95, 95);

    private static readonly Color DimColor = Color.FromArgb(255, 150, 150, 150);

    private static readonly Color HeaderColor = Color.FromArgb(255, 255, 220, 120);

    private static readonly Color WhiteColor = Color.FromArgb(255, 235, 235, 235);

    private readonly Base mParent;

    private WindowControl mWindow;

    // The dungeon list is a column of buttons in a scroll area (same dark style as the difficulty buttons) instead of
    // the standard list box, whose background is plain white.
    private ScrollControl mDungeonScroll;

    private readonly List<Button> mDungeonRows = [];

    private Label mTitleLabel;

    private ScrollControl mDetailsArea;

    private RichLabel mDetailsLabel;

    private Label mDetailsTemplateLabel;

    private Button mEnterButton;

    private readonly Button[] mDifficultyButtons = new Button[DungeonDifficulties.Count];

    private byte mDifficulty = DungeonDifficulties.Easy;

    private DungeonDescriptor? mSelected;

    private Guid mLeaderboardDungeonId = Guid.Empty;

    private byte mLeaderboardDifficulty;

    private DungeonLeaderboardEntryPacket[] mLeaderboardEntries = [];

    public DungeonsWindow(Canvas gameCanvas)
    {
        mParent = gameCanvas;

        GenerateControls();
    }

    private Base mRightPanel;

    private Base mButtonRow;

    private void GenerateControls()
    {
        mWindow = new WindowControl(mParent, Strings.Dungeons.Title, false, "DungeonsWindow")
        {
            IsHidden = true,
        };
        mWindow.SetSize(WindowWidth, WindowHeight);
        mWindow.SetPosition(180, 90);
        mWindow.DisableResizing();

        mDungeonScroll = new ScrollControl(mWindow, "DungeonListScroll");
        mDungeonScroll.EnableScroll(false, true);
        mDungeonScroll.SizeChanged += (_, _) => LayoutDungeonRows();

        // Everything on the right is docked inside this panel, so it always fits the window's real client area.
        mRightPanel = new Base(mWindow, "DungeonRightPanel");

        mTitleLabel = new Label(mRightPanel, "DungeonTitle")
        {
            FontName = FontName,
            FontSize = 14,
        };

        mButtonRow = new Base(mRightPanel, "DungeonDifficultyRow");
        for (var difficulty = 0; difficulty < DungeonDifficulties.Count; difficulty++)
        {
            var level = (byte)difficulty;
            var button = new Button(mButtonRow, $"DifficultyButton{difficulty}")
            {
                FontName = FontName,
                FontSize = 10,
            };
            button.SetText($"{GetDifficultyName(level)}  {Strings.Dungeons.LevelShort.ToString(DungeonDifficulties.MonsterLevel(level))}");
            button.Clicked += (_, _) => SelectDifficulty(level);
            mDifficultyButtons[difficulty] = button;
        }

        mButtonRow.SizeChanged += (_, _) => LayoutDifficultyButtons();

        mEnterButton = new Button(mRightPanel, "EnterButton")
        {
            FontName = FontName,
            FontSize = 10,
        };
        mEnterButton.SetText(Strings.Dungeons.Enter);
        mEnterButton.Clicked += EnterButton_Clicked;

        mDetailsArea = new ScrollControl(mRightPanel, "DungeonDetails");
        mDetailsArea.EnableScroll(false, true);

        mDetailsTemplateLabel = new Label(mRightPanel, "DungeonDetailsTemplate")
        {
            FontName = FontName,
            FontSize = 10,
            IsHidden = true,
        };

        mDetailsLabel = new RichLabel(mDetailsArea);

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        // Layout is set after the JSON so it always wins over positions saved by an older version of this window.
        // Docking keeps every control inside the real client area of the skin: list on the left, the right panel
        // fills the rest; inside it title and difficulty buttons stick to the top, the Enter button to the bottom
        // and the details take whatever is left.
        // The window skin (window_blue_dungeonswindow.png) is generated at 700x470, so the size is set again here: the
        // saved layout JSON still says 598x414.
        mWindow.SetSize(WindowWidth, WindowHeight);

        mDungeonScroll.Dock = Pos.Left;
        mDungeonScroll.Width = 200;
        mDungeonScroll.Margin = new Margin(18, 6, 8, 22);

        mRightPanel.Dock = Pos.Fill;
        mRightPanel.Padding = new Padding(0, 6, 22, 22);

        mTitleLabel.Dock = Pos.Top;
        mTitleLabel.Height = 26;

        mButtonRow.Dock = Pos.Top;
        mButtonRow.Height = 28;
        mButtonRow.Margin = new Margin(0, 2, 0, 6);

        mEnterButton.Dock = Pos.Bottom;
        mEnterButton.Height = 28;
        mEnterButton.Margin = new Margin(60, 6, 60, 0);

        mDetailsArea.Dock = Pos.Fill;

        LayoutDifficultyButtons();
        RefreshDifficultyButtons();
    }

    /// <summary>Three equal buttons filling the row, whatever its real width is.</summary>
    private void LayoutDifficultyButtons()
    {
        const int gap = 6;
        var width = mButtonRow.Width;
        if (width <= 0)
        {
            return;
        }

        var buttonWidth = (width - gap * (DungeonDifficulties.Count - 1)) / DungeonDifficulties.Count;
        for (var difficulty = 0; difficulty < DungeonDifficulties.Count; difficulty++)
        {
            mDifficultyButtons[difficulty].SetBounds(
                difficulty * (buttonWidth + gap),
                0,
                buttonWidth,
                mButtonRow.Height
            );
        }
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Show()
    {
        mWindow.Show();
        LayoutDifficultyButtons();
    }

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

        foreach (var row in mDungeonRows)
        {
            mDungeonScroll.RemoveChild(row, true);
        }

        mDungeonRows.Clear();

        var dungeons = DungeonDescriptor.Lookup.Values
            .OfType<DungeonDescriptor>()
            .OrderBy(dungeon => dungeon.Folder)
            .ThenBy(dungeon => dungeon.RequiredLevel)
            .ThenBy(dungeon => dungeon.Name)
            .ToList();

        foreach (var dungeon in dungeons)
        {
            var dungeonId = dungeon.Id;
            var row = new Button(mDungeonScroll, $"DungeonRow{dungeonId}")
            {
                FontName = FontName,
                FontSize = 10,
                UserData = dungeonId,
            };
            row.SetText(dungeon.Name);
            row.Clicked += (_, _) => SelectDungeon(dungeonId);
            mDungeonRows.Add(row);
        }

        mSelected = dungeons.FirstOrDefault(dungeon => dungeon.Id == previouslySelected) ?? dungeons.FirstOrDefault();

        LayoutDungeonRows();
        RefreshDungeonRows();
        RequestLeaderboard();
        UpdateDetails();
    }

    /// <summary>One full-width row per dungeon, stacked from the top.</summary>
    private void LayoutDungeonRows()
    {
        var width = mDungeonScroll.Width - (mDungeonScroll.VerticalScrollBar?.Width ?? 0) - 2;
        if (width <= 0)
        {
            return;
        }

        const int rowHeight = 30;
        const int gap = 4;
        for (var index = 0; index < mDungeonRows.Count; index++)
        {
            mDungeonRows[index].SetBounds(0, index * (rowHeight + gap), width, rowHeight);
        }
    }

    /// <summary>Availability colour on each row (green ready, orange on cooldown, grey if the level is too low); the
    /// selected dungeon is drawn larger.</summary>
    private void RefreshDungeonRows()
    {
        foreach (var row in mDungeonRows)
        {
            if (row.UserData is not Guid dungeonId || DungeonDescriptor.Get(dungeonId) is not { } dungeon)
            {
                continue;
            }

            var color = GetAvailabilityColor(dungeon);
            var selected = mSelected?.Id == dungeonId;
            row.SetTextColor(color, ComponentState.Normal);
            row.SetTextColor(color, ComponentState.Hovered);
            row.SetTextColor(color, ComponentState.Active);
            row.SetTextColor(color, ComponentState.Disabled);
            row.FontSize = selected ? 12 : 10;
            row.SetText(selected ? $"> {dungeon.Name}" : dungeon.Name);
        }
    }

    private void SelectDungeon(Guid dungeonId)
    {
        mSelected = DungeonDescriptor.Get(dungeonId);
        RefreshDungeonRows();
        UpdateDetails();
        RequestLeaderboard();
    }

    private void SelectDifficulty(byte difficulty)
    {
        mDifficulty = DungeonDifficulties.Clamp(difficulty);
        RefreshDifficultyButtons();
        UpdateDetails();
        RequestLeaderboard();
    }

    private void RequestLeaderboard()
    {
        if (mSelected != null)
        {
            PacketSender.SendRequestDungeonLeaderboard(mSelected.Id, mDifficulty);
        }
    }

    /// <summary>The selected difficulty shows in its colour, the others are dimmed.</summary>
    private void RefreshDifficultyButtons()
    {
        for (var difficulty = 0; difficulty < DungeonDifficulties.Count; difficulty++)
        {
            var selected = difficulty == mDifficulty;
            var button = mDifficultyButtons[difficulty];
            var color = selected ? GetDifficultyColor((byte)difficulty) : DimColor;
            button.SetTextColor(color, ComponentState.Normal);
            button.SetTextColor(color, ComponentState.Hovered);
            button.SetTextColor(color, ComponentState.Active);
            button.SetTextColor(color, ComponentState.Disabled);
            button.FontSize = selected ? 11 : 10;
        }
    }

    /// <summary>Called by <see cref="MenuContainer"/> when the server answers a leaderboard request.</summary>
    public void SetLeaderboard(DungeonLeaderboardPacket packet)
    {
        mLeaderboardDungeonId = packet.DungeonId;
        mLeaderboardDifficulty = packet.Difficulty;
        mLeaderboardEntries = packet.Entries;

        if (mSelected?.Id == mLeaderboardDungeonId && mDifficulty == mLeaderboardDifficulty)
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

        PacketSender.SendEnterDungeon(mSelected.Id, mDifficulty);
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
        var difficulty = mDifficulty;
        var difficultyColor = GetDifficultyColor(difficulty);
        mTitleLabel.SetText(dungeon.Name);

        if (!string.IsNullOrWhiteSpace(dungeon.Description))
        {
            AddColored(dungeon.Description, WhiteColor);
            mDetailsLabel.AddLineBreak();
        }

        // What the chosen difficulty changes.
        AddHeader(Strings.Dungeons.DifficultyHeader);
        AddColored($"{GetDifficultyName(difficulty)}", difficultyColor);
        AddColored(
            Strings.Dungeons.MonsterLevelLine.ToString(DungeonDifficulties.MonsterLevel(difficulty)),
            WhiteColor
        );

        var recommended = DungeonDifficulties.MonsterLevel(difficulty);
        var playerLevel = Globals.Me?.Level ?? 0;
        AddColored(
            Strings.Dungeons.RecommendedLevelLine.ToString(recommended),
            playerLevel >= recommended ? EasyColor : NormalColor
        );
        mDetailsLabel.AddLineBreak();

        // The dungeon itself.
        AddHeader(Strings.Dungeons.InfoHeader);
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

        // Rewards: items and experience scale with the difficulty, titles and achievements do not.
        var itemMultiplier = DungeonDifficulties.ItemMultiplier(difficulty);
        var experienceMultiplier = DungeonDifficulties.ExperienceMultiplier(difficulty);
        AddHeader(Strings.Dungeons.RewardsHeader);
        var anyReward = false;
        foreach (var reward in dungeon.Rewards)
        {
            if (ItemDescriptor.Get(reward.ItemId) is not { } item)
            {
                continue;
            }

            anyReward = true;
            AddColored(Strings.Dungeons.RewardItem.ToString(item.Name, reward.Quantity * itemMultiplier), WhiteColor);
        }

        if (dungeon.RewardExperience > 0)
        {
            anyReward = true;
            AddColored(Strings.Dungeons.RewardExperience.ToString(dungeon.RewardExperience * experienceMultiplier), WhiteColor);
        }

        if (dungeon.RewardTitleId != Guid.Empty && TitleDescriptor.Get(dungeon.RewardTitleId) is { } title)
        {
            anyReward = true;
            AddColored(Strings.Dungeons.RewardTitle.ToString(title.Name), HeaderColor);
        }

        if (!anyReward)
        {
            AddColored(Strings.Dungeons.None, DimColor);
        }

        mDetailsLabel.AddLineBreak();

        // Personal record and leaderboard are kept per difficulty.
        var recordKey = DungeonDifficulties.RecordKey(dungeon.Id, difficulty);
        AddHeader(Strings.Dungeons.RecordHeader);
        if (Globals.Me != null && Globals.Me.DungeonRecords.TryGetValue(recordKey, out var record) && record.Completions > 0)
        {
            AddColored(Strings.Dungeons.Record.ToString(record.Completions, FormatTime(record.BestTimeMs)), WhiteColor);
        }
        else
        {
            AddColored(Strings.Dungeons.NeverCompleted, DimColor);
        }

        if (mLeaderboardDungeonId == dungeon.Id && mLeaderboardDifficulty == difficulty && mLeaderboardEntries.Length > 0)
        {
            mDetailsLabel.AddLineBreak();
            AddHeader(Strings.Dungeons.LeaderboardHeader);
            for (var rank = 0; rank < mLeaderboardEntries.Length; rank++)
            {
                var entry = mLeaderboardEntries[rank];
                AddColored(
                    Strings.Dungeons.LeaderboardEntry.ToString(rank + 1, entry.PlayerName, FormatTime(entry.TimeMs)),
                    rank == 0 ? HeaderColor : WhiteColor
                );
            }
        }

        UpdateEnterButton();
        FinishDetails();
    }

    private void AddHeader(string text) => AddColored(text, HeaderColor);

    private void AddColored(string text, Color color)
    {
        mDetailsLabel.AddText(text, color);
        mDetailsLabel.AddLineBreak();
    }

    private void AddLine(string label, string value) => AddColored($"{label} {value}", WhiteColor);

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

        var cooldown = GetCooldownRemainingMs(mSelected, mDifficulty);
        if (cooldown > 0)
        {
            mEnterButton.SetText(Strings.Dungeons.AvailableIn.ToString(FormatTime(cooldown)));
            mEnterButton.IsDisabled = true;

            return;
        }

        mEnterButton.SetText(
            Strings.Dungeons.EnterDifficulty.ToString(GetDifficultyName(mDifficulty), DungeonDifficulties.MonsterLevel(mDifficulty))
        );
        mEnterButton.IsDisabled = false;
    }

    private static long GetCooldownRemainingMs(DungeonDescriptor dungeon, byte difficulty = DungeonDifficulties.Easy)
    {
        var key = DungeonDifficulties.RecordKey(dungeon.Id, difficulty);
        if (Globals.Me == null || !Globals.Me.DungeonRecords.TryGetValue(key, out var record))
        {
            return 0;
        }

        // The server sent the remaining cooldown at a point in time; count it down locally since then.
        var elapsedSinceUpdate = Timing.Global.Milliseconds - Globals.Me.DungeonRecordsReceivedAt;

        return Math.Max(0, record.CooldownRemainingMs - elapsedSinceUpdate);
    }

    private static string GetDifficultyName(byte difficulty) => DungeonDifficulties.Clamp(difficulty) switch
    {
        DungeonDifficulties.Normal => Strings.Dungeons.DifficultyNormal.ToString(),
        DungeonDifficulties.Hard => Strings.Dungeons.DifficultyHard.ToString(),
        _ => Strings.Dungeons.DifficultyEasy.ToString(),
    };

    private static Color GetDifficultyColor(byte difficulty) => DungeonDifficulties.Clamp(difficulty) switch
    {
        DungeonDifficulties.Normal => NormalColor,
        DungeonDifficulties.Hard => HardColor,
        _ => EasyColor,
    };

    private Color GetAvailabilityColor(DungeonDescriptor dungeon)
    {
        if (Globals.Me == null)
        {
            return Color.White;
        }

        if (Globals.Me.Level < dungeon.RequiredLevel)
        {
            return Color.FromArgb(255, 150, 150, 150);
        }

        return GetCooldownRemainingMs(dungeon, mDifficulty) > 0 ? Color.FromArgb(255, 255, 170, 60) : Color.FromArgb(255, 120, 220, 120);
    }

    public static string FormatTime(long ms)
    {
        var time = TimeSpan.FromMilliseconds(Math.Max(0, ms));

        return time.TotalHours >= 1 ? time.ToString(@"h\:mm\:ss") : time.ToString(@"m\:ss");
    }
}
