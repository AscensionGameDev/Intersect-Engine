using System;
using System.Linq;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandConditionalBranch : UserControl
    {

        private readonly FrmEvent mEventEditor;

        public bool Cancelled;

        public Condition Condition;

        private EventPage mCurrentPage;

        private ConditionalBranchCommand mEventCommand;

        private bool mLoading = false;

        public EventCommandConditionalBranch(
            Condition refCommand,
            EventPage refPage,
            FrmEvent editor,
            ConditionalBranchCommand command
        )
        {
            InitializeComponent();
            mLoading = true;
            if (refCommand == null)
            {
                refCommand = new VariableIsCondition();
            }

            Condition = refCommand;
            mEventEditor = editor;
            mEventCommand = command;
            mCurrentPage = refPage;
            UpdateFormElements(refCommand.Type);
            InitLocalization();
            var typeIndex = 0;
            foreach (var itm in Strings.EventConditional.conditions)
            {
                if (itm.Key == (int) Condition.Type)
                {
                    cmbConditionType.SelectedIndex = typeIndex;

                    break;
                }

                typeIndex++;
            }

            nudVariableValue.Minimum = long.MinValue;
            nudVariableValue.Maximum = long.MaxValue;
            chkNegated.Checked = refCommand.Negated;
            chkHasElse.Checked = refCommand.ElseEnabled;
            SetupFormValues((dynamic) refCommand);
            mLoading = false;
        }

        private void InitLocalization()
        {
            grpConditional.Text = Strings.EventConditional.title;
            lblType.Text = Strings.EventConditional.type;

            cmbConditionType.Items.Clear();
            foreach (var itm in Strings.EventConditional.conditions)
            {
                cmbConditionType.Items.Add(itm.Value);
            }

            chkNegated.Text = Strings.EventConditional.negated;
            chkHasElse.Text = Strings.EventConditional.HasElse;

            //Variable Is
            grpVariable.Text = Strings.EventConditional.variable;
            grpSelectVariable.Text = Strings.EventConditional.selectvariable;
            rdoPlayerVariable.Text = Strings.EventConditional.playervariable;
            rdoGlobalVariable.Text = Strings.EventConditional.globalvariable;

            //Numeric Variable
            grpNumericVariable.Text = Strings.EventConditional.numericvariable;
            lblNumericComparator.Text = Strings.EventConditional.comparator;
            rdoVarCompareStaticValue.Text = Strings.EventConditional.value;
            rdoVarComparePlayerVar.Text = Strings.EventConditional.playervariablevalue;
            rdoVarCompareGlobalVar.Text = Strings.EventConditional.globalvariablevalue;
            cmbNumericComparitor.Items.Clear();
            for (var i = 0; i < Strings.EventConditional.comparators.Count; i++)
            {
                cmbNumericComparitor.Items.Add(Strings.EventConditional.comparators[i]);
            }

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

            //Has Item + Has Free Inventory Slots
            grpInventoryConditions.Text = Strings.EventConditional.hasitem;
            lblItemQuantity.Text = Strings.EventConditional.hasatleast;
            lblItem.Text = Strings.EventConditional.item;
            lblInvVariable.Text = Strings.EventConditional.VariableLabel;
            grpAmountType.Text = Strings.EventConditional.AmountType;
            rdoManual.Text = Strings.EventConditional.Manual;
            rdoVariable.Text = Strings.EventConditional.VariableLabel;
            grpManualAmount.Text = Strings.EventConditional.Manual;
            grpVariableAmount.Text = Strings.EventConditional.VariableLabel;
            rdoInvPlayerVariable.Text = Strings.EventConditional.playervariable;
            rdoInvGlobalVariable.Text = Strings.EventConditional.globalvariable;

            //Has Item Equipped
            grpEquippedItem.Text = Strings.EventConditional.hasitemequipped;
            lblEquippedItem.Text = Strings.EventConditional.item;

            //Class is
            grpClass.Text = Strings.EventConditional.classis;
            lblClass.Text = Strings.EventConditional.Class;

            //Knows Spell
            grpSpell.Text = Strings.EventConditional.knowsspell;
            lblSpell.Text = Strings.EventConditional.spell;

            //Level or Stat is
            grpLevelStat.Text = Strings.EventConditional.levelorstat;
            lblLvlStatValue.Text = Strings.EventConditional.levelstatvalue;
            lblLevelComparator.Text = Strings.EventConditional.comparator;
            lblLevelOrStat.Text = Strings.EventConditional.levelstatitem;
            cmbLevelStat.Items.Clear();
            cmbLevelStat.Items.Add(Strings.EventConditional.level);
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                cmbLevelStat.Items.Add(Strings.Combat.stats[i]);
            }

            cmbLevelComparator.Items.Clear();
            for (var i = 0; i < Strings.EventConditional.comparators.Count; i++)
            {
                cmbLevelComparator.Items.Add(Strings.EventConditional.comparators[i]);
            }

            chkStatIgnoreBuffs.Text = Strings.EventConditional.ignorestatbuffs;

            //Self Switch Is
            grpSelfSwitch.Text = Strings.EventConditional.selfswitchis;
            lblSelfSwitch.Text = Strings.EventConditional.selfswitch;
            lblSelfSwitchIs.Text = Strings.EventConditional.switchis;
            cmbSelfSwitch.Items.Clear();
            for (var i = 0; i < 4; i++)
            {
                cmbSelfSwitch.Items.Add(Strings.EventConditional.selfswitches[i]);
            }

            cmbSelfSwitchVal.Items.Clear();
            cmbSelfSwitchVal.Items.Add(Strings.EventConditional.False);
            cmbSelfSwitchVal.Items.Add(Strings.EventConditional.True);

            //Power Is
            grpPowerIs.Text = Strings.EventConditional.poweris;
            lblPower.Text = Strings.EventConditional.power;
            cmbPower.Items.Clear();
            cmbPower.Items.Add(Strings.EventConditional.power0);
            cmbPower.Items.Add(Strings.EventConditional.power1);

            //Time Is
            grpTime.Text = Strings.EventConditional.time;
            lblStartRange.Text = Strings.EventConditional.startrange;
            lblEndRange.Text = Strings.EventConditional.endrange;
            lblAnd.Text = Strings.EventConditional.and;

            //Can Start Quest
            grpStartQuest.Text = Strings.EventConditional.canstartquest;
            lblStartQuest.Text = Strings.EventConditional.startquest;

            //Quest In Progress
            grpQuestInProgress.Text = Strings.EventConditional.questinprogress;
            lblQuestProgress.Text = Strings.EventConditional.questprogress;
            lblQuestIs.Text = Strings.EventConditional.questis;
            cmbTaskModifier.Items.Clear();
            for (var i = 0; i < Strings.EventConditional.questcomparators.Count; i++)
            {
                cmbTaskModifier.Items.Add(Strings.EventConditional.questcomparators[i]);
            }

            lblQuestTask.Text = Strings.EventConditional.task;

            //Quest Completed
            grpQuestCompleted.Text = Strings.EventConditional.questcompleted;
            lblQuestCompleted.Text = Strings.EventConditional.questcompletedlabel;

            //Gender is
            grpGender.Text = Strings.EventConditional.genderis;
            lblGender.Text = Strings.EventConditional.gender;
            cmbGender.Items.Clear();
            cmbGender.Items.Add(Strings.EventConditional.male);
            cmbGender.Items.Add(Strings.EventConditional.female);

            //Map Is
            grpMapIs.Text = Strings.EventConditional.mapis;
            btnSelectMap.Text = Strings.EventConditional.selectmap;

            //In Guild With At Least Rank
            grpInGuild.Text = Strings.EventConditional.inguild;
            lblRank.Text = Strings.EventConditional.rank;
            cmbRank.Items.Clear();
            foreach (var rank in Options.Instance.Guild.Ranks)
            {
                cmbRank.Items.Add(rank.Title);
            }

            // Map Zone Type
            grpMapZoneType.Text = Strings.EventConditional.MapZoneTypeIs;
            lblMapZoneType.Text = Strings.EventConditional.MapZoneTypeLabel;
            cmbMapZoneType.Items.Clear();
            for (var i = 0; i < Strings.MapProperties.zones.Count; i++)
            {
                cmbMapZoneType.Items.Add(Strings.MapProperties.zones[i]);
            }

            btnSave.Text = Strings.EventConditional.okay;
            btnCancel.Text = Strings.EventConditional.cancel;
        }

        private void ConditionTypeChanged(ConditionTypes type)
        {
            switch (type)
            {
                case ConditionTypes.VariableIs:
                    Condition = new VariableIsCondition();
                    SetupFormValues((dynamic) Condition);

                    break;
                case ConditionTypes.HasItem:
                    Condition = new HasItemCondition();
                    if (cmbItem.Items.Count > 0)
                    {
                        cmbItem.SelectedIndex = 0;
                    }

                    nudItemAmount.Value = 1;

                    break;
                case ConditionTypes.ClassIs:
                    Condition = new ClassIsCondition();
                    if (cmbClass.Items.Count > 0)
                    {
                        cmbClass.SelectedIndex = 0;
                    }

                    break;
                case ConditionTypes.KnowsSpell:
                    Condition = new KnowsSpellCondition();
                    if (cmbSpell.Items.Count > 0)
                    {
                        cmbSpell.SelectedIndex = 0;
                    }

                    break;
                case ConditionTypes.LevelOrStat:
                    Condition = new LevelOrStatCondition();
                    cmbLevelComparator.SelectedIndex = 0;
                    cmbLevelStat.SelectedIndex = 0;
                    nudLevelStatValue.Value = 0;
                    chkStatIgnoreBuffs.Checked = false;

                    break;
                case ConditionTypes.SelfSwitch:
                    Condition = new SelfSwitchCondition();
                    cmbSelfSwitch.SelectedIndex = 0;
                    cmbSelfSwitchVal.SelectedIndex = 0;

                    break;
                case ConditionTypes.AccessIs:
                    Condition = new AccessIsCondition();
                    cmbPower.SelectedIndex = 0;

                    break;
                case ConditionTypes.TimeBetween:
                    Condition = new TimeBetweenCondition();
                    cmbTime1.SelectedIndex = 0;
                    cmbTime2.SelectedIndex = 0;

                    break;
                case ConditionTypes.CanStartQuest:
                    Condition = new CanStartQuestCondition();
                    if (cmbStartQuest.Items.Count > 0)
                    {
                        cmbStartQuest.SelectedIndex = 0;
                    }

                    break;
                case ConditionTypes.QuestInProgress:
                    Condition = new QuestInProgressCondition();
                    if (cmbQuestInProgress.Items.Count > 0)
                    {
                        cmbQuestInProgress.SelectedIndex = 0;
                    }

                    cmbTaskModifier.SelectedIndex = 0;

                    break;
                case ConditionTypes.QuestCompleted:
                    Condition = new QuestCompletedCondition();
                    if (cmbCompletedQuest.Items.Count > 0)
                    {
                        cmbCompletedQuest.SelectedIndex = 0;
                    }

                    break;
                case ConditionTypes.NoNpcsOnMap:
                    Condition = new NoNpcsOnMapCondition();

                    break;
                case ConditionTypes.GenderIs:
                    Condition = new GenderIsCondition();
                    cmbGender.SelectedIndex = 0;

                    break;
                case ConditionTypes.MapIs:
                    Condition = new MapIsCondition();
                    btnSelectMap.Tag = Guid.Empty;

                    break;
                case ConditionTypes.IsItemEquipped:
                    Condition = new IsItemEquippedCondition();
                    if (cmbEquippedItem.Items.Count > 0)
                    {
                        cmbEquippedItem.SelectedIndex = 0;
                    }

                    break;
                case ConditionTypes.HasFreeInventorySlots:
                    Condition = new HasFreeInventorySlots();
                    

                    break;
                case ConditionTypes.InGuildWithRank:
                    Condition = new InGuildWithRank();
                    cmbRank.SelectedIndex = 0;

                    break;
                case ConditionTypes.MapZoneTypeIs:
                    Condition = new MapZoneTypeIs();
                    if (cmbMapZoneType.Items.Count > 0)
                    {
                        cmbMapZoneType.SelectedIndex = 0;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateFormElements(ConditionTypes type)
        {
            grpVariable.Hide();
            grpInventoryConditions.Hide();
            grpSpell.Hide();
            grpClass.Hide();
            grpLevelStat.Hide();
            grpSelfSwitch.Hide();
            grpPowerIs.Hide();
            grpTime.Hide();
            grpStartQuest.Hide();
            grpQuestInProgress.Hide();
            grpQuestCompleted.Hide();
            grpGender.Hide();
            grpMapIs.Hide();
            grpEquippedItem.Hide();
            grpInGuild.Hide();
            grpMapZoneType.Hide();
            switch (type)
            {
                case ConditionTypes.VariableIs:
                    grpVariable.Show();

                    cmbCompareGlobalVar.Items.Clear();
                    cmbCompareGlobalVar.Items.AddRange(ServerVariableBase.Names);
                    cmbComparePlayerVar.Items.Clear();
                    cmbComparePlayerVar.Items.AddRange(PlayerVariableBase.Names);

                    cmbBooleanGlobalVariable.Items.Clear();
                    cmbBooleanGlobalVariable.Items.AddRange(ServerVariableBase.Names);
                    cmbBooleanPlayerVariable.Items.Clear();
                    cmbBooleanPlayerVariable.Items.AddRange(PlayerVariableBase.Names);

                    break;
                case ConditionTypes.HasItem:
                    grpInventoryConditions.Show();
                    grpInventoryConditions.Text = Strings.EventConditional.hasitem;
                    lblItem.Visible = true;
                    cmbItem.Visible = true;
                    cmbItem.Items.Clear();
                    cmbItem.Items.AddRange(ItemBase.Names);
                    SetupAmountInput();

                    break;
                case ConditionTypes.ClassIs:
                    grpClass.Show();
                    cmbClass.Items.Clear();
                    cmbClass.Items.AddRange(ClassBase.Names);

                    break;
                case ConditionTypes.KnowsSpell:
                    grpSpell.Show();
                    cmbSpell.Items.Clear();
                    cmbSpell.Items.AddRange(SpellBase.Names);

                    break;
                case ConditionTypes.LevelOrStat:
                    grpLevelStat.Show();

                    break;
                case ConditionTypes.SelfSwitch:
                    grpSelfSwitch.Show();

                    break;
                case ConditionTypes.AccessIs:
                    grpPowerIs.Show();

                    break;
                case ConditionTypes.TimeBetween:
                    grpTime.Show();
                    cmbTime1.Items.Clear();
                    cmbTime2.Items.Clear();
                    var time = new DateTime(2000, 1, 1, 0, 0, 0);
                    for (var i = 0; i < 1440; i += TimeBase.GetTimeBase().RangeInterval)
                    {
                        var addRange = time.ToString("h:mm:ss tt") + " " + Strings.EventConditional.to + " ";
                        time = time.AddMinutes(TimeBase.GetTimeBase().RangeInterval);
                        addRange += time.ToString("h:mm:ss tt");
                        cmbTime1.Items.Add(addRange);
                        cmbTime2.Items.Add(addRange);
                    }

                    break;
                case ConditionTypes.CanStartQuest:
                    grpStartQuest.Show();
                    cmbStartQuest.Items.Clear();
                    cmbStartQuest.Items.AddRange(QuestBase.Names);

                    break;
                case ConditionTypes.QuestInProgress:
                    grpQuestInProgress.Show();
                    cmbQuestInProgress.Items.Clear();
                    cmbQuestInProgress.Items.AddRange(QuestBase.Names);

                    break;
                case ConditionTypes.QuestCompleted:
                    grpQuestCompleted.Show();
                    cmbCompletedQuest.Items.Clear();
                    cmbCompletedQuest.Items.AddRange(QuestBase.Names);

                    break;
                case ConditionTypes.NoNpcsOnMap:
                    break;
                case ConditionTypes.GenderIs:
                    grpGender.Show();

                    break;
                case ConditionTypes.MapIs:
                    grpMapIs.Show();

                    break;
                case ConditionTypes.IsItemEquipped:
                    grpEquippedItem.Show();
                    cmbEquippedItem.Items.Clear();
                    cmbEquippedItem.Items.AddRange(ItemBase.Names);

                    break;

                case ConditionTypes.HasFreeInventorySlots:
                    grpInventoryConditions.Show();
                    grpInventoryConditions.Text = Strings.EventConditional.FreeInventorySlots;
                    lblItem.Visible = false;
                    cmbItem.Visible = false;
                    cmbItem.Items.Clear();
                    SetupAmountInput();

                    break;
                case ConditionTypes.InGuildWithRank:
                    grpInGuild.Show();

                    break;
                case ConditionTypes.MapZoneTypeIs:
                    grpMapZoneType.Show();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFormValues((dynamic) Condition);
            Condition.Negated = chkNegated.Checked;
            Condition.ElseEnabled = chkHasElse.Checked;

            if (mEventCommand != null)
            {
                mEventCommand.Condition = Condition;
            }

            if (mEventEditor != null)
            {
                mEventEditor.FinishCommandEdit();
            }
            else
            {
                if (ParentForm != null)
                {
                    ParentForm.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mCurrentPage != null)
            {
                mEventEditor.CancelCommandEdit();
            }

            Cancelled = true;
            if (ParentForm != null)
            {
                ParentForm.Close();
            }
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = Strings.EventConditional.conditions.FirstOrDefault(x => x.Value == cmbConditionType.Text).Key;
            if (type < 4)
            {
                type = 0;
            }

            UpdateFormElements((ConditionTypes) type);
            if ((ConditionTypes) type != Condition.Type)
            {
                ConditionTypeChanged((ConditionTypes) type);
            }
        }

        private void cmbTaskModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTaskModifier.SelectedIndex == 0)
            {
                cmbQuestTask.Enabled = false;
            }
            else
            {
                cmbQuestTask.Enabled = true;
            }
        }

        private void cmbQuestInProgress_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbQuestTask.Items.Clear();
            var quest = QuestBase.Get(QuestBase.IdFromList(cmbQuestInProgress.SelectedIndex));
            if (quest != null)
            {
                foreach (var task in quest.Tasks)
                {
                    cmbQuestTask.Items.Add(task.GetTaskString(Strings.TaskEditor.descriptions));
                }

                if (cmbQuestTask.Items.Count > 0)
                {
                    cmbQuestTask.SelectedIndex = 0;
                }
            }
        }

        private void btnSelectMap_Click(object sender, EventArgs e)
        {
            var frmWarpSelection = new FrmWarpSelection();
            frmWarpSelection.InitForm(false, null);
            frmWarpSelection.SelectTile((Guid) btnSelectMap.Tag, 0, 0);
            frmWarpSelection.TopMost = true;
            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                btnSelectMap.Tag = frmWarpSelection.GetMap();
            }
        }

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

        private void UpdateNumericVariableElements()
        {
            nudVariableValue.Enabled = rdoVarCompareStaticValue.Checked;
            cmbComparePlayerVar.Enabled = rdoVarComparePlayerVar.Checked;
            cmbCompareGlobalVar.Enabled = rdoVarCompareGlobalVar.Checked;
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
            }

            //Load the correct editor
            if (varType > 0)
            {
                switch ((VariableDataTypes) varType)
                {
                    case VariableDataTypes.Boolean:
                        grpBooleanVariable.Show();
                        TryLoadVariableBooleanComparison(((VariableIsCondition) Condition).Comparison);

                        break;

                    case VariableDataTypes.Integer:
                        grpNumericVariable.Show();
                        TryLoadVariableIntegerComparison(((VariableIsCondition) Condition).Comparison);
                        UpdateNumericVariableElements();

                        break;

                    case VariableDataTypes.Number:
                        break;

                    case VariableDataTypes.String:
                        grpStringVariable.Show();
                        TryLoadVariableStringComparison(((VariableIsCondition) Condition).Comparison);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void TryLoadVariableBooleanComparison(VariableCompaison comp)
        {
            if (comp.GetType() == typeof(BooleanVariableComparison))
            {
                var com = (BooleanVariableComparison) comp;

                cmbBooleanComparator.SelectedIndex = Convert.ToInt32(!com.ComparingEqual);

                if (cmbBooleanComparator.SelectedIndex < 0)
                {
                    cmbBooleanComparator.SelectedIndex = 0;
                }

                optBooleanTrue.Checked = com.Value;
                optBooleanFalse.Checked = !com.Value;

                if (com.CompareVariableId != Guid.Empty)
                {
                    if (com.CompareVariableType == VariableTypes.PlayerVariable)
                    {
                        optBooleanPlayerVariable.Checked = true;
                        cmbBooleanPlayerVariable.SelectedIndex = PlayerVariableBase.ListIndex(com.CompareVariableId);
                    }
                    else
                    {
                        optBooleanGlobalVariable.Checked = true;
                        cmbBooleanGlobalVariable.SelectedIndex = ServerVariableBase.ListIndex(com.CompareVariableId);
                    }
                }
            }
        }

        private void TryLoadVariableIntegerComparison(VariableCompaison comp)
        {
            if (comp.GetType() == typeof(IntegerVariableComparison))
            {
                var com = (IntegerVariableComparison) comp;

                cmbNumericComparitor.SelectedIndex = (int) com.Comparator;

                if (cmbNumericComparitor.SelectedIndex < 0)
                {
                    cmbNumericComparitor.SelectedIndex = 0;
                }

                if (com.CompareVariableId != Guid.Empty)
                {
                    if (com.CompareVariableType == VariableTypes.PlayerVariable)
                    {
                        rdoVarComparePlayerVar.Checked = true;
                        cmbComparePlayerVar.SelectedIndex = PlayerVariableBase.ListIndex(com.CompareVariableId);
                    }
                    else
                    {
                        rdoVarCompareGlobalVar.Checked = true;
                        cmbCompareGlobalVar.SelectedIndex = ServerVariableBase.ListIndex(com.CompareVariableId);
                    }
                }
                else
                {
                    rdoVarCompareStaticValue.Checked = true;
                    nudVariableValue.Value = com.Value;
                }

                UpdateNumericVariableElements();
            }
        }

        private void TryLoadVariableStringComparison(VariableCompaison comp)
        {
            if (comp.GetType() == typeof(StringVariableComparison))
            {
                var com = (StringVariableComparison) comp;

                cmbStringComparitor.SelectedIndex = Convert.ToInt32(com.Comparator);

                if (cmbStringComparitor.SelectedIndex < 0)
                {
                    cmbStringComparitor.SelectedIndex = 0;
                }

                txtStringValue.Text = com.Value;
            }
        }

        private void InitVariableElements(Guid variableId)
        {
            mLoading = true;
            cmbVariable.Items.Clear();
            if (rdoPlayerVariable.Checked)
            {
                cmbVariable.Items.AddRange(PlayerVariableBase.Names);
                cmbVariable.SelectedIndex = PlayerVariableBase.ListIndex(variableId);
            }
            else
            {
                cmbVariable.Items.AddRange(ServerVariableBase.Names);
                cmbVariable.SelectedIndex = ServerVariableBase.ListIndex(variableId);
            }

            mLoading = false;
        }

        private BooleanVariableComparison GetBooleanVariableComparison()
        {
            var comp = new BooleanVariableComparison();

            if (cmbBooleanComparator.SelectedIndex < 0)
            {
                cmbBooleanComparator.SelectedIndex = 0;
            }

            comp.ComparingEqual = !Convert.ToBoolean(cmbBooleanComparator.SelectedIndex);

            comp.Value = optBooleanTrue.Checked;

            if (optBooleanGlobalVariable.Checked)
            {
                comp.CompareVariableType = VariableTypes.ServerVariable;
                comp.CompareVariableId = ServerVariableBase.IdFromList(cmbBooleanGlobalVariable.SelectedIndex);
            }
            else if (optBooleanPlayerVariable.Checked)
            {
                comp.CompareVariableType = VariableTypes.PlayerVariable;
                comp.CompareVariableId = PlayerVariableBase.IdFromList(cmbBooleanPlayerVariable.SelectedIndex);
            }

            return comp;
        }

        private IntegerVariableComparison GetNumericVariableComparison()
        {
            var comp = new IntegerVariableComparison();

            if (cmbNumericComparitor.SelectedIndex < 0)
            {
                cmbNumericComparitor.SelectedIndex = 0;
            }

            comp.Comparator = (VariableComparators) cmbNumericComparitor.SelectedIndex;

            comp.CompareVariableId = Guid.Empty;

            if (rdoVarCompareStaticValue.Checked)
            {
                comp.Value = (long) nudVariableValue.Value;
            }
            else if (rdoVarCompareGlobalVar.Checked)
            {
                comp.CompareVariableType = VariableTypes.ServerVariable;
                comp.CompareVariableId = ServerVariableBase.IdFromList(cmbCompareGlobalVar.SelectedIndex);
            }
            else if (rdoVarComparePlayerVar.Checked)
            {
                comp.CompareVariableType = VariableTypes.PlayerVariable;
                comp.CompareVariableId = PlayerVariableBase.IdFromList(cmbComparePlayerVar.SelectedIndex);
            }

            return comp;
        }

        private StringVariableComparison GetStringVariableComparison()
        {
            var comp = new StringVariableComparison();

            if (cmbStringComparitor.SelectedIndex < 0)
            {
                cmbStringComparitor.SelectedIndex = 0;
            }

            comp.Comparator = (StringVariableComparators) cmbStringComparitor.SelectedIndex;

            comp.Value = txtStringValue.Text;

            return comp;
        }

        private void rdoPlayerVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitVariableElements(Guid.Empty);
            if (!mLoading && cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }
        }

        private void rdoGlobalVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitVariableElements(Guid.Empty);
            if (!mLoading && cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }
        }

        private void cmbVariable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mLoading)
            {
                return;
            }

            if (rdoPlayerVariable.Checked)
            {
                InitVariableElements(PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex));
            }
            else if (rdoGlobalVariable.Checked)
            {
                InitVariableElements(ServerVariableBase.IdFromList(cmbVariable.SelectedIndex));
            }

            UpdateVariableElements();
        }

        private void lblStringTextVariables_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                "http://www.ascensiongamedev.com/community/topic/749-event-text-variables/"
            );
        }

        private void NudItemAmount_ValueChanged(object sender, System.EventArgs e)
        {
            nudItemAmount.Value = Math.Max(1, nudItemAmount.Value);
        }

        private void rdoManual_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
        }

        private void rdoVariable_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
        }

        private void rdoInvPlayerVariable_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
        }

        private void rdoInvGlobalVariable_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
        }

        private void SetupAmountInput()
        {
            grpManualAmount.Visible = rdoManual.Checked;
            grpVariableAmount.Visible = !rdoManual.Checked;

            VariableTypes conditionVariableType;
            Guid conditionVariableId;
            int ConditionQuantity;

            switch (Condition.Type)
            {
                case ConditionTypes.HasFreeInventorySlots:
                    conditionVariableType = ((HasFreeInventorySlots)Condition).VariableType;
                    conditionVariableId = ((HasFreeInventorySlots)Condition).VariableId;
                    ConditionQuantity = ((HasFreeInventorySlots)Condition).Quantity;
                    break;
                case ConditionTypes.HasItem:
                    conditionVariableType = ((HasItemCondition)Condition).VariableType;
                    conditionVariableId = ((HasItemCondition)Condition).VariableId;
                    ConditionQuantity = ((HasItemCondition)Condition).Quantity;
                    break;
                default:
                    conditionVariableType = VariableTypes.PlayerVariable;
                    conditionVariableId = Guid.Empty;
                    ConditionQuantity = 0;
                    return;
            }

            cmbInvVariable.Items.Clear();
            if (rdoInvPlayerVariable.Checked)
            {
                cmbInvVariable.Items.AddRange(PlayerVariableBase.GetNamesByType(VariableDataTypes.Integer));
                // Do not update if the wrong type of variable is saved
                if (conditionVariableType == VariableTypes.PlayerVariable)
                {
                    var index = PlayerVariableBase.ListIndex(conditionVariableId, VariableDataTypes.Integer);
                    if (index > -1)
                    {
                        cmbInvVariable.SelectedIndex = index;
                    }
                    else
                    {
                        VariableBlank();
                    }
                }
                else
                {
                    VariableBlank();
                }
            }
            else
            {
                cmbInvVariable.Items.AddRange(ServerVariableBase.GetNamesByType(VariableDataTypes.Integer));
                // Do not update if the wrong type of variable is saved
                if (conditionVariableType == VariableTypes.ServerVariable)
                {
                    var index = ServerVariableBase.ListIndex(conditionVariableId, VariableDataTypes.Integer);
                    if (index > -1)
                    {
                        cmbInvVariable.SelectedIndex = index;
                    }
                    else
                    {
                        VariableBlank();
                    }
                }
                else
                {
                    VariableBlank();
                }
            }

            nudItemAmount.Value = Math.Max(1, ConditionQuantity);
        }

        private void VariableBlank()
        {
            if (cmbInvVariable.Items.Count > 0)
            {
                cmbInvVariable.SelectedIndex = 0;
            }
            else
            {
                cmbInvVariable.SelectedIndex = -1;
                cmbInvVariable.Text = "";
            }
        }

        #region "SetupFormValues"

        private void SetupFormValues(VariableIsCondition condition)
        {
            if (condition.VariableType == VariableTypes.PlayerVariable)
            {
                rdoPlayerVariable.Checked = true;
            }
            else
            {
                rdoGlobalVariable.Checked = true;
            }

            InitVariableElements(condition.VariableId);

            UpdateVariableElements();
        }

        private void SetupFormValues(HasItemCondition condition)
        {
            cmbItem.SelectedIndex = ItemBase.ListIndex(condition.ItemId);
            nudItemAmount.Value = condition.Quantity;
            rdoVariable.Checked = condition.UseVariable;
            rdoInvGlobalVariable.Checked = condition.VariableType == VariableTypes.ServerVariable;
            SetupAmountInput();
        }

        private void SetupFormValues(ClassIsCondition condition)
        {
            cmbClass.SelectedIndex = ClassBase.ListIndex(condition.ClassId);
        }

        private void SetupFormValues(KnowsSpellCondition condition)
        {
            cmbSpell.SelectedIndex = SpellBase.ListIndex(condition.SpellId);
        }

        private void SetupFormValues(LevelOrStatCondition condition)
        {
            cmbLevelComparator.SelectedIndex = (int) condition.Comparator;
            nudLevelStatValue.Value = condition.Value;
            cmbLevelStat.SelectedIndex = condition.ComparingLevel ? 0 : (int) condition.Stat + 1;
            chkStatIgnoreBuffs.Checked = condition.IgnoreBuffs;
        }

        private void SetupFormValues(SelfSwitchCondition condition)
        {
            cmbSelfSwitch.SelectedIndex = condition.SwitchIndex;
            cmbSelfSwitchVal.SelectedIndex = Convert.ToInt32(condition.Value);
        }

        private void SetupFormValues(AccessIsCondition condition)
        {
            cmbPower.SelectedIndex = (int) condition.Access;
        }

        private void SetupFormValues(TimeBetweenCondition condition)
        {
            cmbTime1.SelectedIndex = Math.Min(condition.Ranges[0], cmbTime1.Items.Count - 1);
            cmbTime2.SelectedIndex = Math.Min(condition.Ranges[1], cmbTime2.Items.Count - 1);
        }

        private void SetupFormValues(CanStartQuestCondition condition)
        {
            cmbStartQuest.SelectedIndex = QuestBase.ListIndex(condition.QuestId);
        }

        private void SetupFormValues(QuestInProgressCondition condition)
        {
            cmbQuestInProgress.SelectedIndex = QuestBase.ListIndex(condition.QuestId);
            cmbTaskModifier.SelectedIndex = (int) condition.Progress;
            if (cmbTaskModifier.SelectedIndex == -1)
            {
                cmbTaskModifier.SelectedIndex = 0;
            }

            if (cmbTaskModifier.SelectedIndex != 0)
            {
                //Get Quest Task Here
                var quest = QuestBase.Get(QuestBase.IdFromList(cmbQuestInProgress.SelectedIndex));
                if (quest != null)
                {
                    for (var i = 0; i < quest.Tasks.Count; i++)
                    {
                        if (quest.Tasks[i].Id == condition.TaskId)
                        {
                            cmbQuestTask.SelectedIndex = i;
                        }
                    }
                }
            }
        }

        private void SetupFormValues(NoNpcsOnMapCondition condition)
        {
            //Nothing to do but we need this here so the dynamic will work :) 
        }

        private void SetupFormValues(QuestCompletedCondition condition)
        {
            cmbCompletedQuest.SelectedIndex = QuestBase.ListIndex(condition.QuestId);
        }

        private void SetupFormValues(GenderIsCondition condition)
        {
            cmbGender.SelectedIndex = (int) condition.Gender;
        }

        private void SetupFormValues(MapIsCondition condition)
        {
            btnSelectMap.Tag = condition.MapId;
        }

        private void SetupFormValues(IsItemEquippedCondition condition)
        {
            cmbEquippedItem.SelectedIndex = ItemBase.ListIndex(condition.ItemId);
        }

        private void SetupFormValues(HasFreeInventorySlots condition)
        {
            nudItemAmount.Value = condition.Quantity;
            rdoVariable.Checked = condition.UseVariable;
            rdoInvGlobalVariable.Checked = condition.VariableType == VariableTypes.ServerVariable;
            SetupAmountInput();
        }

        private void SetupFormValues(InGuildWithRank condition)
        {
            cmbRank.SelectedIndex = Math.Max(0, Math.Min(Options.Instance.Guild.Ranks.Length - 1, condition.Rank));
        }

        private void SetupFormValues(MapZoneTypeIs condition)
        {
            if (cmbMapZoneType.Items.Count > 0)
            {
                cmbMapZoneType.SelectedIndex = (int)condition.ZoneType;
            }
        }


        #endregion

        #region "SaveFormValues"

        private void SaveFormValues(VariableIsCondition condition)
        {
            if (rdoGlobalVariable.Checked)
            {
                condition.VariableType = VariableTypes.ServerVariable;
                condition.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }
            else
            {
                condition.VariableType = VariableTypes.PlayerVariable;
                condition.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex);
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
                condition.Comparison = new VariableCompaison();
            }
        }

        private void SaveFormValues(HasItemCondition condition)
        {
            condition.ItemId = ItemBase.IdFromList(cmbItem.SelectedIndex);
            condition.Quantity = (int) nudItemAmount.Value;
            condition.VariableType = rdoInvPlayerVariable.Checked ? VariableTypes.PlayerVariable : VariableTypes.ServerVariable;
            condition.UseVariable = !rdoManual.Checked;
            condition.VariableId = rdoInvPlayerVariable.Checked ? PlayerVariableBase.IdFromList(cmbInvVariable.SelectedIndex) : ServerVariableBase.IdFromList(cmbInvVariable.SelectedIndex);
        }

        private void SaveFormValues(ClassIsCondition condition)
        {
            condition.ClassId = ClassBase.IdFromList(cmbClass.SelectedIndex);
        }

        private void SaveFormValues(KnowsSpellCondition condition)
        {
            condition.SpellId = SpellBase.IdFromList(cmbSpell.SelectedIndex);
        }

        private void SaveFormValues(LevelOrStatCondition condition)
        {
            condition.Comparator = (VariableComparators) cmbLevelComparator.SelectedIndex;
            condition.Value = (int) nudLevelStatValue.Value;
            condition.ComparingLevel = cmbLevelStat.SelectedIndex == 0;
            if (!condition.ComparingLevel)
            {
                condition.Stat = (Stats) (cmbLevelStat.SelectedIndex - 1);
            }

            condition.IgnoreBuffs = chkStatIgnoreBuffs.Checked;
        }

        private void SaveFormValues(SelfSwitchCondition condition)
        {
            condition.SwitchIndex = cmbSelfSwitch.SelectedIndex;
            condition.Value = Convert.ToBoolean(cmbSelfSwitchVal.SelectedIndex);
        }

        private void SaveFormValues(AccessIsCondition condition)
        {
            condition.Access = (Access) cmbPower.SelectedIndex;
        }

        private void SaveFormValues(TimeBetweenCondition condition)
        {
            condition.Ranges[0] = cmbTime1.SelectedIndex;
            condition.Ranges[1] = cmbTime2.SelectedIndex;
        }

        private void SaveFormValues(CanStartQuestCondition condition)
        {
            condition.QuestId = QuestBase.IdFromList(cmbStartQuest.SelectedIndex);
        }

        private void SaveFormValues(QuestInProgressCondition condition)
        {
            condition.QuestId = QuestBase.IdFromList(cmbQuestInProgress.SelectedIndex);
            condition.Progress = (QuestProgressState) cmbTaskModifier.SelectedIndex;
            condition.TaskId = Guid.Empty;
            if (cmbTaskModifier.SelectedIndex != 0)
            {
                //Get Quest Task Here
                var quest = QuestBase.Get(QuestBase.IdFromList(cmbQuestInProgress.SelectedIndex));
                if (quest != null)
                {
                    if (cmbQuestTask.SelectedIndex > -1)
                    {
                        condition.TaskId = quest.Tasks[cmbQuestTask.SelectedIndex].Id;
                    }
                }
            }
        }

        private void SaveFormValues(QuestCompletedCondition condition)
        {
            condition.QuestId = QuestBase.IdFromList(cmbCompletedQuest.SelectedIndex);
        }

        private void SaveFormValues(NoNpcsOnMapCondition condition)
        {
            //Nothing to do but we need this here so the dynamic will work :) 
        }

        private void SaveFormValues(GenderIsCondition condition)
        {
            condition.Gender = (Gender) cmbGender.SelectedIndex;
        }

        private void SaveFormValues(MapIsCondition condition)
        {
            condition.MapId = (Guid) btnSelectMap.Tag;
        }

        private void SaveFormValues(IsItemEquippedCondition condition)
        {
            condition.ItemId = ItemBase.IdFromList(cmbEquippedItem.SelectedIndex);
        }

        private void SaveFormValues(HasFreeInventorySlots condition)
        {
            condition.Quantity = (int) nudItemAmount.Value;
            condition.VariableType = rdoInvPlayerVariable.Checked ? VariableTypes.PlayerVariable : VariableTypes.ServerVariable;
            condition.UseVariable = !rdoManual.Checked;
            condition.VariableId = rdoInvPlayerVariable.Checked ? PlayerVariableBase.IdFromList(cmbInvVariable.SelectedIndex) : ServerVariableBase.IdFromList(cmbInvVariable.SelectedIndex);
        }

        private void SaveFormValues(InGuildWithRank condition)
        {
            condition.Rank = Math.Max(cmbRank.SelectedIndex, 0);
        }

        private void SaveFormValues(MapZoneTypeIs condition)
        {
            if (cmbMapZoneType.Items.Count > 0)
            {
                condition.ZoneType = (MapZones)cmbMapZoneType.SelectedIndex;
            }
        }

        #endregion
    }

}
