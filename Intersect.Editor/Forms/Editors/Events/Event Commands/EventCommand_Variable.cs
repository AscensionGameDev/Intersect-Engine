using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandVariable : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private bool mLoading;

        private SetVariableCommand mMyCommand;

        public EventCommandVariable(SetVariableCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mLoading = true;
            InitLocalization();

            //Numerics
            cmbNumericCloneGlobalVar.Items.Clear();
            cmbNumericCloneGlobalVar.Items.AddRange(ServerVariableBase.Names);
            cmbNumericClonePlayerVar.Items.Clear();
            cmbNumericClonePlayerVar.Items.AddRange(PlayerVariableBase.Names);
            cmbNumericCloneGuildVar.Items.Clear();
            cmbNumericCloneGuildVar.Items.AddRange(PlayerVariableBase.Names);

            //Booleans
            cmbBooleanCloneGlobalVar.Items.Clear();
            cmbBooleanCloneGlobalVar.Items.AddRange(ServerVariableBase.Names);
            cmbBooleanClonePlayerVar.Items.Clear();
            cmbBooleanClonePlayerVar.Items.AddRange(PlayerVariableBase.Names);
            cmbBooleanCloneGuildVar.Items.Clear();
            cmbBooleanCloneGuildVar.Items.AddRange(PlayerVariableBase.Names);

            if (mMyCommand.VariableType == VariableTypes.ServerVariable)
            {
                rdoGlobalVariable.Checked = true;
            }
            else if (mMyCommand.VariableType == VariableTypes.PlayerVariable)
            {
                rdoPlayerVariable.Checked = true;
            }
            else if (mMyCommand.VariableType == VariableTypes.GuildVariable)
            {
                rdoGuildVariable.Checked = true;
            }

            mLoading = false;
            InitEditor();
        }

        private void InitLocalization()
        {
            grpSetVariable.Text = Strings.EventSetVariable.title;

            grpSelectVariable.Text = Strings.EventSetVariable.label;
            rdoGlobalVariable.Text = Strings.EventSetVariable.global;
            rdoPlayerVariable.Text = Strings.EventSetVariable.player;
            rdoGuildVariable.Text = Strings.EventSetVariable.guild;
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
            optNumericClonePlayerVar.Text = Strings.EventSetVariable.numericcloneplayervariablevalue;
            optNumericCloneGuildVar.Text = Strings.EventSetVariable.numericcloneguildvariablevalue;
            optNumericCloneGlobalVar.Text = Strings.EventSetVariable.numericcloneglobalvariablevalue;
            lblNumericRandomLow.Text = Strings.EventSetVariable.numericrandomlow;
            lblNumericRandomHigh.Text = Strings.EventSetVariable.numericrandomhigh;

            //Booleanic
            grpBooleanVariable.Text = Strings.EventSetVariable.booleanlabel;
            optBooleanTrue.Text = Strings.EventSetVariable.booleantrue;
            optBooleanFalse.Text = Strings.EventSetVariable.booleanfalse;
            optBooleanCloneGlobalVar.Text = Strings.EventSetVariable.booleanccloneglobalvariablevalue;
            optBooleanClonePlayerVar.Text = Strings.EventSetVariable.booleancloneplayervariablevalue;
            optBooleanCloneGuildVar.Text = Strings.EventSetVariable.booleanccloneguildvariablevalue;

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
        }

        private void InitEditor()
        {
            cmbVariable.Items.Clear();
            var varCount = 0;
            if (rdoPlayerVariable.Checked)
            {
                cmbVariable.Items.AddRange(PlayerVariableBase.Names);
                cmbVariable.SelectedIndex = PlayerVariableBase.ListIndex(mMyCommand.VariableId);
            }
            else if (rdoGlobalVariable.Checked)
            {
                cmbVariable.Items.AddRange(ServerVariableBase.Names);
                cmbVariable.SelectedIndex = ServerVariableBase.ListIndex(mMyCommand.VariableId);
            }
            else if (rdoGuildVariable.Checked)
            {
                cmbVariable.Items.AddRange(GuildVariableBase.Names);
                cmbVariable.SelectedIndex = GuildVariableBase.ListIndex(mMyCommand.VariableId);
            }

            chkSyncParty.Checked = mMyCommand.SyncParty;

            UpdateFormElements();
        }

        private void UpdateFormElements()
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
                    var playerVar = PlayerVariableBase.FromList(cmbVariable.SelectedIndex);
                    if (playerVar != null)
                    {
                        varType = (byte) playerVar.Type;
                    }
                }
                else if (rdoGlobalVariable.Checked)
                {
                    var serverVar = ServerVariableBase.FromList(cmbVariable.SelectedIndex);
                    if (serverVar != null)
                    {
                        varType = (byte) serverVar.Type;
                    }
                }
                else if (rdoGuildVariable.Checked)
                {
                    var guildVar = GuildVariableBase.FromList(cmbVariable.SelectedIndex);
                    if (guildVar != null)
                    {
                        varType = (byte)guildVar.Type;
                    }
                }
            }

            //Load the correct editor
            if (varType > 0)
            {
                switch ((VariableDataTypes) varType)
                {
                    case VariableDataTypes.Boolean:
                        grpBooleanVariable.Show();
                        TryLoadBooleanMod(mMyCommand.Modification);

                        break;

                    case VariableDataTypes.Integer:
                        grpNumericVariable.Show();
                        TryLoadNumericMod(mMyCommand.Modification);
                        UpdateNumericFormElements();

                        break;

                    case VariableDataTypes.Number:
                        break;

                    case VariableDataTypes.String:
                        grpStringVariable.Show();
                        TryLoadStringMod(mMyCommand.Modification);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int n;
            if (rdoPlayerVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.PlayerVariable;
                mMyCommand.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }

            if (rdoGlobalVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.ServerVariable;
                mMyCommand.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }

            if (rdoGuildVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.GuildVariable;
                mMyCommand.VariableId = GuildVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }

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

        private void rdoPlayerVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!mLoading && cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }

            if (!mLoading)
            {
                optNumericSet.Checked = true;
            }

            if (!mLoading)
            {
                nudNumericValue.Value = 0;
            }
        }

        private void rdoGlobalVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!mLoading && cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }

            if (!mLoading)
            {
                optNumericSet.Checked = true;
            }

            if (!mLoading)
            {
                nudNumericValue.Value = 0;
            }
        }

        private void rdoGuildVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!mLoading && cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }

            if (!mLoading)
            {
                optNumericSet.Checked = true;
            }

            if (!mLoading)
            {
                nudNumericValue.Value = 0;
            }
        }

        private void cmbVariable_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        #region "Boolean Variable"

        private void TryLoadBooleanMod(VariableMod variableMod)
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
                return;
            }

            if (booleanMod.DupVariableType == VariableTypes.PlayerVariable)
            {
                optBooleanClonePlayerVar.Checked = true;
                cmbBooleanClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(booleanMod.DuplicateVariableId);
            }
            else if (booleanMod.DupVariableType == VariableTypes.ServerVariable)
            {
                optBooleanCloneGlobalVar.Checked = true;
                cmbBooleanCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(booleanMod.DuplicateVariableId);
            }
            else if (booleanMod.DupVariableType == VariableTypes.GuildVariable)
            {
                optBooleanCloneGuildVar.Checked = true;
                cmbBooleanCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(booleanMod.DuplicateVariableId);
            }
        }

        private BooleanVariableMod GetBooleanVariableMod()
        {
            var mod = new BooleanVariableMod
            {
                Value = optBooleanTrue.Checked,
            };

            if (optBooleanClonePlayerVar.Checked)
            {
                mod.DupVariableType = VariableTypes.PlayerVariable;
                mod.DuplicateVariableId = PlayerVariableBase.IdFromList(cmbBooleanClonePlayerVar.SelectedIndex);
            }
            else if (optBooleanCloneGlobalVar.Checked)
            {
                mod.DupVariableType = VariableTypes.ServerVariable;
                mod.DuplicateVariableId = ServerVariableBase.IdFromList(cmbBooleanCloneGlobalVar.SelectedIndex);
            }
            else if (optBooleanCloneGuildVar.Checked)
            {
                mod.DupVariableType = VariableTypes.GuildVariable;
                mod.DuplicateVariableId = GuildVariableBase.IdFromList(cmbBooleanCloneGuildVar.SelectedIndex);
            }

            return mod;
        }

        #endregion

        #region "Numeric Variable"

        private void TryLoadNumericMod(VariableMod variableMod)
        {
            if (variableMod == null)
            {
                variableMod = new IntegerVariableMod();
            }

            if (!(variableMod is IntegerVariableMod integerMod))
            {
                return;
            }

            //Should properly seperate static value, player & global vars into a seperate enum.
            //But technical debt :/
            switch (integerMod.ModType)
            {
                case VariableMods.Set:
                    optNumericSet.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMods.Add:
                    optNumericAdd.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMods.Subtract:
                    optNumericSubtract.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMods.Multiply:
                    optNumericMultiply.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMods.Divide:
                    optNumericDivide.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMods.LeftShift:
                    optNumericLeftShift.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMods.RightShift:
                    optNumericRightShift.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMods.Random:
                    optNumericRandom.Checked = true;
                    nudLow.Value = integerMod.Value;
                    nudHigh.Value = integerMod.HighValue;

                    break;

                case VariableMods.SystemTime:
                    optNumericSystemTime.Checked = true;

                    break;

                case VariableMods.DupPlayerVar:
                    optNumericSet.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.DupGlobalVar:
                    optNumericSet.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.AddPlayerVar:
                    optNumericAdd.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.AddGlobalVar:
                    optNumericAdd.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.SubtractPlayerVar:
                    optNumericSubtract.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.SubtractGlobalVar:
                    optNumericSubtract.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.MultiplyPlayerVar:
                    optNumericMultiply.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.MultiplyGlobalVar:
                    optNumericMultiply.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.DividePlayerVar:
                    optNumericDivide.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.DivideGlobalVar:
                    optNumericDivide.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.LeftShiftPlayerVar:
                    optNumericLeftShift.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.LeftShiftGlobalVar:
                    optNumericLeftShift.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.RightShiftPlayerVar:
                    optNumericRightShift.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.RightShiftGlobalVar:
                    optNumericRightShift.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.DupGuildVar:
                    optNumericSet.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.AddGuildVar:
                    optNumericAdd.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.SubtractGuildVar:
                    optNumericSubtract.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.MultiplyGuildVar:
                    optNumericMultiply.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.DivideGuildVar:
                    optNumericDivide.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.LeftShiftGuildVar:
                    optNumericLeftShift.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMods.RightShiftGuildVar:
                    optNumericRightShift.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;
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
            UpdateNumericFormElements();
        }

        private void optNumericSystemTime_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericClonePlayerVar_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericCloneGlobalVar_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private void optNumericCloneGuildVar_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumericFormElements();
        }

        private IntegerVariableMod GetNumericVariableMod()
        {
            var mod = new IntegerVariableMod();
            if (optNumericSet.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMods.Set;
                mod.Value = (int) nudNumericValue.Value;
            }
            else if (optNumericAdd.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMods.Add;
                mod.Value = (int) nudNumericValue.Value;
            }
            else if (optNumericSubtract.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMods.Subtract;
                mod.Value = (int) nudNumericValue.Value;
            }
            else if (optNumericMultiply.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMods.Multiply;
                mod.Value = (int)nudNumericValue.Value;
            }
            else if (optNumericDivide.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMods.Divide;
                mod.Value = (int)nudNumericValue.Value;
            }
            else if (optNumericLeftShift.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMods.LeftShift;
                mod.Value = (int)nudNumericValue.Value;
            }
            else if (optNumericRightShift.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMods.RightShift;
                mod.Value = (int)nudNumericValue.Value;
            }
            else if (optNumericRandom.Checked)
            {
                mod.ModType = VariableMods.Random;
                mod.Value = (int) nudLow.Value;
                mod.HighValue = (int) nudHigh.Value;
                if (mod.HighValue < mod.Value)
                {
                    var n = mod.Value;
                    mod.Value = mod.HighValue;
                    mod.HighValue = n;
                }
            }
            else if (optNumericSystemTime.Checked)
            {
                mod.ModType = VariableMods.SystemTime;
            }
            else if (optNumericClonePlayerVar.Checked)
            {
                if (optNumericSet.Checked)
                {
                    mod.ModType = VariableMods.DupPlayerVar;
                }
                else if (optNumericAdd.Checked)
                {
                    mod.ModType = VariableMods.AddPlayerVar;
                }
                else if (optNumericSubtract.Checked)
                {
                    mod.ModType = VariableMods.SubtractPlayerVar;
                }
                else if (optNumericMultiply.Checked)
                {
                    mod.ModType = VariableMods.MultiplyPlayerVar;
                }
                else if (optNumericDivide.Checked)
                {
                    mod.ModType = VariableMods.DividePlayerVar;
                }
                else if (optNumericLeftShift.Checked)
                {
                    mod.ModType = VariableMods.LeftShiftPlayerVar;
                }
                else
                {
                    mod.ModType = VariableMods.RightShiftPlayerVar;
                }

                mod.DuplicateVariableId = PlayerVariableBase.IdFromList(cmbNumericClonePlayerVar.SelectedIndex);
            }
            else if (optNumericCloneGlobalVar.Checked)
            {
                if (optNumericSet.Checked)
                {
                    mod.ModType = VariableMods.DupGlobalVar;
                }
                else if (optNumericAdd.Checked)
                {
                    mod.ModType = VariableMods.AddGlobalVar;
                }
                else if (optNumericSubtract.Checked)
                {
                    mod.ModType = VariableMods.SubtractGlobalVar;
                }
                else if (optNumericMultiply.Checked)
                {
                    mod.ModType = VariableMods.MultiplyGlobalVar;
                }
                else if (optNumericDivide.Checked)
                {
                    mod.ModType = VariableMods.DivideGlobalVar;
                }
                else if (optNumericLeftShift.Checked)
                {
                    mod.ModType = VariableMods.LeftShiftGlobalVar;
                }
                else
                {
                    mod.ModType = VariableMods.RightShiftGlobalVar;
                }

                mod.DuplicateVariableId = ServerVariableBase.IdFromList(cmbNumericCloneGlobalVar.SelectedIndex);
            }
            else if (optNumericCloneGuildVar.Checked)
            {
                if (optNumericSet.Checked)
                {
                    mod.ModType = VariableMods.DupGuildVar;
                }
                else if (optNumericAdd.Checked)
                {
                    mod.ModType = VariableMods.AddGuildVar;
                }
                else if (optNumericSubtract.Checked)
                {
                    mod.ModType = VariableMods.SubtractGuildVar;
                }
                else if (optNumericMultiply.Checked)
                {
                    mod.ModType = VariableMods.MultiplyGuildVar;
                }
                else if (optNumericDivide.Checked)
                {
                    mod.ModType = VariableMods.DivideGuildVar;
                }
                else if (optNumericLeftShift.Checked)
                {
                    mod.ModType = VariableMods.LeftShiftGuildVar;
                }
                else
                {
                    mod.ModType = VariableMods.RightShiftGuildVar;
                }

                mod.DuplicateVariableId = GuildVariableBase.IdFromList(cmbNumericCloneGuildVar.SelectedIndex);
            }

            return mod;
        }

        #endregion

        #region "String Variable"

        private void TryLoadStringMod(VariableMod variableMod)
        {
            if (variableMod == null)
            {
                variableMod = new StringVariableMod();
            }

            if (variableMod is StringVariableMod stringMod)
            {
                switch (stringMod.ModType)
                {
                    case VariableMods.Set:
                        optStaticString.Checked = true;
                        txtStringValue.Text = stringMod.Value;

                        break;
                    case VariableMods.Replace:
                        optReplaceString.Checked = true;
                        txtStringFind.Text = stringMod.Value;
                        txtStringReplace.Text = stringMod.Replace;

                        break;
                }
            }
        }

        private StringVariableMod GetStringVariableMod()
        {
            var mod = new StringVariableMod();
            if (optStaticString.Checked)
            {
                mod.ModType = VariableMods.Set;
                mod.Value = txtStringValue.Text;
            }
            else if (optReplaceString.Checked)
            {
                mod.ModType = VariableMods.Replace;
                mod.Value = txtStringFind.Text;
                mod.Replace = txtStringReplace.Text;
            }

            return mod;
        }

        private void lblStringTextVariables_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                "http://www.ascensiongamedev.com/community/topic/749-event-text-variables/"
            );
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
    }

}
