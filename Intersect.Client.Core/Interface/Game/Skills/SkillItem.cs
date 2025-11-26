using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Framework.Core.GameObjects.Skills;

namespace Intersect.Client.Interface.Game.Skills;

public partial class SkillItem : ImagePanel
{
    private readonly SkillsWindow _skillsWindow;
    private readonly ImagePanel _iconPanel;
    private readonly Label _nameLabel;
    private readonly Label _levelLabel;
    private readonly Label _experienceLabel;
    private readonly ImagePanel _progressBarBackground;
    private readonly ImagePanel _progressBarForeground;
    private Guid _skillId;
    private bool _isInitialized;

    public SkillItem(SkillsWindow skillsWindow, Base parent, Guid skillId) : base(parent, $"SkillItem_{skillId}")
    {
        _skillsWindow = skillsWindow;
        _skillId = skillId;

        MinimumSize = new Point(220, 65);
        MaximumSize = new Point(220, 65);
        Size = new Point(220, 65);
        Margin = new Margin(4, 2, 4, 2);
        MouseInputEnabled = true;

        // Icon panel
        _iconPanel = new ImagePanel(this, "SkillIcon")
        {
            Size = new Point(48, 48),
            X = 4,
            Y = 4,
            MouseInputEnabled = false,
        };

        // Name label - positioned to the right of icon
        var nameFont = GameContentManager.Current.GetFont("sourcesansproblack");
        _nameLabel = new Label(this, "SkillName")
        {
            X = 56,
            Y = 4,
            Width = 160,
            Height = 18,
            TextColor = Color.White,
            Font = nameFont,
            FontName = "sourcesansproblack",
            FontSize = 12,
            Alignment = [Alignments.Left, Alignments.Top],
            MouseInputEnabled = false,
            AutoSizeToContents = false,
        };

        // Level label - below name
        var levelFont = GameContentManager.Current.GetFont("sourcesanspro");
        _levelLabel = new Label(this, "SkillLevel")
        {
            X = 56,
            Y = 22,
            Width = 160,
            Height = 16,
            TextColor = new Color(200, 200, 200, 255),
            Font = levelFont,
            FontName = "sourcesanspro",
            FontSize = 10,
            Alignment = [Alignments.Left, Alignments.Top],
            MouseInputEnabled = false,
            AutoSizeToContents = false,
        };

        // Progress bar background
        _progressBarBackground = new ImagePanel(this, "ProgressBarBackground")
        {
            X = 56,
            Y = 40,
            Width = 160,
            Height = 8,
            MouseInputEnabled = false,
            TextureFilename = "expbackground.png",
        };

        // Progress bar foreground
        _progressBarForeground = new ImagePanel(this, "ProgressBarForeground")
        {
            X = 56,
            Y = 40,
            Width = 0,
            Height = 8,
            MouseInputEnabled = false,
            TextureFilename = "expbar.png",
        };

        // Experience label - below progress bar
        var expFont = GameContentManager.Current.GetFont("sourcesanspro");
        _experienceLabel = new Label(this, "SkillExperience")
        {
            X = 56,
            Y = 50,
            Width = 160,
            Height = 12,
            TextColor = new Color(180, 180, 180, 255),
            Font = expFont,
            FontName = "sourcesanspro",
            FontSize = 9,
            Alignment = [Alignments.Left, Alignments.Top],
            MouseInputEnabled = false,
            AutoSizeToContents = false,
        };

        HoverEnter += SkillItem_HoverEnter;
        HoverLeave += SkillItem_HoverLeave;

        // Set initial text to ensure labels are created properly
        _nameLabel.Text = "Loading...";
        _levelLabel.Text = "Level 1";
        _experienceLabel.Text = "0 / 0 XP";
        
        // Don't use LoadJsonUi - this is a custom control without JSON definitions
        // LoadJsonUi would try to find JSON and might hide our labels
        // LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        
        // CRITICAL: Force labels to be visible and properly initialized
        _nameLabel.IsHidden = false;
        _nameLabel.IsVisibleInParent = true;
        _nameLabel.Invalidate();
        
        _levelLabel.IsHidden = false;
        _levelLabel.IsVisibleInParent = true;
        _levelLabel.Invalidate();
        
        _experienceLabel.IsHidden = false;
        _experienceLabel.IsVisibleInParent = true;
        _experienceLabel.Invalidate();
        
        _progressBarBackground.IsHidden = false;
        _progressBarBackground.IsVisibleInParent = true;
        _progressBarForeground.IsHidden = false;
        _progressBarForeground.IsVisibleInParent = true;
        
        // Force a layout update
        Invalidate();
        
        _isInitialized = true;
    }

