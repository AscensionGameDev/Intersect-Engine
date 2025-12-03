using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Utilities;
using Intersect.Framework.Core.GameObjects.Skills;
using System.Linq;
using System.Collections.Generic;

namespace Intersect.Client.Interface.Game.Skills;

public partial class SkillsWindow : Window
{
    private readonly ScrollControl _skillContainer;
    private readonly Dictionary<Guid, SkillItem> _skillItems = new();
    private readonly Label _noSkillsLabel;
    
    // Store individual controls per skill
    private class SkillItem
    {
        public ImagePanel IconPanel { get; set; }
        public Label NameLabel { get; set; }
        public Label LevelLabel { get; set; }
        public Label ExperienceLabel { get; set; }
        public ImagePanel ProgressBarBackground { get; set; }
        public ImagePanel ProgressBarForeground { get; set; }
        public int YPosition { get; set; }
    }
    
    private readonly Dictionary<Guid, SkillItem> _skillControls = new();

    public SkillsWindow(Canvas gameCanvas) : base(gameCanvas, Strings.GameMenu.Skills, false, nameof(SkillsWindow))
    {
        DisableResizing();

        Alignment = [Alignments.Bottom, Alignments.Left];
        MinimumSize = new Point(x: 220, y: 400);
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

        _noSkillsLabel = new Label(_skillContainer, "NoSkillsLabel")
        {
            Text = "No skills yet. Gain experience to see your skills here!",
            TextColor = new Color(150, 150, 150, 255),
            FontName = "sourcesanspro",
            FontSize = 12,
            Alignment = [Alignments.Center, Alignments.Center],
            Dock = Pos.Fill,
            IsHidden = true,
        };
    }

    protected override void EnsureInitialized()
    {
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        
        // Format the window title
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;
        
        UpdateSkills();
    }

