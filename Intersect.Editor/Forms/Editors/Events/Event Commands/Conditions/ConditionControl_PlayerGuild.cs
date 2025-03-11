using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_PlayerGuild : UserControl
{
    public ConditionControl_PlayerGuild()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpInGuild.Text = Strings.EventConditional.inguild;
        lblRank.Text = Strings.EventConditional.rank;
        cmbRank.Items.Clear();

        foreach (var rank in Options.Instance.Guild.Ranks)
        {
            cmbRank.Items.Add(rank.Title);
        }
    }

    public void SetupFormValues(InGuildWithRank condition)
    {
        cmbRank.SelectedIndex = Math.Max(0, Math.Min(Options.Instance.Guild.Ranks.Length - 1, condition.Rank));
    }

    public void SaveFormValues(InGuildWithRank condition)
    {
        condition.Rank = Math.Max(cmbRank.SelectedIndex, 0);
    }
}
