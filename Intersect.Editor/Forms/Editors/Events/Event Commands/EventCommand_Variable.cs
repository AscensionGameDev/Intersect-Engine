using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.Editor.Utilities;
using Intersect.Enums;
using Intersect.Extensions;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Utilities;
using VariableMod = Intersect.Enums.VariableMod;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandVariable : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private bool mLoading;

        private SetVariableCommand mMyCommand;

        private VariableType mSelectedVariableType = VariableType.PlayerVariable;
        private Guid mSelectedVariableId = Guid.Empty;

        private VariableType mSettingVariableType = VariableType.PlayerVariable;
        private Guid mSettingVariableId = Guid.Empty;

        public EventCommandVariable(SetVariableCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mLoading = true;
            InitLocalization();

            mSelectedVariableId = refCommand.VariableId;
            mSelectedVariableType = refCommand.VariableType;

            mLoading = false;
            InitEditor();
        }

        private void InitLocalization()
        {
            grpSetVariable.Text = Strings.EventSetVariable.title;

            grpSelectVariable.Text = Strings.EventSetVariable.label;
            btnSave.Text = Strings.EventSetVariable.okay;
            btnCancel.Text = Strings.EventSetVariable.cancel;
            chkSyncParty.Text = Strings.EventSetVariable.syncparty;

            //Numeric
            grpNumericVariable.Text = Strings.EventSetVariable.numericlabel;
            grpNumericRandom.Text = Strings.EventSetVariable.numericrandomdesc;
            optNumericSet.Text = Strings.EventSetVariable.numericset;
            optNumericAdd.Text = Strings.EventSetVariable.numericadd;
            optNumericSubtract.Text = Strings.EventSetVariable.numericsubtract;
            optNumericMultiply.Text = Strings.EventSetVariable.numericmultiply;
            optNumericDivide.Text = Strings.EventSetVariable.numericdivide;
            optNumericLeftShift.Text = Strings.EventSetVariable.numericleftshift;
            optNumericRightShift.Text = Strings.EventSetVariable.numericrightshift;
            optNumericRandom.Text = Strings.EventSetVariable.numericrandom;
            optNumericSystemTime.Text = Strings.EventSetVariable.numericsystemtime;
            lblNumericRandomLow.Text = Strings.EventSetVariable.numericrandomlow;
            lblNumericRandomHigh.Text = Strings.EventSetVariable.numericrandomhigh;

            //Booleanic
            grpBooleanVariable.Text = Strings.EventSetVariable.booleanlabel;
            optBooleanTrue.Text = Strings.EventSetVariable.booleantrue;
            optBooleanFalse.Text = Strings.EventSetVariable.booleanfalse;

            //String
            grpStringVariable.Text = Strings.EventSetVariable.stringlabel;
            optStaticString.Text = Strings.EventSetVariable.stringset;
            optReplaceString.Text = Strings.EventSetVariable.stringreplace;
            grpStringSet.Text = Strings.EventSetVariable.stringset;
            grpStringReplace.Text = Strings.EventSetVariable.stringreplace;
            lblStringValue.Text = Strings.EventSetVariable.stringsetvalue;
            lblStringFind.Text = Strings.EventSetVariable.stringreplacefind;
            lblStringReplace.Text = Strings.EventSetVariable.stringreplacereplace;
            lblStringTextVariables.Text = Strings.EventSetVariable.stringtip;

            // New variable handling
            rdoBoolVariable.Text = Strings.EventSetVariable.VariableValue;
            rdoVariableValue.Text = Strings.EventSetVariable.VariableValue;

            btnSettingVariableSelector.Text = Strings.VariableSelector.Button;
            btnVarSelector.Text = Strings.VariableSelector.Button;

            grpSettingVariable.Text = Strings.EventSetVariable.SetVariableGroup;
            grpSelectVariable.Text = Strings.EventSetVariable.SelectVariableGroup;

            lblCurrentVar.Text = Strings.VariableSelector.LabelCurrentSelection;
            lblSettingVarCurrentValue.Text = Strings.VariableSelector.LabelCurrentSelection;
        }

        private void InitEditor()
        {
            chkSyncParty.Checked = mMyCommand.SyncParty;

            UpdateFormElements();
        }

        private VariableDataType GetCurrentDataTypeFilter()
        {
            if (mSelectedVariableId != Guid.Empty)
            {
                return mSelectedVariableType.GetRelatedTable().GetVariableType(mSelectedVariableId);
            }
            else
            {
                return 0;
            }
        }

        private void UpdateFormElements()
        {
            //Hide editor windows until we have a variable selected to work with
            grpNumericVariable.Hide();
            grpBooleanVariable.Hide();
            grpStringVariable.Hide();

            var varType = GetCurrentDataTypeFilter();

            //Load the correct editor
            if (varType > 0)
            {
                switch (varType)
                {
                    case VariableDataType.Boolean:
                        grpBooleanVariable.Show();
                        TryLoadBooleanMod(mMyCommand.Modification);

                        break;

                    case VariableDataType.Integer:
                        grpNumericVariable.Show();
                        TryLoadNumericMod(mMyCommand.Modification);
                        UpdateNumericFormElements();

                        break;

                    case VariableDataType.Number:
                        break;

                    case VariableDataType.String:
                        grpStringVariable.Show();
                        TryLoadStringMod(mMyCommand.Modification);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            lblVarSelection.Text = VariableSelectorUtils.GetSelectedVarText(mSelectedVariableType, mSelectedVariableId);
            lblSettingVarSelection.Text = VariableSelectorUtils.GetSelectedVarText(mSettingVariableType, mSettingVariableId);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.VariableType = mSelectedVariableType;
            mMyCommand.VariableId = mSelectedVariableId;

            if (grpNumericVariable.Visible)
            {
                mMyCommand.Modification = GetNumericVariableMod();
            }
            else if (grpBooleanVariable.Visible)
            {
                mMyCommand.Modification = GetBooleanVariableMod();
            }
            else if (grpStringVariable.Visible)
            {
                mMyCommand.Modification = GetStringVariableMod();
            }
            else
            {
                switch (mMyCommand.Modification)
                {
                    case BooleanVariableMod _:
                        mMyCommand.Modification = GetBooleanVariableMod();
                        break;

                    case IntegerVariableMod _:
                        mMyCommand.Modification = GetNumericVariableMod();
                        break;

                    case null:
                        mMyCommand.Modification = new BooleanVariableMod();
                        break;
                }
            }

            mMyCommand.SyncParty = chkSyncParty.Checked;

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        #region "Boolean Variable"

        private void TryLoadBooleanMod(GameObjects.Events.VariableMod variableMod)
        {
            if (variableMod == null)
            {
                variableMod = new BooleanVariableMod();
            }

            if (!(variableMod is BooleanVariableMod booleanMod))
            {
                return;
            }

            optBooleanTrue.Checked = booleanMod.Value;
            optBooleanFalse.Checked = !booleanMod.Value;

            if (booleanMod.DuplicateVariableId == Guid.Empty)
            {
                ResetSettingVariableSelection();
                return;
            }

            rdoBoolVariable.Checked = true;
            mSettingVariableId = booleanMod.DuplicateVariableId;
            mSettingVariableType = booleanMod.DupVariableType;

            lblSettingVarSelection.Text = VariableSelectorUtils.GetSelectedVarText(mSettingVariableType, mSettingVariableId);
        }

        private BooleanVariableMod GetBooleanVariableMod()
        {
            var mod = new BooleanVariableMod
            {
                Value = optBooleanTrue.Checked,
                DuplicateVariableId = mSettingVariableId,
                DupVariableType = mSettingVariableType
            };

            return mod;
        }

        #endregion

        #region "Numeric Variable"

        private void TryLoadNumericMod(GameObjects.Events.VariableMod variableMod)
        {
            if (variableMod == null)
            {
                variableMod = new IntegerVariableMod();
            }

            if (!(variableMod is IntegerVariableMod integerMod))
            {
                return;
            }

            if (integerMod.DuplicateVariableId == Guid.Empty && integerMod.ModType < VariableMod.DupPlayerVar)
            {
                nudNumericValue.Value = integerMod.Value;
                ResetSettingVariableSelection();
            }
            else
            {
                rdoVariableValue.Checked = true;
                mSettingVariableId = integerMod.DuplicateVariableId;
                mSettingVariableType = integerMod.ModType.GetRelatedVariableType();
            }

            optNumericSet.Checked = VariableModUtils.SetMods.Contains(integerMod.ModType);
            optNumericAdd.Checked = VariableModUtils.AddMods.Contains(integerMod.ModType);
            optNumericSubtract.Checked = VariableModUtils.SubMods.Contains(integerMod.ModType);
            optNumericMultiply.Checked = VariableModUtils.MultMods.Contains(integerMod.ModType);
            optNumericDivide.Checked = VariableModUtils.DivideMods.Contains(integerMod.ModType);
            optNumericLeftShift.Checked = VariableModUtils.LShiftMods.Contains(integerMod.ModType);
            optNumericRightShift.Checked = VariableModUtils.RShiftMods.Contains(integerMod.ModType);

            optNumericSystemTime.Checked = integerMod.ModType == VariableMod.SystemTime;

            if (integerMod.ModType == VariableMod.Random)
            {
                optNumericRandom.Checked = true;
                nudLow.Value = integerMod.Value;
                nudHigh.Value = integerMod.HighValue;
            }
        }

        private void UpdateNumericFormElements()
        {
            grpNumericRandom.Visible = optNumericRandom.Checked;
            grpNumericValues.Visible = optNumericAdd.Checked | optNumericSubtract.Checked | optNumericSet.Checked | optNumericMultiply.Checked |
                                       optNumericDivide.Checked | optNumericLeftShift.Checked | optNumericRightShift.Checked;
        }

        private void optNumericSet_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericAdd_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericSubtract_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericMultiply_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericDivide_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericLeftShift_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericRightShift_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericRandom_CheckedChanged(object sender, EventArgs e)
        {
            ResetSettingVariableSelection();
            UpdateNumericFormElements();
        }

        private void optNumericSystemTime_CheckedChanged(object sender, EventArgs e)
        {
            ResetSettingVariableSelection();
            UpdateNumericFormElements();
        }

        private IntegerVariableMod GetNumericVariableMod()
        {
            var mod = new IntegerVariableMod()
            {
                DuplicateVariableId = mSettingVariableId
            };

            mod.Value = optNumericRandom.Checked ? (int)nudLow.Value : (int)nudNumericValue.Value;

            if (optNumericSet.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.Set;
            }
            else if (optNumericAdd.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.Add;
            }
            else if (optNumericSubtract.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.Subtract;
            }
            else if (optNumericMultiply.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.Multiply;
            }
            else if (optNumericDivide.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.Divide;
            }
            else if (optNumericLeftShift.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.LeftShift;
            }
            else if (optNumericRandom.Checked)
            {
                mod.ModType = VariableMod.Random;
                mod.HighValue = (int)nudHigh.Value;
                if (mod.HighValue < mod.Value)
                {
                    var n = mod.Value;
                    mod.Value = mod.HighValue;
                    mod.HighValue = n;
                }
            }
            else if (optNumericSystemTime.Checked)
            {
                mod.ModType = VariableMod.SystemTime;
            }
            else if (mSettingVariableType == VariableType.PlayerVariable)
            {
                if (optNumericSet.Checked)
                {
                    mod.ModType = VariableMod.DupPlayerVar;
                }
                else if (optNumericAdd.Checked)
                {
                    mod.ModType = VariableMod.AddPlayerVar;
                }
                else if (optNumericSubtract.Checked)
                {
                    mod.ModType = VariableMod.SubtractPlayerVar;
                }
                else if (optNumericMultiply.Checked)
                {
                    mod.ModType = VariableMod.MultiplyPlayerVar;
                }
                else if (optNumericDivide.Checked)
                {
                    mod.ModType = VariableMod.DividePlayerVar;
                }
                else if (optNumericLeftShift.Checked)
                {
                    mod.ModType = VariableMod.LeftShiftPlayerVar;
                }
                else
                {
                    mod.ModType = VariableMod.RightShiftPlayerVar;
                }
            }
            else if (mSettingVariableType == VariableType.ServerVariable)
            {
                if (optNumericSet.Checked)
                {
                    mod.ModType = VariableMod.DupGlobalVar;
                }
                else if (optNumericAdd.Checked)
                {
                    mod.ModType = VariableMod.AddGlobalVar;
                }
                else if (optNumericSubtract.Checked)
                {
                    mod.ModType = VariableMod.SubtractGlobalVar;
                }
                else if (optNumericMultiply.Checked)
                {
                    mod.ModType = VariableMod.MultiplyGlobalVar;
                }
                else if (optNumericDivide.Checked)
                {
                    mod.ModType = VariableMod.DivideGlobalVar;
                }
                else if (optNumericLeftShift.Checked)
                {
                    mod.ModType = VariableMod.LeftShiftGlobalVar;
                }
                else
                {
                    mod.ModType = VariableMod.RightShiftGlobalVar;
                }
            }
            else if (mSettingVariableType == VariableType.GuildVariable)
            {
                if (optNumericSet.Checked)
                {
                    mod.ModType = VariableMod.DupGuildVar;
                }
                else if (optNumericAdd.Checked)
                {
                    mod.ModType = VariableMod.AddGuildVar;
                }
                else if (optNumericSubtract.Checked)
                {
                    mod.ModType = VariableMod.SubtractGuildVar;
                }
                else if (optNumericMultiply.Checked)
                {
                    mod.ModType = VariableMod.MultiplyGuildVar;
                }
                else if (optNumericDivide.Checked)
                {
                    mod.ModType = VariableMod.DivideGuildVar;
                }
                else if (optNumericLeftShift.Checked)
                {
                    mod.ModType = VariableMod.LeftShiftGuildVar;
                }
                else
                {
                    mod.ModType = VariableMod.RightShiftGuildVar;
                }
            }
            else if (mSettingVariableType == VariableType.UserVariable)
            {
                if (optNumericSet.Checked)
                {
                    mod.ModType = VariableMod.DuplicateUserVariable;
                }
                else if (optNumericAdd.Checked)
                {
                    mod.ModType = VariableMod.AddUserVariable;
                }
                else if (optNumericSubtract.Checked)
                {
                    mod.ModType = VariableMod.SubtractUserVariable;
                }
                else if (optNumericMultiply.Checked)
                {
                    mod.ModType = VariableMod.MultiplyUserVariable;
                }
                else if (optNumericDivide.Checked)
                {
                    mod.ModType = VariableMod.DivideUserVariable;
                }
                else if (optNumericLeftShift.Checked)
                {
                    mod.ModType = VariableMod.LeftShiftUserVariable;
                }
                else
                {
                    mod.ModType = VariableMod.RightShiftUserVariable;
                }
            }

            return mod;
        }

        #endregion

        #region "String Variable"

        private void TryLoadStringMod(GameObjects.Events.VariableMod variableMod)
        {
            if (variableMod == null)
            {
                variableMod = new StringVariableMod();
            }

            if (variableMod is StringVariableMod stringMod)
            {
                switch (stringMod.ModType)
                {
                    case VariableMod.Set:
                        optStaticString.Checked = true;
                        txtStringValue.Text = stringMod.Value;

                        break;
                    case VariableMod.Replace:
                        optReplaceString.Checked = true;
                        txtStringFind.Text = stringMod.Value;
                        txtStringReplace.Text = stringMod.Replace;

                        break;
                }
            }

            ResetSettingVariableSelection();
        }

        private StringVariableMod GetStringVariableMod()
        {
            var mod = new StringVariableMod();
            if (optStaticString.Checked)
            {
                mod.ModType = VariableMod.Set;
                mod.Value = txtStringValue.Text;
            }
            else if (optReplaceString.Checked)
            {
                mod.ModType = VariableMod.Replace;
                mod.Value = txtStringFind.Text;
                mod.Replace = txtStringReplace.Text;
            }

            return mod;
        }

        private void lblStringTextVariables_Click(object sender, EventArgs e)
        {
            BrowserUtils.Open("http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
        }

        private void UpdateStringFormElements()
        {
            grpStringSet.Visible = optStaticString.Checked;
            grpStringReplace.Visible = optReplaceString.Checked;
        }

        private void optStaticString_CheckedChanged(object sender, EventArgs e)
        {
            UpdateStringFormElements();
        }

        private void optReplaceString_CheckedChanged(object sender, EventArgs e)
        {
            UpdateStringFormElements();
        }

        #endregion

        private void btnVisual_Click(object sender, EventArgs e)
        {
            VariableSelectorUtils.OpenVariableSelector((selection) =>
            {
                mSelectedVariableType = selection.VariableType;
                mSelectedVariableId = selection.VariableId;

                InitEditor();
                ResetSettingVariableSelection();
            }, mSelectedVariableId, mSelectedVariableType);
        }

        private void rdoBoolVariable_CheckedChanged(object sender, EventArgs e)
        {
            grpSettingVariable.Visible = true;
        }

        private void optBooleanTrue_CheckedChanged(object sender, EventArgs e)
        {
            ResetSettingVariableSelection();
        }

        private void optBooleanFalse_CheckedChanged(object sender, EventArgs e)
        {
            ResetSettingVariableSelection();
        }

        private void ResetSettingVariableSelection(bool hide = true)
        {
            grpSettingVariable.Visible = !hide;
            mSettingVariableType = VariableType.PlayerVariable;
            mSettingVariableId = Guid.Empty;
        }

        private void btnSettingVariableSelector_Click(object sender, EventArgs e)
        {
            VariableSelectorUtils.OpenVariableSelector((selection) =>
            {
                mSettingVariableType = selection.VariableType;
                mSettingVariableId = selection.VariableId;

                lblSettingVarSelection.Text = VariableSelectorUtils.GetSelectedVarText(mSettingVariableType, mSettingVariableId);
            }, mSettingVariableId, mSettingVariableType, GetCurrentDataTypeFilter());
        }

        private void rdoVariableValue_CheckedChanged(object sender, EventArgs e)
        {
            grpSettingVariable.Visible = true;
        }

        private void optNumericStaticVal_CheckedChanged(object sender, EventArgs e)
        {
            ResetSettingVariableSelection();
        }
    }

}
