using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Framework.Core.GameObjects.Crafting;

namespace Intersect.Client.Interface.Game.Metiers;

/// <summary>
/// WoW-style skill window: crafting professions first (limited to two), then gathering ones. Learned professions show
/// their rank and a skill bar, the others are greyed out.
/// </summary>
public partial class ProfessionsWindow
{
    private const string FontName = "sourcesansproblack";

    private const int WindowWidth = 340;

    private const int WindowHeight = 420;

    private static readonly Color GoldColor = new(255, 255, 215, 0);

    private static readonly Color DimColor = new(255, 150, 150, 150);

    private static readonly Color HeaderColor = new(255, 255, 220, 120);

    private readonly Base mParent;

    private WindowControl mWindow;

    private Label mSummaryLabel;

    private Label mHintLabel;

    private ScrollControl mScroll;

    private readonly List<ProfessionRow> mRows = [];

    public ProfessionsWindow(Canvas gameCanvas)
    {
        mParent = gameCanvas;

        GenerateControls();
    }

    private void GenerateControls()
    {
        mWindow = new WindowControl(mParent, "Métiers", false, "ProfessionsWindow")
        {
            IsHidden = true,
        };
        mWindow.SetSize(WindowWidth, WindowHeight);
        mWindow.SetPosition(300, 100);
        mWindow.DisableResizing();

        mSummaryLabel = new Label(mWindow, "ProfessionsSummary")
        {
            FontName = FontName,
            FontSize = 12,
        };

        mHintLabel = new Label(mWindow, "ProfessionsHint")
        {
            FontName = FontName,
            FontSize = 9,
        };

        mScroll = new ScrollControl(mWindow, "ProfessionsScroll");
        mScroll.EnableScroll(false, true);
        mScroll.SizeChanged += (_, _) => LayoutRows();

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        // Same frame as the Titles window (about 18 px on the sides, 22 px at the bottom).
        mWindow.SetSize(WindowWidth, WindowHeight);

        mSummaryLabel.Dock = Pos.Top;
        mSummaryLabel.Height = 24;
        mSummaryLabel.Margin = new Margin(18, 8, 22, 2);

        mHintLabel.Dock = Pos.Bottom;
        mHintLabel.Height = 18;
        mHintLabel.Margin = new Margin(18, 2, 22, 22);

        mScroll.Dock = Pos.Fill;
        mScroll.Margin = new Margin(18, 4, 22, 2);
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Show()
    {
        mWindow.Show();
        LayoutRows();
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
            mScroll.RemoveChild(row, true);
        }

        mRows.Clear();

        if (Globals.Me == null)
        {
            return;
        }

        var skills = Globals.Me.ProfessionSkills;
        var craftingCount = skills.Keys.Count(id => Professions.IsCrafting((Profession)id));

        mSummaryLabel.SetText($"Métiers de fabrication : {craftingCount}/{Professions.MaxCraftingProfessions}");
        mSummaryLabel.SetTextColor(HeaderColor, ComponentState.Normal);
        mHintLabel.SetText("Fabrique des objets pour monter en niveau (orange > jaune > vert > gris).");
        mHintLabel.SetTextColor(DimColor, ComponentState.Normal);

        var ordered = Professions.All.Where(Professions.IsCrafting).Concat(Professions.All.Where(p => !Professions.IsCrafting(p)));
        foreach (var profession in ordered)
        {
            var learned = skills.TryGetValue((int)profession, out var skill);
            var row = new ProfessionRow(mScroll, profession, learned, skill);
            mRows.Add(row);
        }

        LayoutRows();
    }

    private void LayoutRows()
    {
        var width = mScroll.Width - (mScroll.VerticalScrollBar?.Width ?? 0) - 2;
        if (width <= 0)
        {
            return;
        }

        const int rowHeight = 40;
        const int gap = 6;
        for (var index = 0; index < mRows.Count; index++)
        {
            mRows[index].SetBounds(0, index * (rowHeight + gap), width, rowHeight);
            mRows[index].LayoutChildren();
        }
    }

    private sealed class ProfessionRow : Base
    {
        private readonly Label mName;

        private readonly Label mValue;

        private readonly SkillBar mBar;

        public ProfessionRow(Base parent, Profession profession, bool learned, int skill) : base(parent, $"Profession{profession}")
        {
            MouseInputEnabled = false;

            var color = learned ? GoldColor : DimColor;

            mName = new Label(this, "Name") { FontName = FontName, FontSize = 12 };
            mName.SetText(Professions.GetName(profession));
            mName.SetTextColor(color, ComponentState.Normal);

            mValue = new Label(this, "Value") { FontName = FontName, FontSize = 10 };
            mValue.SetText(learned ? $"{Professions.GetRankName(skill)}  {skill}/{Professions.MaxSkill}" : "Non appris");
            mValue.SetTextColor(color, ComponentState.Normal);

            mBar = new SkillBar(this)
            {
                Fraction = learned ? Math.Clamp(skill / (float)Professions.MaxSkill, 0f, 1f) : 0f,
                FillColor = Professions.IsCrafting(profession) ? new Color(255, 230, 150, 30) : new Color(255, 70, 170, 70),
            };
        }

        public void LayoutChildren()
        {
            mName.SetPosition(4, 2);
            mValue.SetPosition(Math.Max(4, Width - 130), 4);
            mBar.SetBounds(4, 26, Math.Max(1, Width - 8), 8);
        }
    }

    private sealed class SkillBar : Base
    {
        public float Fraction { get; set; }

        public Color FillColor { get; set; } = Color.White;

        public SkillBar(Base parent) : base(parent, "SkillBar")
        {
            MouseInputEnabled = false;
        }

        protected override void Render(Framework.Gwen.Skin.Base skin)
        {
            var bounds = RenderBounds;
            skin.Renderer.DrawColor = new Color(255, 25, 25, 25);
            skin.Renderer.DrawFilledRect(bounds);

            var fillWidth = (int)(bounds.Width * Fraction);
            if (fillWidth > 0)
            {
                skin.Renderer.DrawColor = FillColor;
                skin.Renderer.DrawFilledRect(new Rectangle(bounds.X, bounds.Y, fillWidth, bounds.Height));
            }
        }
    }
}
