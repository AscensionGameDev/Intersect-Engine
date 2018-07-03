using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandConditionalBranch : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventPage mCurrentPage;
        private Condition mMyCondtion;
        public bool Cancelled;

        public EventCommandConditionalBranch(Condition refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCondtion = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();
            cmbConditionType.SelectedIndex = (int)mMyCondtion.Type;
            UpdateFormElements();
            SetupFormValues((dynamic) refCommand);
        }

        private void InitLocalization()
        {
            grpConditional.Text = Strings.EventConditional.title;
            lblType.Text = Strings.EventConditional.type;

            cmbConditionType.Items.Clear();
            for (int i = 0; i < Strings.EventConditional.conditions.Length; i++)
            {
                cmbConditionType.Items.Add(Strings.EventConditional.conditions[i]);
            }

            //Player Switch
            grpSwitch.Text = Strings.EventConditional.playerswitch;
            lblSwitch.Text = Strings.EventConditional.Switch;
            lblSwitchIs.Text = Strings.EventConditional.switchis;
            cmbSwitchVal.Items.Clear();
            cmbSwitchVal.Items.Add(Strings.EventConditional.False);
            cmbSwitchVal.Items.Add(Strings.EventConditional.True);

            //Player Variable
            grpPlayerVariable.Text = Strings.EventConditional.playervariable;
            lblVariable.Text = Strings.EventConditional.variable;
            lblComparator.Text = Strings.EventConditional.comparator;
            lblVariableValue.Text = Strings.EventConditional.value;
            cmbVariableMod.Items.Clear();
            for (int i = 0; i < Strings.EventConditional.comparators.Length; i++)
            {
                cmbVariableMod.Items.Add(Strings.EventConditional.comparators[i]);
            }

            //Has Item
            grpHasItem.Text = Strings.EventConditional.hasitem;
            lblItemQuantity.Text = Strings.EventConditional.hasatleast;
            lblItem.Text = Strings.EventConditional.item;

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
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                cmbLevelStat.Items.Add(Strings.Combat.stats[i]);
            }
            cmbLevelComparator.Items.Clear();
            for (int i = 0; i < Strings.EventConditional.comparators.Length; i++)
            {
                cmbLevelComparator.Items.Add(Strings.EventConditional.comparators[i]);
            }

            //Self Switch Is
            grpSelfSwitch.Text = Strings.EventConditional.selfswitchis;
            lblSelfSwitch.Text = Strings.EventConditional.selfswitch;
            lblSelfSwitchIs.Text = Strings.EventConditional.switchis;
            cmbSelfSwitch.Items.Clear();
            for (int i = 0; i < 4; i++)
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
            for (int i = 0; i < Strings.EventConditional.questcomparators.Length; i++)
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

            btnSave.Text = Strings.EventConditional.okay;
            btnCancel.Text = Strings.EventConditional.cancel;
        }

        private void UpdateFormElements()
        {
            grpSwitch.Hide();
            grpPlayerVariable.Hide();
            grpHasItem.Hide();
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
            if (cmbConditionType.SelectedIndex != (int) mMyCondtion.Type)
            {
                switch ((ConditionTypes) cmbConditionType.SelectedIndex)
                {
                    case ConditionTypes.PlayerSwitch:
                        mMyCondtion = new PlayerSwitchCondition();
                        grpSwitch.Text = Strings.EventConditional.playerswitch;
                        grpSwitch.Show();
                        cmbSwitch.Items.Clear();
                        cmbSwitch.Items.AddRange(PlayerSwitchBase.Names);
                        if (cmbSwitch.Items.Count > 0) cmbSwitch.SelectedIndex = 0;
                        cmbSwitchVal.SelectedIndex = 0;
                        break;
                    case ConditionTypes.PlayerVariable:
                        mMyCondtion = new PlayerVariableCondition();
                        grpPlayerVariable.Text = Strings.EventConditional.playervariable;
                        grpPlayerVariable.Show();
                        cmbVariable.Items.Clear();
                        cmbVariable.Items.AddRange(PlayerVariableBase.Names);
                        if (cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
                        cmbVariableMod.SelectedIndex = 0;
                        txtVariableVal.Text = @"0";
                        break;
                    case ConditionTypes.ServerSwitch:
                        mMyCondtion = new ServerSwitchCondition();
                        grpPlayerVariable.Text = Strings.EventConditional.globalswitch;
                        grpSwitch.Show();
                        cmbSwitch.Items.Clear();
                        cmbSwitch.Items.AddRange(ServerSwitchBase.Names);
                        if (cmbSwitch.Items.Count > 0) cmbSwitch.SelectedIndex = 0;
                        cmbSwitchVal.SelectedIndex = 0;
                        break;
                    case ConditionTypes.ServerVariable:
                        mMyCondtion = new ServerVariableCondition();
                        grpPlayerVariable.Text = Strings.EventConditional.globalvariable;
                        grpPlayerVariable.Show();
                        cmbVariable.Items.Clear();
                        cmbVariable.Items.AddRange(ServerVariableBase.Names);
                        if (cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
                        cmbVariableMod.SelectedIndex = 0;
                        txtVariableVal.Text = @"0";
                        break;
                    case ConditionTypes.HasItem:
                        mMyCondtion = new HasItemCondition();
                        grpHasItem.Show();
                        cmbItem.Items.Clear();
                        cmbItem.Items.AddRange(ItemBase.Names);
                        if (cmbItem.Items.Count > 0) cmbItem.SelectedIndex = 0;
                        nudItemAmount.Value = 1;
                        break;
                    case ConditionTypes.ClassIs:
                        mMyCondtion = new ClassIsCondition();
                        grpClass.Show();
                        cmbClass.Items.Clear();
                        cmbClass.Items.AddRange(ClassBase.Names);
                        if (cmbClass.Items.Count > 0) cmbClass.SelectedIndex = 0;
                        break;
                    case ConditionTypes.KnowsSpell:
                        mMyCondtion = new KnowsSpellCondition();
                        grpSpell.Show();
                        cmbSpell.Items.Clear();
                        cmbSpell.Items.AddRange(SpellBase.Names);
                        if (cmbSpell.Items.Count > 0) cmbSpell.SelectedIndex = 0;
                        break;
                    case ConditionTypes.LevelOrStat:
                        mMyCondtion = new LevelOrStatCondition();
                        grpLevelStat.Show();
                        cmbLevelComparator.SelectedIndex = 0;
                        cmbLevelStat.SelectedIndex = 0;
                        nudLevelStatValue.Value = 0;
                        break;
                    case ConditionTypes.SelfSwitch:
                        mMyCondtion = new SelfSwitchCondition();
                        grpSelfSwitch.Show();
                        cmbSelfSwitch.SelectedIndex = 0;
                        cmbSelfSwitchVal.SelectedIndex = 0;
                        break;
                    case ConditionTypes.PowerIs:
                        mMyCondtion = new PowerIsCondition();
                        grpPowerIs.Show();
                        cmbPower.SelectedIndex = 0;
                        break;
                    case ConditionTypes.TimeBetween:
                        mMyCondtion = new TimeBetweenCondition();
                        grpTime.Show();
                        cmbTime1.Items.Clear();
                        cmbTime2.Items.Clear();
                        var time = new DateTime(2000, 1, 1, 0, 0, 0);
                        for (int i = 0; i < 1440; i += TimeBase.GetTimeBase().RangeInterval)
                        {
                            var addRange = time.ToString("h:mm:ss tt") + " " + Strings.EventConditional.to + " ";
                            time = time.AddMinutes(TimeBase.GetTimeBase().RangeInterval);
                            addRange += time.ToString("h:mm:ss tt");
                            cmbTime1.Items.Add(addRange);
                            cmbTime2.Items.Add(addRange);
                        }
                        cmbTime1.SelectedIndex = 0;
                        cmbTime2.SelectedIndex = 0;
                        break;
                    case ConditionTypes.CanStartQuest:
                        mMyCondtion = new CanStartQuestCondition();
                        grpStartQuest.Show();
                        cmbStartQuest.Items.Clear();
                        cmbStartQuest.Items.AddRange(QuestBase.Names);
                        if (cmbStartQuest.Items.Count > 0) cmbStartQuest.SelectedIndex = 0;
                        break;
                    case ConditionTypes.QuestInProgress:
                        mMyCondtion = new QuestInProgressCondition();
                        grpQuestInProgress.Show();
                        cmbQuestInProgress.Items.Clear();
                        cmbQuestInProgress.Items.AddRange(QuestBase.Names);
                        if (cmbQuestInProgress.Items.Count > 0) cmbQuestInProgress.SelectedIndex = 0;
                        cmbTaskModifier.SelectedIndex = 0;
                        break;
                    case ConditionTypes.QuestCompleted:
                        mMyCondtion = new QuestCompletedCondition();
                        grpQuestCompleted.Show();
                        cmbCompletedQuest.Items.Clear();
                        cmbCompletedQuest.Items.AddRange(QuestBase.Names);
                        if (cmbCompletedQuest.Items.Count > 0) cmbCompletedQuest.SelectedIndex = 0;
                        break;
                    case ConditionTypes.NoNpcsOnMap:
                        mMyCondtion = new NoNpcsOnMapCondition();
                        grpGender.Show();
                        cmbGender.SelectedIndex = 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFormValues((dynamic) mMyCondtion);
            
            if (mEventEditor != null)
            {
                mEventEditor.FinishCommandEdit();
            }
            else
            {
                if (ParentForm != null) ParentForm.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mCurrentPage != null)
            {
                mEventEditor.CancelCommandEdit();
            }
            Cancelled = true;
            if (ParentForm != null) ParentForm.Close();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
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
            var quest =
                QuestBase.Get(
                    QuestBase.IdFromList(cmbQuestInProgress.SelectedIndex));
            if (quest != null)
            {
                foreach (var task in quest.Tasks)
                {
                    cmbQuestTask.Items.Add(task.GetTaskString(Strings.TaskEditor.descriptions));
                }
                if (cmbQuestTask.Items.Count > 0) cmbQuestTask.SelectedIndex = 0;
            }
        }


        #region "SetupFormValues"
        private void SetupFormValues(PlayerSwitchCondition condition)
        {
            cmbSwitch.SelectedIndex = PlayerSwitchBase.ListIndex(condition.SwitchId);
            cmbSwitchVal.SelectedIndex = Convert.ToInt32(condition.Value);
        }

        private void SetupFormValues(PlayerVariableCondition condition)
        {
            cmbVariable.SelectedIndex = PlayerVariableBase.ListIndex(condition.VariableId);
            cmbVariableMod.SelectedIndex = (int)condition.Comparator;
            txtVariableVal.Text = condition.Value.ToString();
        }

        private void SetupFormValues(ServerSwitchCondition condition)
        {
            cmbSwitch.SelectedIndex = ServerSwitchBase.ListIndex(condition.SwitchId);
            cmbSwitchVal.SelectedIndex = Convert.ToInt32(condition.Value);
        }

        private void SetupFormValues(ServerVariableCondition condition)
        {
            cmbVariable.SelectedIndex = ServerVariableBase.ListIndex(condition.VariableId);
            cmbVariableMod.SelectedIndex = (int)condition.Comparator;
            txtVariableVal.Text = condition.Value.ToString();
        }

        private void SetupFormValues(HasItemCondition condition)
        {
            cmbItem.SelectedIndex = ItemBase.ListIndex(condition.ItemId);
            nudItemAmount.Value = condition.Quantity;
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
            cmbLevelComparator.SelectedIndex = (int)condition.Comparator;
            nudLevelStatValue.Value = condition.Value;
            cmbLevelStat.SelectedIndex = condition.ComparingLevel ? 0 : (int)condition.Stat + 1;
        }

        private void SetupFormValues(SelfSwitchCondition condition)
        {
            cmbSelfSwitch.SelectedIndex = condition.SwitchIndex;
            cmbSelfSwitchVal.SelectedIndex = Convert.ToInt32(condition.Value);
        }

        private void SetupFormValues(PowerIsCondition condition)
        {
            cmbPower.SelectedIndex = condition.Power;
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
            cmbTaskModifier.SelectedIndex = (int)condition.Progress;
            if (cmbTaskModifier.SelectedIndex == -1) cmbTaskModifier.SelectedIndex = 0;
            if (cmbTaskModifier.SelectedIndex != 0)
            {
                //Get Quest Task Here
                var quest = QuestBase.Get(QuestBase.IdFromList(cmbQuestInProgress.SelectedIndex));
                if (quest != null)
                {
                    for (int i = 0; i < quest.Tasks.Count; i++)
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
            cmbGender.SelectedIndex = condition.Gender;
        }
        #endregion

        #region "SaveFormValues"
        private void SaveFormValues(PlayerSwitchCondition condition)
        {
            condition.SwitchId = PlayerSwitchBase.IdFromList(cmbSwitch.SelectedIndex);
            condition.Value = Convert.ToBoolean(cmbSwitchVal.SelectedIndex);
        }

        private void SaveFormValues(PlayerVariableCondition condition)
        {
            condition.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            condition.Comparator = (VariableComparators)cmbVariableMod.SelectedIndex;
            int.TryParse(txtVariableVal.Text, out int n);
            condition.Value = n;
        }

        private void SaveFormValues(ServerSwitchCondition condition)
        {
            condition.SwitchId = ServerSwitchBase.IdFromList(cmbSwitch.SelectedIndex);
            condition.Value = Convert.ToBoolean(cmbSwitchVal.SelectedIndex);
        }

        private void SaveFormValues(ServerVariableCondition condition)
        {
            condition.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            condition.Comparator = (VariableComparators)cmbVariableMod.SelectedIndex;
            int.TryParse(txtVariableVal.Text, out int n);
            condition.Value = n;
        }

        private void SaveFormValues(HasItemCondition condition)
        {
            condition.ItemId = ItemBase.IdFromList(cmbItem.SelectedIndex);
            condition.Quantity = (int)nudItemAmount.Value;
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
            condition.Comparator = (VariableComparators)cmbLevelComparator.SelectedIndex;
            condition.Value = (int)nudLevelStatValue.Value;
            condition.ComparingLevel = cmbLevelStat.SelectedIndex == 0;
            if (!condition.ComparingLevel)
            {
                condition.Stat = (Stats) (cmbLevelStat.SelectedIndex - 1);
            }
        }

        private void SaveFormValues(SelfSwitchCondition condition)
        {
            condition.SwitchIndex = cmbSelfSwitch.SelectedIndex;
            condition.Value = Convert.ToBoolean(cmbSelfSwitchVal.SelectedIndex);
        }

        private void SaveFormValues(PowerIsCondition condition)
        {
            condition.Power = (byte)cmbPower.SelectedIndex;
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
            condition.Progress = (QuestProgress)cmbTaskModifier.SelectedIndex;
            condition.TaskId = Guid.Empty;
            if (cmbTaskModifier.SelectedIndex != 0)
            {
                //Get Quest Task Here
                var quest =  QuestBase.Get(QuestBase.IdFromList(cmbQuestInProgress.SelectedIndex));
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
            condition.Gender = (byte)cmbGender.SelectedIndex;
        }
        #endregion
    }
}