    private void SkillItem_HoverEnter(Base sender, EventArgs arguments)
    {
        if (!SkillDescriptor.TryGet(_skillId, out var skillDescriptor))
        {
            return;
        }

        if (Globals.Me?.Skills.TryGetValue(_skillId, out var skillData) != true)
        {
            return;
        }

        var tooltipText = $"{skillDescriptor.Name}\n";
        tooltipText += $"Level: {skillData.Level}";
        
        if (!string.IsNullOrWhiteSpace(skillDescriptor.Description))
        {
            tooltipText += $"\n\n{skillDescriptor.Description}";
        }

        var expToNext = Globals.Me.GetSkillExperienceToNextLevel(_skillId);
        if (expToNext > 0)
        {
            tooltipText += $"\n\nExperience: {skillData.Experience:#,##0} / {expToNext:#,##0}";
            var expNeeded = expToNext - skillData.Experience;
            if (expNeeded > 0)
            {
                tooltipText += $"\nTo Next Level: {expNeeded:#,##0}";
            }
        }
        else
        {
            tooltipText += $"\n\nExperience: {skillData.Experience:#,##0} (Max Level)";
        }

        SetToolTipText(tooltipText);
    }

    private void SkillItem_HoverLeave(Base sender, EventArgs arguments)
    {
        SetToolTipText("");
    }