    public void UpdateSkills()
    {
        if (!IsVisibleInTree || Globals.Me == null)
        {
            return;
        }

        // Get all skills the player has experience in
        var playerSkills = Globals.Me.Skills.Keys.ToHashSet();

        // Remove skill items that the player no longer has
        var skillsToRemove = _skillControls.Keys.Where(skillId => !playerSkills.Contains(skillId)).ToList();
        foreach (var skillId in skillsToRemove)
        {
            if (_skillControls.TryGetValue(skillId, out var skillControl))
            {
                skillControl.IconPanel?.Parent?.RemoveChild(skillControl.IconPanel, false);
                skillControl.NameLabel?.Parent?.RemoveChild(skillControl.NameLabel, false);
                skillControl.LevelLabel?.Parent?.RemoveChild(skillControl.LevelLabel, false);
                skillControl.ExperienceLabel?.Parent?.RemoveChild(skillControl.ExperienceLabel, false);
                skillControl.ProgressBarBackground?.Parent?.RemoveChild(skillControl.ProgressBarBackground, false);
                skillControl.ProgressBarForeground?.Parent?.RemoveChild(skillControl.ProgressBarForeground, false);
                _skillControls.Remove(skillId);
            }
        }

        // Add or update skill items
        var skillsToShow = SkillDescriptor.Lookup.Values
            .OfType<SkillDescriptor>()
            .Where(s => playerSkills.Contains(s.Id))
            .OrderBy(s => s.Name)
            .ToList();

        if (skillsToShow.Count == 0)
        {
            _noSkillsLabel.IsHidden = false;
            return;
        }

        _noSkillsLabel.IsHidden = true;

        // Position skill items vertically
        var yPosition = 4;
        foreach (var skillDescriptor in skillsToShow)
        {
            if (!_skillControls.TryGetValue(skillDescriptor.Id, out var skillControl))
            {
                // Create new skill controls directly on the ScrollControl - following CharacterWindow pattern exactly
                skillControl = new SkillItem
                {
                    IconPanel = new ImagePanel(_skillContainer, $"SkillIcon_{skillDescriptor.Id}"),
                    NameLabel = new Label(_skillContainer, $"SkillName_{skillDescriptor.Id}"),
                    LevelLabel = new Label(_skillContainer, $"SkillLevel_{skillDescriptor.Id}"),
                    ProgressBarBackground = new ImagePanel(_skillContainer, $"ProgressBarBg_{skillDescriptor.Id}"),
                    ProgressBarForeground = new ImagePanel(_skillContainer, $"ProgressBarFg_{skillDescriptor.Id}"),
                    ExperienceLabel = new Label(_skillContainer, $"SkillExp_{skillDescriptor.Id}")
                };
                
                // Set up icon panel
                skillControl.IconPanel.Size = new Point(48, 48);
                skillControl.IconPanel.MouseInputEnabled = false;
                
                // Set up name label - ensure font and color are set
                skillControl.NameLabel.FontName = "sourcesansproblack";
                skillControl.NameLabel.FontSize = 14;
                skillControl.NameLabel.SetTextColor(Color.White, ComponentState.Normal);
                skillControl.NameLabel.TextColor = Color.White;
                skillControl.NameLabel.SetText("");
                
                // Set up level label
                skillControl.LevelLabel.FontName = "sourcesanspro";
                skillControl.LevelLabel.FontSize = 12;
                skillControl.LevelLabel.SetTextColor(Color.White, ComponentState.Normal); // Use white for visibility
                skillControl.LevelLabel.TextColor = Color.White;
                skillControl.LevelLabel.SetText("");
                
                // Set up experience label
                skillControl.ExperienceLabel.FontName = "sourcesanspro";
                skillControl.ExperienceLabel.FontSize = 11;
                skillControl.ExperienceLabel.SetTextColor(Color.White, ComponentState.Normal); // Use white for visibility
                skillControl.ExperienceLabel.TextColor = Color.White;
                skillControl.ExperienceLabel.SetText("");
                
                // Set up progress bars
                skillControl.ProgressBarBackground.TextureFilename = "expbackground.png";
                skillControl.ProgressBarBackground.MouseInputEnabled = false;
                skillControl.ProgressBarForeground.TextureFilename = "expbar.png";
                skillControl.ProgressBarForeground.MouseInputEnabled = false;
                
                _skillControls[skillDescriptor.Id] = skillControl;
            }

            // Update skill data - following CharacterWindow Update() pattern
            if (Globals.Me.Skills.TryGetValue(skillDescriptor.Id, out var skillData))
            {
                // Update icon
                if (!string.IsNullOrWhiteSpace(skillDescriptor.Icon))
                {
                    var iconTexture = GameContentManager.Current.GetTexture(TextureType.Item, skillDescriptor.Icon);
                    if (iconTexture != null)
                    {
                        skillControl.IconPanel.Texture = iconTexture;
                        skillControl.IconPanel.Show();
                    }
                    else
                    {
                        skillControl.IconPanel.Hide();
                    }
                }
                else
                {
                    skillControl.IconPanel.Hide();
                }

                // CRITICAL: Set text FIRST, then position - this ensures text is rendered
                var skillName = skillDescriptor.Name ?? "Unknown Skill";
                skillControl.NameLabel.SetText(skillName);
                skillControl.NameLabel.Text = skillName; // Also set .Text property
                
                var levelText = $"Level {skillData.Level}";
                skillControl.LevelLabel.SetText(levelText);
                skillControl.LevelLabel.Text = levelText;
                
                // Update progress bar
                var expToNext = Globals.Me.GetSkillExperienceToNextLevel(skillDescriptor.Id);
                string expText;
                if (expToNext > 0)
                {
                    var progress = (float)skillData.Experience / expToNext;
                    progress = Math.Max(0, Math.Min(1, progress));
                    var progressWidth = (int)(160 * progress);
                    skillControl.ProgressBarForeground.Width = Math.Max(1, progressWidth);
                    expText = $"{skillData.Experience:#,##0} / {expToNext:#,##0} XP";
                }
                else
                {
                    skillControl.ProgressBarForeground.Width = 160;
                    expText = $"{skillData.Experience:#,##0} XP (Max Level)";
                }
                skillControl.ExperienceLabel.SetText(expText);
                skillControl.ExperienceLabel.Text = expText;
                
                // Update positions and sizes - ensure proper spacing
                skillControl.IconPanel.X = 4;
                skillControl.IconPanel.Y = yPosition + 4;
                skillControl.IconPanel.Size = new Point(48, 48);
                
                // Update positions and ensure proper sizing with adequate spacing
                // Name at top
                skillControl.NameLabel.X = 56;
                skillControl.NameLabel.Y = yPosition + 2;
                skillControl.NameLabel.Width = 160;
                skillControl.NameLabel.Height = 18;
                skillControl.NameLabel.AutoSizeToContents = false;
                
                // Level below name with spacing
                skillControl.LevelLabel.X = 56;
                skillControl.LevelLabel.Y = yPosition + 20; // 2px gap after name (18px height)
                skillControl.LevelLabel.Width = 160;
                skillControl.LevelLabel.Height = 16;
                skillControl.LevelLabel.AutoSizeToContents = false;
                
                // Progress bar below level with spacing
                skillControl.ProgressBarBackground.X = 56;
                skillControl.ProgressBarBackground.Y = yPosition + 38; // 2px gap after level (16px height)
                skillControl.ProgressBarBackground.Width = 160;
                skillControl.ProgressBarBackground.Height = 8;
                
                skillControl.ProgressBarForeground.X = 56;
                skillControl.ProgressBarForeground.Y = yPosition + 38;
                skillControl.ProgressBarForeground.Height = 8;
                
                // Experience below progress bar with spacing
                skillControl.ExperienceLabel.X = 56;
                skillControl.ExperienceLabel.Y = yPosition + 48; // 2px gap after progress bar (8px height)
                skillControl.ExperienceLabel.Width = 160;
                skillControl.ExperienceLabel.Height = 12;
                skillControl.ExperienceLabel.AutoSizeToContents = false;
                
                // CRITICAL: Force all controls to be visible and render
                skillControl.IconPanel.IsHidden = false;
                skillControl.IconPanel.IsVisibleInParent = true;
                
                skillControl.NameLabel.IsHidden = false;
                skillControl.NameLabel.IsVisibleInParent = true;
                skillControl.NameLabel.Show();
                
                skillControl.LevelLabel.IsHidden = false;
                skillControl.LevelLabel.IsVisibleInParent = true;
                skillControl.LevelLabel.Show();
                
                skillControl.ExperienceLabel.IsHidden = false;
                skillControl.ExperienceLabel.IsVisibleInParent = true;
                skillControl.ExperienceLabel.Show();
                
                skillControl.ProgressBarBackground.IsHidden = false;
                skillControl.ProgressBarBackground.IsVisibleInParent = true;
                skillControl.ProgressBarForeground.IsHidden = false;
                skillControl.ProgressBarForeground.IsVisibleInParent = true;
            }
            
            // Move to next position (2 + 18 + 2 + 16 + 2 + 8 + 2 + 12 = 62px total height + 4px margin)
            yPosition += 62 + 4;
        }
    }

    public override void Show()
    {
        base.Show();
        UpdateSkills();
    }
}

