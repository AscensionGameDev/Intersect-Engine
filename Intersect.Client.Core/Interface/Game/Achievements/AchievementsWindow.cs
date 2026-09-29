using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Framework.Core.GameObjects.Achievements;

namespace Intersect.Client.Interface.Game.Achievements;

public partial class AchievementsWindow
{
    private readonly Base mParent;

    private WindowControl mWindow;

    private Label mProgressLabel;

    private ListBox mAchievementList;

    private RichLabel mDescriptionLabel;

    private ScrollControl mDescriptionArea;

    private Label mDescriptionTemplate;

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
        mWindow.SetSize(320, 380);
        mWindow.SetPosition(260, 160);
        mWindow.DisableResizing();

        mProgressLabel = new Label(mWindow, "ProgressLabel")
        {
            FontName = "sourcesansproblack",
            FontSize = 9,
        };
        mProgressLabel.SetBounds(8, 32, 304, 16);

        mAchievementList = new ListBox(mWindow, "AchievementList")
        {
            FontName = "sourcesansproblack",
            FontSize = 10,
        };
        mAchievementList.SetBounds(8, 52, 304, 190);
        mAchievementList.EnableScroll(false, true);
        mAchievementList.RowSelected += AchievementList_RowSelected;

        mDescriptionArea = new ScrollControl(mWindow, "DescriptionArea");
        mDescriptionArea.SetBounds(8, 250, 304, 122);
        mDescriptionArea.EnableScroll(false, true);

        mDescriptionTemplate = new Label(mWindow, "DescriptionTemplate")
        {
            FontName = "sourcesansproblack",
            FontSize = 10,
            IsHidden = true,
        };

        mDescriptionLabel = new RichLabel(mDescriptionArea);

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Show() => mWindow.Show();

    public void Hide() => mWindow.Hide();

    public void Update()
    {
        // Nothing to update on a per-frame basis; the list refreshes when the server pushes new data.
    }

    public void UpdateList()
    {
        mAchievementList.RemoveAllRows();
        mDescriptionLabel.ClearText();

        if (Globals.Me == null)
        {
            return;
        }

        var unlocked = Globals.Me.UnlockedAchievements;
        var achievements = AchievementDescriptor.Lookup.Values.OfType<AchievementDescriptor>()
            .OrderBy(achievement => unlocked.ContainsKey(achievement.Id) ? 0 : 1)
            .ThenBy(achievement => achievement.Name)
            .ToArray();

        mProgressLabel.Text = Strings.Achievements.Progress.ToString(
            achievements.Count(achievement => unlocked.ContainsKey(achievement.Id)),
            achievements.Length
        );

        foreach (var achievement in achievements)
        {
            var isUnlocked = unlocked.ContainsKey(achievement.Id);
            var displayName = isUnlocked || !achievement.Hidden ? achievement.Name : Strings.Achievements.Hidden.ToString();

            var row = mAchievementList.AddRow(displayName, name: null, userData: achievement.Id);
            row.SetTextColor(isUnlocked ? new Color(255, 255, 215, 0) : new Color(255, 140, 140, 140));
        }
    }

    private void AchievementList_RowSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        // ListBox raises this event without populating SelectedUserData, so read the row's UserData directly.
        if (arguments.SelectedItem?.UserData is not Guid achievementId || AchievementDescriptor.Get(achievementId) is not { } achievement)
        {
            return;
        }

        mDescriptionLabel.ClearText();

        var isUnlocked = Globals.Me?.UnlockedAchievements.ContainsKey(achievementId) == true;
        if (!isUnlocked && achievement.Hidden)
        {
            mDescriptionLabel.AddText(Strings.Achievements.HiddenDescription, mDescriptionTemplate);
        }
        else if (!string.IsNullOrWhiteSpace(achievement.Description))
        {
            mDescriptionLabel.AddText(achievement.Description, mDescriptionTemplate);
        }

        mDescriptionLabel.Width = mDescriptionArea.Width - mDescriptionArea.VerticalScrollBar.Width;
        mDescriptionLabel.SizeToChildren(false, true);
    }
}
