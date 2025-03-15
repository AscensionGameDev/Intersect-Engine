using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_Variable : UserControl
{
    private readonly EventCommandConditionalBranch _root;

    public ConditionControl_Variable(EventCommandConditionalBranch parent)
    {
        _root = parent;
        InitializeComponent();

        nudVariableValue.Minimum = long.MinValue;
        nudVariableValue.Maximum = long.MaxValue;
        nudVariableMaxValue.Minimum = long.MinValue;
        nudVariableMaxValue.Maximum = long.MaxValue;

        InitLocalization();
    }

    #region Basic Handlers

    public void InitLocalization()
    {
        grpVariable.Text = Strings.EventConditional.variable;
        grpSelectVariable.Text = Strings.EventConditional.selectvariable;
        rdoPlayerVariable.Text = Strings.EventConditional.playervariable;
        rdoGlobalVariable.Text = Strings.EventConditional.globalvariable;
        rdoGuildVariable.Text = Strings.EventConditional.guildvariable;
        rdoUserVariable.Text = Strings.GameObjectStrings.UserVariable;

        //Numeric Variable
        grpNumericVariable.Text = Strings.EventConditional.numericvariable;
        lblNumericComparator.Text = Strings.EventConditional.comparator;
        rdoVarCompareStaticValue.Text = Strings.EventConditional.value;
        rdoVarComparePlayerVar.Text = Strings.EventConditional.playervariablevalue;
        rdoVarCompareGlobalVar.Text = Strings.EventConditional.globalvariablevalue;
        rdoVarCompareGuildVar.Text = Strings.EventConditional.guildvariablevalue;
        rdoVarCompareUserVar.Text = Strings.EventConditional.UserVariableValue;
        cmbNumericComparitor.Items.Clear();
        cmbNumericComparitor.Items.AddRange(Strings.EventConditional.comparators.Values.ToArray());
        cmbNumericComparitor.SelectedIndex = 0;

        //Boolean Variable
        grpBooleanVariable.Text = Strings.EventConditional.booleanvariable;
        cmbBooleanComparator.Items.Clear();
        cmbBooleanComparator.Items.Add(Strings.EventConditional.booleanequal);
        cmbBooleanComparator.Items.Add(Strings.EventConditional.booleannotequal);
        cmbBooleanComparator.SelectedIndex = 0;
        optBooleanTrue.Text = Strings.EventConditional.True;
        optBooleanFalse.Text = Strings.EventConditional.False;
        optBooleanGlobalVariable.Text = Strings.EventConditional.globalvariablevalue;
        optBooleanPlayerVariable.Text = Strings.EventConditional.playervariablevalue;
        optBooleanGuildVariable.Text = Strings.EventConditional.guildvariablevalue;
        optBooleanUserVariable.Text = Strings.EventConditional.UserVariableValue;

        //String Variable
        grpStringVariable.Text = Strings.EventConditional.stringvariable;
        cmbStringComparitor.Items.Clear();
        for (var i = 0; i < Strings.EventConditional.stringcomparators.Count; i++)
        {
            cmbStringComparitor.Items.Add(Strings.EventConditional.stringcomparators[i]);
        }

        cmbStringComparitor.SelectedIndex = 0;
        lblStringComparator.Text = Strings.EventConditional.comparator;
        lblStringComparatorValue.Text = Strings.EventConditional.value;
        lblStringTextVariables.Text = Strings.EventConditional.stringtip;
    }

    public void SetupFormValues(VariableIsCondition condition)
    {
        if (condition.VariableType == VariableType.PlayerVariable)
        {
            rdoPlayerVariable.Checked = true;
        }
        else if (condition.VariableType == VariableType.ServerVariable)
        {
            rdoGlobalVariable.Checked = true;
        }
        else if (condition.VariableType == VariableType.GuildVariable)
        {
            rdoGuildVariable.Checked = true;
        }
        else if (condition.VariableType == VariableType.UserVariable)
        {
            rdoUserVariable.Checked = true;
        }

        InitVariableElements(condition.VariableId);
        UpdateVariableElements();
    }

    public void SaveFormValues(VariableIsCondition condition)
    {
        if (rdoGlobalVariable.Checked)
        {
            condition.VariableType = VariableType.ServerVariable;
            condition.VariableId = ServerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex);
        }
        else if (rdoPlayerVariable.Checked)
        {
            condition.VariableType = VariableType.PlayerVariable;
            condition.VariableId = PlayerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex);
        }
        else if (rdoGuildVariable.Checked)
        {
            condition.VariableType = VariableType.GuildVariable;
            condition.VariableId = GuildVariableDescriptor.IdFromList(cmbVariable.SelectedIndex);
        }
        else if (rdoUserVariable.Checked)
        {
            condition.VariableType = VariableType.UserVariable;
            condition.VariableId = UserVariableDescriptor.IdFromList(cmbVariable.SelectedIndex);
        }

        if (grpBooleanVariable.Visible)
        {
            condition.Comparison = GetBooleanVariableComparison();
        }
        else if (grpNumericVariable.Visible)
        {
            condition.Comparison = GetNumericVariableComparison();
        }
        else if (grpStringVariable.Visible)
        {
            condition.Comparison = GetStringVariableComparison();
        }
        else
        {
            condition.Comparison = new VariableComparison();
        }
    }

    public new void Show()
    {
        cmbCompareGlobalVar.Items.Clear();
        cmbCompareGlobalVar.Items.AddRange(ServerVariableDescriptor.Names);
        cmbComparePlayerVar.Items.Clear();
        cmbComparePlayerVar.Items.AddRange(PlayerVariableDescriptor.Names);
        cmbCompareGuildVar.Items.Clear();
        cmbCompareGuildVar.Items.AddRange(GuildVariableDescriptor.Names);
        cmbCompareUserVar.Items.Clear();
        cmbCompareUserVar.Items.AddRange(UserVariableDescriptor.Names);

        cmbBooleanGlobalVariable.Items.Clear();
        cmbBooleanGlobalVariable.Items.AddRange(ServerVariableDescriptor.Names);
        cmbBooleanPlayerVariable.Items.Clear();
        cmbBooleanPlayerVariable.Items.AddRange(PlayerVariableDescriptor.Names);
        cmbBooleanGuildVariable.Items.Clear();
        cmbBooleanGuildVariable.Items.AddRange(GuildVariableDescriptor.Names);
        cmbBooleanUserVariable.Items.Clear();
        cmbBooleanUserVariable.Items.AddRange(UserVariableDescriptor.Names);

        base.Show();
    }

    #endregion

    #region Setup Handlers

    private void InitVariableElements(Guid variableId)
    {
        _root.Loading = true;
        cmbVariable.Items.Clear();
        if (rdoPlayerVariable.Checked)
        {
            cmbVariable.Items.AddRange(PlayerVariableDescriptor.Names);
            cmbVariable.SelectedIndex = PlayerVariableDescriptor.ListIndex(variableId);
        }
        else if (rdoGlobalVariable.Checked)
        {
            cmbVariable.Items.AddRange(ServerVariableDescriptor.Names);
            cmbVariable.SelectedIndex = ServerVariableDescriptor.ListIndex(variableId);
        }
        else if (rdoGuildVariable.Checked)
        {
            cmbVariable.Items.AddRange(GuildVariableDescriptor.Names);
            cmbVariable.SelectedIndex = GuildVariableDescriptor.ListIndex(variableId);
        }
        else if (rdoUserVariable.Checked)
        {
            cmbVariable.Items.AddRange(UserVariableDescriptor.Names);
            cmbVariable.SelectedIndex = UserVariableDescriptor.ListIndex(variableId);
        }

        _root.Loading = false;
    }

    private void UpdateVariableElements()
    {
        //Hide editor windows until we have a variable selected to work with
        grpNumericVariable.Hide();
        grpBooleanVariable.Hide();
        grpStringVariable.Hide();

        var varType = 0;
        if (cmbVariable.SelectedIndex > -1)
        {
            //Determine Variable Type
            if (rdoPlayerVariable.Checked)
            {
                var playerVar = PlayerVariableDescriptor.FromList(cmbVariable.SelectedIndex);
                if (playerVar != null)
                {
                    varType = (byte)playerVar.DataType;
                }
            }
            else if (rdoGlobalVariable.Checked)
            {
                var serverVar = ServerVariableDescriptor.FromList(cmbVariable.SelectedIndex);
                if (serverVar != null)
                {
                    varType = (byte)serverVar.DataType;
                }
            }
            else if (rdoGuildVariable.Checked)
            {
                var guildVar = GuildVariableDescriptor.FromList(cmbVariable.SelectedIndex);
                if (guildVar != null)
                {
                    varType = (byte)guildVar.DataType;
                }
            }
            else if (rdoUserVariable.Checked)
            {
                var userVar = UserVariableDescriptor.FromList(cmbVariable.SelectedIndex);
                if (userVar != null)
                {
                    varType = (byte)userVar.DataType;
                }
            }
        }

        //Load the correct editor
        if (varType > 0)
        {
            switch ((VariableDataType)varType)
            {
                case VariableDataType.Boolean:
                    grpBooleanVariable.Show();
                    TryLoadVariableBooleanComparison(((VariableIsCondition)_root.Condition).Comparison);

                    break;

                case VariableDataType.Integer:
                    grpNumericVariable.Show();
                    TryLoadVariableIntegerComparison(((VariableIsCondition)_root.Condition).Comparison);
                    UpdateNumericVariableElements();

                    break;

                case VariableDataType.Number:
                    break;

                case VariableDataType.String:
                    grpStringVariable.Show();
                    TryLoadVariableStringComparison(((VariableIsCondition)_root.Condition).Comparison);

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void TryLoadVariableBooleanComparison(VariableComparison comparison)
    {
        if (comparison is not BooleanVariableComparison booleanComparison)
        {
            return;
        }

        cmbBooleanComparator.SelectedIndex = Convert.ToInt32(!booleanComparison.ComparingEqual);

        if (cmbBooleanComparator.SelectedIndex < 0)
        {
            cmbBooleanComparator.SelectedIndex = 0;
        }

        optBooleanTrue.Checked = booleanComparison.Value;
        optBooleanFalse.Checked = !booleanComparison.Value;

        if (booleanComparison.CompareVariableId != Guid.Empty)
        {
            if (booleanComparison.CompareVariableType == VariableType.PlayerVariable)
            {
                optBooleanPlayerVariable.Checked = true;
                cmbBooleanPlayerVariable.SelectedIndex = PlayerVariableDescriptor.ListIndex(booleanComparison.CompareVariableId);
            }
            else if (booleanComparison.CompareVariableType == VariableType.ServerVariable)
            {
                optBooleanGlobalVariable.Checked = true;
                cmbBooleanGlobalVariable.SelectedIndex = ServerVariableDescriptor.ListIndex(booleanComparison.CompareVariableId);
            }
            else if (booleanComparison.CompareVariableType == VariableType.GuildVariable)
            {
                optBooleanGuildVariable.Checked = true;
                cmbBooleanGuildVariable.SelectedIndex = GuildVariableDescriptor.ListIndex(booleanComparison.CompareVariableId);
            }
            else if (booleanComparison.CompareVariableType == VariableType.UserVariable)
            {
                optBooleanUserVariable.Checked = true;
                cmbBooleanUserVariable.SelectedIndex = UserVariableDescriptor.ListIndex(booleanComparison.CompareVariableId);
            }
        }
    }

    private void TryLoadVariableIntegerComparison(VariableComparison comparison)
    {
        if (comparison is not IntegerVariableComparison integerComparison)
        {
            return;
        }

        cmbNumericComparitor.SelectedIndex = (int)integerComparison.Comparator;

        if (cmbNumericComparitor.SelectedIndex < 0)
        {
            cmbNumericComparitor.SelectedIndex = 0;
        }

        if (integerComparison.CompareVariableId != Guid.Empty)
        {
            if (integerComparison.CompareVariableType == VariableType.PlayerVariable)
            {
                rdoVarComparePlayerVar.Checked = true;
                cmbComparePlayerVar.SelectedIndex = PlayerVariableDescriptor.ListIndex(integerComparison.CompareVariableId);
            }
            else if (integerComparison.CompareVariableType == VariableType.ServerVariable)
            {
                rdoVarCompareGlobalVar.Checked = true;
                cmbCompareGlobalVar.SelectedIndex = ServerVariableDescriptor.ListIndex(integerComparison.CompareVariableId);
            }
            else if (integerComparison.CompareVariableType == VariableType.GuildVariable)
            {
                rdoVarCompareGuildVar.Checked = true;
                cmbCompareGuildVar.SelectedIndex = GuildVariableDescriptor.ListIndex(integerComparison.CompareVariableId);
            }
            else if (integerComparison.CompareVariableType == VariableType.UserVariable)
            {
                rdoVarCompareUserVar.Checked = true;
                cmbCompareUserVar.SelectedIndex = UserVariableDescriptor.ListIndex(integerComparison.CompareVariableId);
            }
        }
        else if (integerComparison.TimeSystem)
        {
            rdoTimeSystem.Checked = true;
        }
        else
        {
            rdoVarCompareStaticValue.Checked = true;
            nudVariableValue.Value = integerComparison.Value;
            nudVariableMaxValue.Value = integerComparison.MaxValue;
        }

        UpdateNumericVariableElements();
    }

    private void TryLoadVariableStringComparison(VariableComparison comparison)
    {
        if (comparison is not StringVariableComparison stringComparison)
        {
            return;
        }

        cmbStringComparitor.SelectedIndex = Convert.ToInt32(stringComparison.Comparator);

        if (cmbStringComparitor.SelectedIndex < 0)
        {
            cmbStringComparitor.SelectedIndex = 0;
        }

        txtStringValue.Text = stringComparison.Value;
    }

    private BooleanVariableComparison GetBooleanVariableComparison()
    {
        if (cmbBooleanComparator.SelectedIndex < 0)
        {
            cmbBooleanComparator.SelectedIndex = 0;
        }

        var comparison = new BooleanVariableComparison
        {
            ComparingEqual = !Convert.ToBoolean(cmbBooleanComparator.SelectedIndex),
            Value = optBooleanTrue.Checked,
        };

        if (optBooleanGlobalVariable.Checked)
        {
            comparison.CompareVariableType = VariableType.ServerVariable;
            comparison.CompareVariableId = ServerVariableDescriptor.IdFromList(cmbBooleanGlobalVariable.SelectedIndex);
        }
        else if (optBooleanPlayerVariable.Checked)
        {
            comparison.CompareVariableType = VariableType.PlayerVariable;
            comparison.CompareVariableId = PlayerVariableDescriptor.IdFromList(cmbBooleanPlayerVariable.SelectedIndex);
        }
        else if (optBooleanGuildVariable.Checked)
        {
            comparison.CompareVariableType = VariableType.GuildVariable;
            comparison.CompareVariableId = GuildVariableDescriptor.IdFromList(cmbBooleanGuildVariable.SelectedIndex);
        }
        else if (optBooleanUserVariable.Checked)
        {
            comparison.CompareVariableType = VariableType.UserVariable;
            comparison.CompareVariableId = UserVariableDescriptor.IdFromList(cmbBooleanUserVariable.SelectedIndex);
        }

        return comparison;
    }

    private IntegerVariableComparison GetNumericVariableComparison()
    {
        if (cmbNumericComparitor.SelectedIndex < 0)
        {
            cmbNumericComparitor.SelectedIndex = 0;
        }

        var comparison = new IntegerVariableComparison
        {
            Comparator = (VariableComparator)cmbNumericComparitor.SelectedIndex,
            CompareVariableId = Guid.Empty,
            TimeSystem = false,
        };

        if (rdoVarCompareStaticValue.Checked)
        {
            comparison.Value = (long)nudVariableValue.Value;
            if (comparison.Comparator == VariableComparator.Between)
            {
                comparison.MaxValue = (long)nudVariableMaxValue.Value;
            }
        }
        else if (rdoVarCompareGlobalVar.Checked)
        {
            comparison.CompareVariableType = VariableType.ServerVariable;
            comparison.CompareVariableId = ServerVariableDescriptor.IdFromList(cmbCompareGlobalVar.SelectedIndex);
        }
        else if (rdoVarComparePlayerVar.Checked)
        {
            comparison.CompareVariableType = VariableType.PlayerVariable;
            comparison.CompareVariableId = PlayerVariableDescriptor.IdFromList(cmbComparePlayerVar.SelectedIndex);
        }
        else if (rdoVarCompareGuildVar.Checked)
        {
            comparison.CompareVariableType = VariableType.GuildVariable;
            comparison.CompareVariableId = GuildVariableDescriptor.IdFromList(cmbCompareGuildVar.SelectedIndex);
        }
        else if (rdoVarCompareUserVar.Checked)
        {
            comparison.CompareVariableType = VariableType.UserVariable;
            comparison.CompareVariableId = UserVariableDescriptor.IdFromList(cmbCompareUserVar.SelectedIndex);
        }
        else
        {
            comparison.TimeSystem = true;
        }

        return comparison;
    }

    private StringVariableComparison GetStringVariableComparison()
    {
        if (cmbStringComparitor.SelectedIndex < 0)
        {
            cmbStringComparitor.SelectedIndex = 0;
        }

        var comparison = new StringVariableComparison
        {
            Comparator = (StringVariableComparator)cmbStringComparitor.SelectedIndex,
            Value = txtStringValue.Text,
        };

        return comparison;
    }

    private void UpdateNumericVariableElements()
    {
        nudVariableValue.Enabled = rdoVarCompareStaticValue.Checked;
        nudVariableMaxValue.Enabled = 
            rdoVarCompareStaticValue.Checked &&
            cmbNumericComparitor.SelectedIndex == (int)VariableComparator.Between;

        var isBetween = 
            rdoVarCompareStaticValue.Checked &&
            cmbNumericComparitor.SelectedIndex == (int)VariableComparator.Between;

        rdoVarComparePlayerVar.Enabled = rdoVarComparePlayerVar.Checked || !isBetween;
        cmbComparePlayerVar.Enabled = rdoVarComparePlayerVar.Checked;
        rdoVarCompareGlobalVar.Enabled = rdoVarCompareGlobalVar.Checked || !isBetween;
        cmbCompareGlobalVar.Enabled = rdoVarCompareGlobalVar.Checked;
        rdoVarCompareGuildVar.Enabled = rdoVarCompareGuildVar.Checked || !isBetween;
        cmbCompareGuildVar.Enabled = rdoVarCompareGuildVar.Checked;
        rdoVarCompareUserVar.Enabled = rdoVarCompareUserVar.Checked || !isBetween;
        cmbCompareUserVar.Enabled = rdoVarCompareUserVar.Checked;
        rdoTimeSystem.Enabled = rdoTimeSystem.Checked || !isBetween;
    }

    #endregion

    #region Event Handlers

    private void rdoVarCompareStaticValue_CheckedChanged(object sender, EventArgs e)
    {
        UpdateNumericVariableElements();
    }

    private void rdoVarComparePlayerVar_CheckedChanged(object sender, EventArgs e)
    {
        UpdateNumericVariableElements();
    }

    private void rdoVarCompareGlobalVar_CheckedChanged(object sender, EventArgs e)
    {
        UpdateNumericVariableElements();
    }

    private void rdoVarCompareGuildVar_CheckedChanged(object sender, EventArgs e)
    {
        UpdateNumericVariableElements();
    }

    private void rdoVarCompareUserVar_CheckedChanged(object sender, EventArgs e)
    {
        UpdateNumericVariableElements();
    }

    private void rdoTimeSystem_CheckedChanged(object sender, EventArgs e)
    {
        UpdateNumericVariableElements();
    }

    private void rdoPlayerVariable_CheckedChanged(object sender, EventArgs e)
    {
        InitVariableElements(Guid.Empty);
        if (!_root.Loading && cmbVariable.Items.Count > 0)
        {
            cmbVariable.SelectedIndex = 0;
        }
    }

    private void rdoGlobalVariable_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();
    }

    private void rdoGuildVariable_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();
    }

    private void rdoUserVariable_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();
    }

    private void VariableRadioChanged()
    {
        InitVariableElements(Guid.Empty);
        if (!_root.Loading && cmbVariable.Items.Count > 0)
        {
            cmbVariable.SelectedIndex = 0;
        }
    }

    private void cmbVariable_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_root.Loading)
        {
            return;
        }

        if (rdoPlayerVariable.Checked)
        {
            InitVariableElements(PlayerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex));
        }
        else if (rdoGlobalVariable.Checked)
        {
            InitVariableElements(ServerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex));
        }
        else if (rdoGuildVariable.Checked)
        {
            InitVariableElements(GuildVariableDescriptor.IdFromList(cmbVariable.SelectedIndex));
        }
        else if (rdoUserVariable.Checked)
        {
            InitVariableElements(UserVariableDescriptor.IdFromList(cmbVariable.SelectedIndex));
        }

        UpdateVariableElements();
    }

    private void lblStringTextVariables_Click(object sender, EventArgs e)
    {
        BrowserUtils.Open("http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
    }

    private void cmbNumericComparitor_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbNumericComparitor.SelectedIndex == (int)VariableComparator.Between && !rdoVarCompareStaticValue.Checked)
        {
            rdoVarCompareStaticValue.Checked = true;
        }

        UpdateNumericVariableElements();
    }

    #endregion
}