    public void Update()
    {
        if (!_isInitialized || Globals.Me == null)
        {
            return;
        }

        if (!Globals.Me.Skills.TryGetValue(_skillId, out var skillData))
        {
            Hide();
            return;
        }

        // Show the item even if descriptor isn't loaded yet
        Show();
        IsHidden = false;
        IsVisibleInParent = true;
        
        // CRITICAL: Ensure labels are children and visible BEFORE setting text
        // Bring labels to front so they render above progress bars
        _nameLabel.BringToFront();
        _levelLabel.BringToFront();
        _experienceLabel.BringToFront();
        
        // Force all labels to be visible (JSON UI might hide them)
        _nameLabel.IsHidden = false;
        _nameLabel.IsVisibleInParent = true;
        _levelLabel.IsHidden = false;
        _levelLabel.IsVisibleInParent = true;
        _experienceLabel.IsHidden = false;
        _experienceLabel.IsVisibleInParent = true;
        _progressBarBackground.IsHidden = false;
        _progressBarBackground.IsVisibleInParent = true;

        // Try to get the skill descriptor, but don't hide if it's not loaded yet
        if (!SkillDescriptor.TryGet(_skillId, out var skillDescriptor))
        {
            // Descriptor not loaded yet - show placeholder with skill ID
            _nameLabel.Text = $"Skill ({_skillId.ToString().Substring(0, 8)}...)";
            _levelLabel.Text = $"Level {skillData.Level}";
            _iconPanel.Hide();
            
            // Still show progress bar with available data
            var expToNextLevel = skillData.ExperienceToNextLevel > 0 
                ? skillData.ExperienceToNextLevel 
                : (Globals.Me.GetSkillExperienceToNextLevel(_skillId) > 0 
                    ? Globals.Me.GetSkillExperienceToNextLevel(_skillId) 
                    : 1000); // Fallback if we can't calculate
            
            if (expToNextLevel > 0)
            {
                var progress = (float)skillData.Experience / expToNextLevel;
                progress = Math.Max(0, Math.Min(1, progress));
                var progressWidth = (int)(_progressBarBackground.Width * progress);
                _progressBarForeground.Width = Math.Max(1, progressWidth);
                _progressBarForeground.Show();
                _experienceLabel.Text = $"{skillData.Experience:#,##0} / {expToNextLevel:#,##0} XP";
            }
            else
            {
                _progressBarForeground.Width = _progressBarBackground.Width;
                _progressBarForeground.Show();
                _experienceLabel.Text = $"{skillData.Experience:#,##0} XP (Max Level)";
            }
            
            // Force labels to be visible and positioned
            _nameLabel.X = 56;
            _nameLabel.Y = 4;
            _nameLabel.IsHidden = false;
            _nameLabel.IsVisibleInParent = true;
            
            _levelLabel.X = 56;
            _levelLabel.Y = 22;
            _levelLabel.IsHidden = false;
            _levelLabel.IsVisibleInParent = true;
            
            _experienceLabel.X = 56;
            _experienceLabel.Y = 50;
            _experienceLabel.IsHidden = false;
            _experienceLabel.IsVisibleInParent = true;
            
            _progressBarBackground.X = 56;
            _progressBarBackground.Y = 40;
            _progressBarBackground.IsHidden = false;
            _progressBarForeground.X = 56;
            _progressBarForeground.Y = 40;
            _progressBarForeground.IsHidden = false;
            return;
        }

        // Update icon
        if (!string.IsNullOrWhiteSpace(skillDescriptor.Icon))
        {
            var iconTexture = GameContentManager.Current.GetTexture(TextureType.Item, skillDescriptor.Icon);
            if (iconTexture != null)
            {
                _iconPanel.Texture = iconTexture;
                _iconPanel.Show();
            }
            else
            {
                _iconPanel.Hide();
            }
        }
        else
        {
            _iconPanel.Hide();
        }

        // Update name - ensure it's visible and positioned correctly
        _nameLabel.Text = skillDescriptor.Name ?? "Unknown Skill";
        _nameLabel.X = 56;
        _nameLabel.Y = 4;
        _nameLabel.IsHidden = false;
        _nameLabel.IsVisibleInParent = true;
        _nameLabel.Invalidate();

        // Update level - ensure it's visible and positioned correctly
        _levelLabel.Text = $"Level {skillData.Level}";
        _levelLabel.X = 56;
        _levelLabel.Y = 22;
        _levelLabel.IsHidden = false;
        _levelLabel.IsVisibleInParent = true;
        _levelLabel.Invalidate();

        // Update experience and progress bar
        var expToNext = Globals.Me.GetSkillExperienceToNextLevel(_skillId);
        if (expToNext > 0)
        {
            var progress = (float)skillData.Experience / expToNext;
            progress = Math.Max(0, Math.Min(1, progress)); // Clamp between 0 and 1

            var progressWidth = (int)(_progressBarBackground.Width * progress);
            _progressBarForeground.Width = Math.Max(1, progressWidth); // Ensure at least 1 pixel wide if there's any progress
            _progressBarForeground.Show();
            _experienceLabel.Text = $"{skillData.Experience:#,##0} / {expToNext:#,##0} XP";
        }
        else
        {
            // Max level
            _progressBarForeground.Width = _progressBarBackground.Width;
            _progressBarForeground.Show();
            _experienceLabel.Text = $"{skillData.Experience:#,##0} XP (Max Level)";
        }
        
        // Ensure experience label is visible and positioned correctly
        _experienceLabel.X = 56;
        _experienceLabel.Y = 50;
        _experienceLabel.IsHidden = false;
        _experienceLabel.IsVisibleInParent = true;
        _experienceLabel.Invalidate();
        
        // Ensure progress bars are positioned correctly
        _progressBarBackground.X = 56;
        _progressBarBackground.Y = 40;
        _progressBarBackground.IsHidden = false;
        _progressBarForeground.X = 56;
        _progressBarForeground.Y = 40;
        _progressBarForeground.IsHidden = false;
        
        // Force a layout update
        Invalidate();
    }
}
