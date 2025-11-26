using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Framework.Core.GameObjects.Skills;

namespace Intersect.Client.Interface.Game.Skills;

public partial class SkillsWindow : Window
{
    private readonly ScrollControl _skillContainer;
    private readonly List<Label> _skillLabels = new();

    public SkillsWindow(Canvas gameCanvas) : base(gameCanvas, "Skills", false, nameof(SkillsWindow))
    {
        DisableResizing();

        Alignment = [Alignments.Bottom, Alignments.Left];
        MinimumSize = new Point(x: 300, y: 400);
        Margin = new Margin(15, 0, 0, 60);
        IsVisibleInTree = false;
        IsResizable = true;
        IsClosable = true;

        _skillContainer = new ScrollControl(this, "SkillsContainer")
        {
            Dock = Pos.Fill,
            OverflowX = OverflowBehavior.Auto,
            OverflowY = OverflowBehavior.Scroll,
        };
    }

    protected override void EnsureInitialized()
    {
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        UpdateSkills();
    }

    public void UpdateSkills()
    {
        if (!IsVisibleInTree || Globals.Me == null)
        {
            return;
        }

        // Clear existing labels
        foreach (var label in _skillLabels)
        {
            label?.Parent?.RemoveChild(label, false);
        }
        _skillLabels.Clear();

        // Add header
        var headerLabel = new Label(_skillContainer, "SkillsHeaderLabel")
        {
            Text = "Skills",
            TextColor = Color.White,
            Font = GameContentManager.Current.GetFont("sourcesansproblack"),
            FontSize = 14,
        };
        _skillContainer.AddChild(headerLabel);
        _skillLabels.Add(headerLabel);

        // Display each skill
        var yOffset = 30;
        foreach (var skillDescriptor in SkillDescriptor.Lookup.Values.OrderBy(s => s.Name))
        {
            if (Globals.Me.Skills.TryGetValue(skillDescriptor.Id, out var skillData))
            {
                var skillLabel = new Label(_skillContainer, $"SkillLabel_{skillDescriptor.Id}")
                {
                    Text = $"{skillDescriptor.Name}: Level {skillData.Level} | XP: {skillData.Experience}/{skillData.ExperienceToNextLevel}",
                    TextColor = Color.White,
                    Font = GameContentManager.Current.GetFont("sourcesanspro"),
                    FontSize = 12,
                    Y = yOffset,
                };
                _skillContainer.AddChild(skillLabel);
                _skillLabels.Add(skillLabel);
                yOffset += 25;
            }
        }

        if (_skillLabels.Count == 1)
        {
            var noSkillsLabel = new Label(_skillContainer, "NoSkillsLabel")
            {
                Text = "No skills yet. Gain experience to see your skills here!",
                TextColor = Color.Gray,
                Font = GameContentManager.Current.GetFont("sourcesanspro"),
                FontSize = 12,
                Y = yOffset,
            };
            _skillContainer.AddChild(noSkillsLabel);
            _skillLabels.Add(noSkillsLabel);
        }
    }

    public override void Show()
    {
        base.Show();
        UpdateSkills();
    }
}

