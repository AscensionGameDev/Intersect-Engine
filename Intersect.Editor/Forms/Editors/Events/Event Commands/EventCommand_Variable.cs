using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using VariableMod = Intersect.Enums.VariableMod;

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
            cmbNumericClonePlayerVar.Items.Clear();
            cmbNumericClonePlayerVar.Items.AddRange(PlayerVariableBase.Names);
            cmbNumericCloneGlobalVar.Items.Clear();
            cmbNumericCloneGlobalVar.Items.AddRange(ServerVariableBase.Names);
            cmbNumericCloneGuildVar.Items.Clear();
            cmbNumericCloneGuildVar.Items.AddRange(GuildVariableBase.Names);
            cmbNumericCloneUserVar.Items.Clear();
            cmbNumericCloneUserVar.Items.AddRange(UserVariableBase.Names);

            //Booleans
            cmbBooleanClonePlayerVar.Items.Clear();
            cmbBooleanClonePlayerVar.Items.AddRange(PlayerVariableBase.Names);
            cmbBooleanCloneGlobalVar.Items.Clear();
            cmbBooleanCloneGlobalVar.Items.AddRange(ServerVariableBase.Names);
            cmbBooleanCloneGuildVar.Items.Clear();
            cmbBooleanCloneGuildVar.Items.AddRange(GuildVariableBase.Names);
            cmbBooleanCloneUserVar.Items.Clear();
            cmbBooleanCloneUserVar.Items.AddRange(UserVariableBase.Names);

            if (mMyCommand.VariableType == VariableType.ServerVariable)
            {
                rdoGlobalVariable.Checked = true;
            }
            else if (mMyCommand.VariableType == VariableType.PlayerVariable)
            {
                rdoPlayerVariable.Checked = true;
            }
            else if (mMyCommand.VariableType == VariableType.GuildVariable)
            {
                rdoGuildVariable.Checked = true;
            }
            else if (mMyCommand.VariableType == VariableType.UserVariable)
            {
                rdoUserVariable.Checked = true;
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
            rdoUserVariable.Text = Strings.GameObjectStrings.UserVariable;
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
            optNumericCloneUserVar.Text = Strings.EventSetVariable.NumericCloneUserVariableValue;
            lblNumericRandomLow.Text = Strings.EventSetVariable.numericrandomlow;
            lblNumericRandomHigh.Text = Strings.EventSetVariable.numericrandomhigh;

            //Booleanic
            grpBooleanVariable.Text = Strings.EventSetVariable.booleanlabel;
            optBooleanTrue.Text = Strings.EventSetVariable.booleantrue;
            optBooleanFalse.Text = Strings.EventSetVariable.booleanfalse;
            optBooleanCloneGlobalVar.Text = Strings.EventSetVariable.booleanccloneglobalvariablevalue;
            optBooleanClonePlayerVar.Text = Strings.EventSetVariable.booleancloneplayervariablevalue;
            optBooleanCloneGuildVar.Text = Strings.EventSetVariable.booleanccloneguildvariablevalue;
            optBooleanCloneUserVar.Text = Strings.EventSetVariable.BooleanCloneUserVariableValue;

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
            else if (rdoUserVariable.Checked)
            {
                cmbVariable.Items.AddRange(UserVariableBase.Names);
                cmbVariable.SelectedIndex = UserVariableBase.ListIndex(mMyCommand.VariableId);
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
                else if (rdoUserVariable.Checked)
                {
                    var userVar = UserVariableBase.FromList(cmbVariable.SelectedIndex);
                    if (userVar != null)
                    {
                        varType = (byte)userVar.DataType;
                    }
                }
            }

            //Load the correct editor
            if (varType > 0)
            {
                switch ((VariableDataType) varType)
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
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int n;
            if (rdoPlayerVariable.Checked)
            {
                mMyCommand.VariableType = VariableType.PlayerVariable;
                mMyCommand.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }

            if (rdoGlobalVariable.Checked)
            {
                mMyCommand.VariableType = VariableType.ServerVariable;
                mMyCommand.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }

            if (rdoGuildVariable.Checked)
            {
                mMyCommand.VariableType = VariableType.GuildVariable;
                mMyCommand.VariableId = GuildVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }

            if (rdoUserVariable.Checked)
            {
                mMyCommand.VariableType = VariableType.UserVariable;
                mMyCommand.VariableId = UserVariableBase.IdFromList(cmbVariable.SelectedIndex);
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
            VariableRadioChanged();
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
                return;
            }

            if (booleanMod.DupVariableType == VariableType.PlayerVariable)
            {
                optBooleanClonePlayerVar.Checked = true;
                cmbBooleanClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(booleanMod.DuplicateVariableId);
            }
            else if (booleanMod.DupVariableType == VariableType.ServerVariable)
            {
                optBooleanCloneGlobalVar.Checked = true;
                cmbBooleanCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(booleanMod.DuplicateVariableId);
            }
            else if (booleanMod.DupVariableType == VariableType.GuildVariable)
            {
                optBooleanCloneGuildVar.Checked = true;
                cmbBooleanCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(booleanMod.DuplicateVariableId);
            }
            else if (booleanMod.DupVariableType == VariableType.UserVariable)
            {
                optBooleanCloneUserVar.Checked = true;
                cmbBooleanCloneUserVar.SelectedIndex = UserVariableBase.ListIndex(booleanMod.DuplicateVariableId);
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
                mod.DupVariableType = VariableType.PlayerVariable;
                mod.DuplicateVariableId = PlayerVariableBase.IdFromList(cmbBooleanClonePlayerVar.SelectedIndex);
            }
            else if (optBooleanCloneGlobalVar.Checked)
            {
                mod.DupVariableType = VariableType.ServerVariable;
                mod.DuplicateVariableId = ServerVariableBase.IdFromList(cmbBooleanCloneGlobalVar.SelectedIndex);
            }
            else if (optBooleanCloneGuildVar.Checked)
            {
                mod.DupVariableType = VariableType.GuildVariable;
                mod.DuplicateVariableId = GuildVariableBase.IdFromList(cmbBooleanCloneGuildVar.SelectedIndex);
            }
            else if (optBooleanCloneUserVar.Checked)
            {
                mod.DupVariableType = VariableType.UserVariable;
                mod.DuplicateVariableId = UserVariableBase.IdFromList(cmbBooleanCloneUserVar.SelectedIndex);
            }

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

            //Should properly seperate static value, player & global vars into a seperate enum.
            //But technical debt :/
            switch (integerMod.ModType)
            {
                case VariableMod.Set:
                    optNumericSet.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMod.Add:
                    optNumericAdd.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMod.Subtract:
                    optNumericSubtract.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMod.Multiply:
                    optNumericMultiply.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMod.Divide:
                    optNumericDivide.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMod.LeftShift:
                    optNumericLeftShift.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMod.RightShift:
                    optNumericRightShift.Checked = true;
                    optNumericStaticVal.Checked = true;
                    nudNumericValue.Value = integerMod.Value;

                    break;

                case VariableMod.Random:
                    optNumericRandom.Checked = true;
                    nudLow.Value = integerMod.Value;
                    nudHigh.Value = integerMod.HighValue;

                    break;

                case VariableMod.SystemTime:
                    optNumericSystemTime.Checked = true;

                    break;

                //Player variable modifications
                case VariableMod.DupPlayerVar:
                    optNumericSet.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.AddPlayerVar:
                    optNumericAdd.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.SubtractPlayerVar:
                    optNumericSubtract.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.MultiplyPlayerVar:
                    optNumericMultiply.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.DividePlayerVar:
                    optNumericDivide.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.LeftShiftPlayerVar:
                    optNumericLeftShift.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.RightShiftPlayerVar:
                    optNumericRightShift.Checked = true;
                    optNumericClonePlayerVar.Checked = true;
                    cmbNumericClonePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                //Global variable modifications
                case VariableMod.DupGlobalVar:
                    optNumericSet.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.AddGlobalVar:
                    optNumericAdd.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.SubtractGlobalVar:
                    optNumericSubtract.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.MultiplyGlobalVar:
                    optNumericMultiply.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.DivideGlobalVar:
                    optNumericDivide.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.LeftShiftGlobalVar:
                    optNumericLeftShift.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.RightShiftGlobalVar:
                    optNumericRightShift.Checked = true;
                    optNumericCloneGlobalVar.Checked = true;
                    cmbNumericCloneGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                //Guild variable modifications
                case VariableMod.DupGuildVar:
                    optNumericSet.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.AddGuildVar:
                    optNumericAdd.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.SubtractGuildVar:
                    optNumericSubtract.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.MultiplyGuildVar:
                    optNumericMultiply.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.DivideGuildVar:
                    optNumericDivide.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.LeftShiftGuildVar:
                    optNumericLeftShift.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.RightShiftGuildVar:
                    optNumericRightShift.Checked = true;
                    optNumericCloneGuildVar.Checked = true;
                    cmbNumericCloneGuildVar.SelectedIndex = GuildVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                //User variable modifications
                case VariableMod.DuplicateUserVariable:
                    optNumericSet.Checked = true;
                    optNumericCloneUserVar.Checked = true;
                    cmbNumericCloneUserVar.SelectedIndex = UserVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.AddUserVariable:
                    optNumericAdd.Checked = true;
                    optNumericCloneUserVar.Checked = true;
                    cmbNumericCloneUserVar.SelectedIndex = UserVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.SubtractUserVariable:
                    optNumericSubtract.Checked = true;
                    optNumericCloneUserVar.Checked = true;
                    cmbNumericCloneUserVar.SelectedIndex = UserVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.MultiplyUserVariable:
                    optNumericMultiply.Checked = true;
                    optNumericCloneUserVar.Checked = true;
                    cmbNumericCloneUserVar.SelectedIndex = UserVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.DivideUserVariable:
                    optNumericDivide.Checked = true;
                    optNumericCloneUserVar.Checked = true;
                    cmbNumericCloneUserVar.SelectedIndex = UserVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.LeftShiftUserVariable:
                    optNumericLeftShift.Checked = true;
                    optNumericCloneUserVar.Checked = true;
                    cmbNumericCloneUserVar.SelectedIndex = UserVariableBase.ListIndex(integerMod.DuplicateVariableId);

                    break;

                case VariableMod.RightShiftUserVariable:
                    optNumericRightShift.Checked = true;
                    optNumericCloneUserVar.Checked = true;
                    cmbNumericCloneUserVar.SelectedIndex = UserVariableBase.ListIndex(integerMod.DuplicateVariableId);

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
                mod.ModType = VariableMod.Set;
                mod.Value = (int) nudNumericValue.Value;
            }
            else if (optNumericAdd.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.Add;
                mod.Value = (int) nudNumericValue.Value;
            }
            else if (optNumericSubtract.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.Subtract;
                mod.Value = (int) nudNumericValue.Value;
            }
            else if (optNumericMultiply.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.Multiply;
                mod.Value = (int)nudNumericValue.Value;
            }
            else if (optNumericDivide.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.Divide;
                mod.Value = (int)nudNumericValue.Value;
            }
            else if (optNumericLeftShift.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.LeftShift;
                mod.Value = (int)nudNumericValue.Value;
            }
            else if (optNumericRightShift.Checked && optNumericStaticVal.Checked)
            {
                mod.ModType = VariableMod.RightShift;
                mod.Value = (int)nudNumericValue.Value;
            }
            else if (optNumericRandom.Checked)
            {
                mod.ModType = VariableMod.Random;
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
                mod.ModType = VariableMod.SystemTime;
            }
            else if (optNumericClonePlayerVar.Checked)
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

                mod.DuplicateVariableId = PlayerVariableBase.IdFromList(cmbNumericClonePlayerVar.SelectedIndex);
            }
            else if (optNumericCloneGlobalVar.Checked)
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

                mod.DuplicateVariableId = ServerVariableBase.IdFromList(cmbNumericCloneGlobalVar.SelectedIndex);
            }
            else if (optNumericCloneGuildVar.Checked)
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

                mod.DuplicateVariableId = GuildVariableBase.IdFromList(cmbNumericCloneGuildVar.SelectedIndex);
            }
            else if (optNumericCloneUserVar.Checked)
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

                mod.DuplicateVariableId = UserVariableBase.IdFromList(cmbNumericCloneUserVar.SelectedIndex);
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
