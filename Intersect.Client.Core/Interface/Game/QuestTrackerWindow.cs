using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.NPCs;
using Intersect.Framework.Core.GameObjects.Quests;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game;

/// <summary>
/// Always-on-screen collapsible drawer listing every quest the player currently has in progress, along
/// with their current task, so they don't have to open the quest log just to remember what to do next.
/// Built entirely out of real GWEN controls (same as every other window in the game, e.g.
/// <see cref="PartyWindow"/>'s fixed-slot pattern) instead of direct-drawn text, since that's the
/// rendering path we know is reliable here.
/// </summary>
public partial class QuestTrackerWindow
{
    private const int TopMargin = 180;
    private const int RowHeight = 40;
    private const int PanelWidth = 300;
    private const int MaxTrackedQuests = 8;

    private const string FontName = "m6x11plus";

    private static bool PreferEnglish => string.Equals(ClientConfiguration.Instance.Language, "en", StringComparison.OrdinalIgnoreCase);

    private readonly ImagePanel _panel;

    private readonly Button _toggleTab;

    private readonly List<Label> _nameLabels = [];

    private readonly List<Label> _taskLabels = [];

    private bool _collapsed;

    public QuestTrackerWindow(Canvas gameCanvas)
    {
        _toggleTab = new Button(gameCanvas, "QuestTrackerToggle")
        {
            FontName = FontName,
            FontSize = 16,
            Alignment = [Alignments.Top, Alignments.Right],
            AlignmentPadding = new Padding(0, TopMargin, 0, 0),
        };
        _toggleTab.SetText("«");
        _toggleTab.SetSize(20, 20);
        _toggleTab.Clicked += ToggleTab_Clicked;
        _toggleTab.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());

        _panel = new ImagePanel(gameCanvas, "QuestTrackerPanel")
        {
            Alignment = [Alignments.Top, Alignments.Right],
            AlignmentPadding = new Padding(0, TopMargin + 24, 0, 0),
        };
        _panel.SetSize(PanelWidth, MaxTrackedQuests * RowHeight);
        _panel.RenderColor = new Color(90, 0, 0, 0);

        for (var i = 0; i < MaxTrackedQuests; i++)
        {
            var nameLabel = new Label(_panel, $"QuestTrackerName{i}")
            {
                FontName = FontName,
                FontSize = 16,
            };
            nameLabel.SetPosition(4, i * RowHeight + 2);
            nameLabel.SetSize(PanelWidth - 8, 20);
            nameLabel.SetTextColor(CustomColors.QuestWindow.InProgress, ComponentState.Normal);
            nameLabel.IsHidden = true;
            _nameLabels.Add(nameLabel);

            var taskLabel = new Label(_panel, $"QuestTrackerTask{i}")
            {
                FontName = FontName,
                FontSize = 14,
            };
            taskLabel.SetPosition(10, i * RowHeight + 20);
            taskLabel.SetSize(PanelWidth - 14, 18);
            taskLabel.SetTextColor(Color.White, ComponentState.Normal);
            taskLabel.IsHidden = true;
            _taskLabels.Add(taskLabel);
        }

        _panel.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());
    }

    private void ToggleTab_Clicked(Base sender, MouseButtonState arguments)
    {
        _collapsed = !_collapsed;
        _toggleTab.SetText(_collapsed ? "»" : "«");
        _panel.IsHidden = _collapsed;
    }

    public void NotifyQuestsUpdated()
    {
        // Kept for callers that used to gate a refresh off this - the list is now rebuilt every tick
        // in Update() instead, since the quest count is tiny and this removes any chance of a missed
        // refresh signal leaving the tracker stale.
    }

    public void Update()
    {
        RebuildList();
    }

    private void RebuildList()
    {
        var lines = new List<(string Name, string Task)>();

        if (Globals.Me != null)
        {
            foreach (var (questId, progress) in Globals.Me.QuestProgress)
            {
                if (progress.TaskId == Guid.Empty || progress.Completed)
                {
                    continue;
                }

                if (!QuestDescriptor.TryGet(questId, out var quest))
                {
                    continue;
                }

                var task = quest.Tasks.FirstOrDefault(t => t.Id == progress.TaskId);
                if (task == null)
                {
                    continue;
                }

                string progressText;
                if (task.Objective == QuestObjective.GatherItems)
                {
                    progressText = Strings.QuestLog.TaskItem.ToString(
                        progress.TaskProgress, task.Quantity, ItemDescriptor.GetName(task.TargetId)
                    );
                }
                else if (task.Objective == QuestObjective.KillNpcs)
                {
                    progressText = Strings.QuestLog.TaskNpc.ToString(
                        progress.TaskProgress, task.Quantity, NPCDescriptor.GetName(task.TargetId)
                    );
                }
                else
                {
                    progressText = PreferEnglish && !string.IsNullOrWhiteSpace(task.DescriptionEn)
                        ? task.DescriptionEn
                        : task.Description;
                }

                lines.Add((quest.GetLocalizedName(PreferEnglish), progressText ?? string.Empty));

                if (lines.Count >= MaxTrackedQuests)
                {
                    break;
                }
            }
        }

        for (var i = 0; i < MaxTrackedQuests; i++)
        {
            if (i < lines.Count)
            {
                _nameLabels[i].SetText(lines[i].Name);
                _nameLabels[i].IsHidden = false;
                _taskLabels[i].SetText(lines[i].Task);
                _taskLabels[i].IsHidden = false;
            }
            else
            {
                _nameLabels[i].IsHidden = true;
                _taskLabels[i].IsHidden = true;
            }
        }
    }
}
