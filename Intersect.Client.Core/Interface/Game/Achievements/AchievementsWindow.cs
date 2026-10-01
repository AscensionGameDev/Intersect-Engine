using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Framework.Core.GameObjects.Achievements;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Mounts;
using Intersect.Framework.Core.GameObjects.Titles;

namespace Intersect.Client.Interface.Game.Achievements;

/// <summary>
/// Achievements list on the left (gold = unlocked, grey = locked, hidden ones masked) and, on the right, the status,
/// description and rewards of the selected one.
/// </summary>
public partial class AchievementsWindow
{
    private const string FontName = "sourcesansproblack";

    private const int WindowWidth = 560;

    private const int WindowHeight = 430;

    private static readonly Color GoldColor = new(255, 255, 215, 0);

    private static readonly Color DimColor = new(255, 150, 150, 150);

    private static readonly Color HeaderColor = new(255, 255, 220, 120);

    private static readonly Color WhiteColor = new(255, 235, 235, 235);

    private static readonly Color GreenColor = new(255, 120, 220, 120);

    private readonly Base mParent;

    private WindowControl mWindow;

    private Label mProgressLabel;

    private ScrollControl mListScroll;

    private readonly List<Button> mRows = [];

    private Base mRightPanel;

    private Label mNameLabel;

    private Label mStatusLabel;

    private ScrollControl mDetailsArea;

    private RichLabel mDetailsLabel;

    private Guid mSelectedId = Guid.Empty;

    public AchievementsWindow(Canvas gameCanvas)
    {
        mParent = gameCanvas;

        GenerateControls();
    }

    private void GenerateControls()
    {
        mWindow = new WindowControl(mParent, Strings.Achievements.Title, false, "AchievementsWindow")
        {
            IsHidden = true,
        };
        mWindow.SetSize(WindowWidth, WindowHeight);
        mWindow.SetPosition(220, 90);
        mWindow.DisableResizing();

        mProgressLabel = new Label(mWindow, "ProgressLabel")
        {
            FontName = FontName,
            FontSize = 11,
        };

        mListScroll = new ScrollControl(mWindow, "AchievementListScroll");
        mListScroll.EnableScroll(false, true);
        mListScroll.SizeChanged += (_, _) => LayoutRows();

        // The right side is docked inside one panel so it always fits the window's real client area.
        mRightPanel = new Base(mWindow, "AchievementRightPanel");

        mNameLabel = new Label(mRightPanel, "AchievementName")
        {
            FontName = FontName,
            FontSize = 14,
        };

        mStatusLabel = new Label(mRightPanel, "AchievementStatus")
        {
            FontName = FontName,
            FontSize = 10,
        };

        mDetailsArea = new ScrollControl(mRightPanel, "AchievementDetails");
        mDetailsArea.EnableScroll(false, true);
        mDetailsArea.SizeChanged += (_, _) => FinishDetails();

        mDetailsLabel = new RichLabel(mDetailsArea);

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        // The window skin (window_blue_achievementswindow.png) is generated at 560x430; docking keeps everything
        // inside the frame (borders: about 18 px on the sides, 22 px at the bottom).
        mWindow.SetSize(WindowWidth, WindowHeight);

        mProgressLabel.Dock = Pos.Top;
        mProgressLabel.Height = 22;
        mProgressLabel.Margin = new Margin(18, 8, 22, 2);

        mListScroll.Dock = Pos.Left;
        mListScroll.Width = 210;
        mListScroll.Margin = new Margin(18, 4, 8, 22);

        mRightPanel.Dock = Pos.Fill;
        mRightPanel.Padding = new Padding(0, 4, 22, 22);

        mNameLabel.Dock = Pos.Top;
        mNameLabel.Height = 26;

        mStatusLabel.Dock = Pos.Top;
        mStatusLabel.Height = 20;

        mDetailsArea.Dock = Pos.Fill;
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Show()
    {
        mWindow.Show();
        LayoutRows();
        FinishDetails();
    }

    public void Hide() => mWindow.Hide();

    public void Update()
    {
        // Nothing to update on a per-frame basis; the list refreshes when the server pushes new data.
    }

    public void UpdateList()
    {
        foreach (var row in mRows)
        {
            mListScroll.RemoveChild(row, true);
        }

        mRows.Clear();

        if (Globals.Me == null)
        {
            mDetailsLabel.ClearText();
            return;
        }

        var unlocked = Globals.Me.UnlockedAchievements;
        var achievements = AchievementDescriptor.Lookup.Values.OfType<AchievementDescriptor>()
            .OrderBy(achievement => unlocked.ContainsKey(achievement.Id) ? 0 : 1)
            .ThenBy(achievement => achievement.Name)
            .ToArray();

        var unlockedCount = achievements.Count(achievement => unlocked.ContainsKey(achievement.Id));
        mProgressLabel.SetText(Strings.Achievements.Progress.ToString(unlockedCount, achievements.Length));
        mProgressLabel.SetTextColor(unlockedCount == achievements.Length && achievements.Length > 0 ? GoldColor : HeaderColor, ComponentState.Normal);

        foreach (var achievement in achievements)
        {
            var achievementId = achievement.Id;
            var row = new Button(mListScroll, $"AchievementRow{achievementId}")
            {
                FontName = FontName,
                FontSize = 10,
                UserData = achievementId,
            };
            row.Clicked += (_, _) => SelectAchievement(achievementId);
            mRows.Add(row);
        }

        // Keep the previous selection when it still exists, otherwise take the first achievement.
        if (mSelectedId == Guid.Empty || achievements.All(achievement => achievement.Id != mSelectedId))
        {
            mSelectedId = achievements.FirstOrDefault()?.Id ?? Guid.Empty;
        }

        LayoutRows();
        RefreshRows();
        ShowDetails();
    }

    /// <summary>One full-width row per achievement, stacked from the top.</summary>
    private void LayoutRows()
    {
        var width = mListScroll.Width - (mListScroll.VerticalScrollBar?.Width ?? 0) - 2;
        if (width <= 0)
        {
            return;
        }

        const int rowHeight = 28;
        const int gap = 4;
        for (var index = 0; index < mRows.Count; index++)
        {
            mRows[index].SetBounds(0, index * (rowHeight + gap), width, rowHeight);
        }
    }

    /// <summary>Gold name when unlocked, grey when locked (hidden ones masked); the selected row is larger.</summary>
    private void RefreshRows()
    {
        var unlocked = Globals.Me?.UnlockedAchievements;
        foreach (var row in mRows)
        {
            if (row.UserData is not Guid achievementId || AchievementDescriptor.Get(achievementId) is not { } achievement)
            {
                continue;
            }

            var isUnlocked = unlocked?.ContainsKey(achievementId) == true;
            var name = isUnlocked || !achievement.Hidden ? achievement.Name : Strings.Achievements.Hidden.ToString();
            var selected = achievementId == mSelectedId;
            var color = isUnlocked ? GoldColor : DimColor;

            row.SetText(selected ? $"> {name}" : name);
            row.FontSize = selected ? 11 : 10;
            row.SetTextColor(color, ComponentState.Normal);
            row.SetTextColor(color, ComponentState.Hovered);
            row.SetTextColor(color, ComponentState.Active);
            row.SetTextColor(color, ComponentState.Disabled);
        }
    }

    private void SelectAchievement(Guid achievementId)
    {
        mSelectedId = achievementId;
        RefreshRows();
        ShowDetails();
    }

    private void ShowDetails()
    {
        mDetailsLabel.ClearText();

        if (mSelectedId == Guid.Empty || AchievementDescriptor.Get(mSelectedId) is not { } achievement)
        {
            mNameLabel.SetText(string.Empty);
            mStatusLabel.SetText(string.Empty);
            AddColored(Strings.Achievements.SelectHint, DimColor);
            FinishDetails();

            return;
        }

        var isUnlocked = Globals.Me?.UnlockedAchievements.ContainsKey(achievement.Id) == true;
        var hiddenForPlayer = !isUnlocked && achievement.Hidden;

        mNameLabel.SetText(hiddenForPlayer ? Strings.Achievements.Hidden.ToString() : achievement.Name);
        mNameLabel.SetTextColor(isUnlocked ? GoldColor : WhiteColor, ComponentState.Normal);
        mStatusLabel.SetText(isUnlocked ? Strings.Achievements.StatusUnlocked.ToString() : Strings.Achievements.StatusLocked.ToString());
        mStatusLabel.SetTextColor(isUnlocked ? GreenColor : DimColor, ComponentState.Normal);

        if (hiddenForPlayer)
        {
            AddColored(Strings.Achievements.HiddenDescription, DimColor);
        }
        else if (!string.IsNullOrWhiteSpace(achievement.Description))
        {
            AddColored(achievement.Description, WhiteColor);
        }

        // Rewards are shown for visible achievements only (a hidden one keeps its secret).
        if (!hiddenForPlayer)
        {
            var lines = new List<string>();
            if (achievement.RewardExperience > 0)
            {
                lines.Add(Strings.Achievements.RewardExperience.ToString(achievement.RewardExperience));
            }

            if (achievement.RewardItemId != Guid.Empty && ItemDescriptor.Get(achievement.RewardItemId) is { } item)
            {
                lines.Add(Strings.Achievements.RewardItem.ToString(item.Name, Math.Max(1, achievement.RewardItemQuantity)));
            }

            if (achievement.RewardTitleId != Guid.Empty && TitleDescriptor.Get(achievement.RewardTitleId) is { } title)
            {
                lines.Add(Strings.Achievements.RewardTitle.ToString(title.Name));
            }

            if (achievement.RewardMountId != Guid.Empty && MountDescriptor.Get(achievement.RewardMountId) is { } mount)
            {
                lines.Add(Strings.Achievements.RewardMount.ToString(mount.Name));
            }

            if (lines.Count > 0)
            {
                mDetailsLabel.AddLineBreak();
                AddColored(Strings.Achievements.RewardsHeader, HeaderColor);
                foreach (var line in lines)
                {
                    AddColored(line, WhiteColor);
                }
            }
        }

        FinishDetails();
    }

    private void AddColored(string text, Color color)
    {
        mDetailsLabel.AddText(text, color);
        mDetailsLabel.AddLineBreak();
    }

    private void FinishDetails()
    {
        mDetailsLabel.Width = mDetailsArea.Width - (mDetailsArea.VerticalScrollBar?.Width ?? 0);
        mDetailsLabel.SizeToChildren(false, true);
    }
}
