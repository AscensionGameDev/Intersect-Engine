using Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;
using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;

public partial class EventCommandConditionalBranch : UserControl
{
    public bool Cancelled;
    public bool Loading = false;
    public Condition Condition;

    private readonly FrmEvent _eventEditor;
    private readonly ConditionalBranchCommand _eventCommand;
    private readonly EventPage _currentPage;

    private readonly ConditionControl_EquippedItem _equippedItemControl;
    private readonly ConditionControl_EquippedSlot _equippedSlotControl;
    private readonly ConditionControl_HasItemOrSlots _itemControl;
    private readonly ConditionControl_Map _mapControl;
    private readonly ConditionControl_MapZone _mapZoneControl;
    private readonly ConditionControl_NoNpcOnMap _npcControl;
    private readonly ConditionControl_PlayerClass _playerClassControl;
    private readonly ConditionControl_PlayerGender _playerGenderControl;
    private readonly ConditionControl_PlayerGuild _playerGuildControl;
    private readonly ConditionControl_PlayerPower _playerPowerControl;
    private readonly ConditionControl_PlayerSpell _playerSpellControl;
    private readonly ConditionControl_PlayerStat _playerStatControl;
    private readonly ConditionControl_QuestCanStart _questCanStartControl;
    private readonly ConditionControl_QuestCompleted _questCompletedControl;
    private readonly ConditionControl_QuestInProgress _questInProgressControl;
    private readonly ConditionControl_SelfSwitch _selfSwitchControl;
    private readonly ConditionControl_TimeBetween _timeBetweenControl;
    private readonly ConditionControl_Variable _variableControl;

    public EventCommandConditionalBranch(
        Condition refCommand,
        EventPage refPage,
        FrmEvent editor,
        ConditionalBranchCommand command
    )
    {
        InitializeComponent();
        Loading = true;
        refCommand ??= new VariableIsCondition();

        Condition = refCommand;
        _eventEditor = editor;
        _eventCommand = command;
        _currentPage = refPage;

        _equippedItemControl = new();
        _equippedSlotControl = new();
        _itemControl = new(this);
        _mapControl = new();
        _mapZoneControl = new();
        _npcControl = new();
        _playerClassControl = new();
        _playerGenderControl = new();
        _playerGuildControl = new();
        _playerPowerControl = new();
        _playerSpellControl = new();
        _playerStatControl = new();
        _questCanStartControl = new();
        _questCompletedControl = new();
        _questInProgressControl = new();
        _selfSwitchControl = new();
        _timeBetweenControl = new();
        _variableControl = new(this);

        pnlConditionControl.Controls.Add(_equippedItemControl);
        pnlConditionControl.Controls.Add(_equippedSlotControl);
        pnlConditionControl.Controls.Add(_itemControl);
        pnlConditionControl.Controls.Add(_mapControl);
        pnlConditionControl.Controls.Add(_mapZoneControl);
        pnlConditionControl.Controls.Add(_npcControl);
        pnlConditionControl.Controls.Add(_playerClassControl);
        pnlConditionControl.Controls.Add(_playerGenderControl);
        pnlConditionControl.Controls.Add(_playerGuildControl);
        pnlConditionControl.Controls.Add(_playerPowerControl);
        pnlConditionControl.Controls.Add(_playerSpellControl);
        pnlConditionControl.Controls.Add(_playerStatControl);
        pnlConditionControl.Controls.Add(_questCanStartControl);
        pnlConditionControl.Controls.Add(_questCompletedControl);
        pnlConditionControl.Controls.Add(_questInProgressControl);
        pnlConditionControl.Controls.Add(_selfSwitchControl);
        pnlConditionControl.Controls.Add(_timeBetweenControl);
        pnlConditionControl.Controls.Add(_variableControl);

        InitLocalization();
        UpdateFormElements(refCommand.Type);

        var typeIndex = 0;
        foreach (var item in Strings.EventConditional.conditions)
        {
            if (item.Key == (int)Condition.Type)
            {
                cmbConditionType.SelectedIndex = typeIndex;
                break;
            }

            typeIndex++;
        }

        chkNegated.Checked = refCommand.Negated;
        chkHasElse.Checked = refCommand.ElseEnabled;
        SetupFormValues(refCommand);
        Loading = false;
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

        btnSave.Text = Strings.EventConditional.okay;
        btnCancel.Text = Strings.EventConditional.cancel;
    }

    private void UpdateFormElements(ConditionType type)
    {
        _equippedItemControl.Hide();
        _equippedSlotControl.Hide();
        _itemControl.Hide();
        _mapControl.Hide();
        _mapZoneControl.Hide();
        _npcControl.Hide();
        _playerClassControl.Hide();
        _playerGenderControl.Hide();
        _playerGuildControl.Hide();
        _playerPowerControl.Hide();
        _playerSpellControl.Hide();
        _playerStatControl.Hide();
        _questCanStartControl.Hide();
        _questCompletedControl.Hide();
        _questInProgressControl.Hide();
        _selfSwitchControl.Hide();
        _timeBetweenControl.Hide();
        _variableControl.Hide();

        switch (type)
        {
            case ConditionType.IsItemEquipped:
                _equippedItemControl.Show();
                break;

            case ConditionType.CheckEquipment:
                _equippedSlotControl.Show();
                break;

            case ConditionType.HasItem:
                _itemControl.ShowHasItem();
                break;

            case ConditionType.HasFreeInventorySlots:
                _itemControl.ShowHasFreeInventorySlots();
                break;

            case ConditionType.MapIs:
                _mapControl.Show();
                break;

            case ConditionType.MapZoneTypeIs:
                _mapZoneControl.Show();
                break;

            case ConditionType.NoNpcsOnMap:
                _npcControl.Show();
                break;

            case ConditionType.ClassIs:
                _playerClassControl.Show();
                break;

            case ConditionType.GenderIs:
                _playerGenderControl.Show();
                break;

            case ConditionType.InGuildWithRank:
                _playerGuildControl.Show();
                break;

            case ConditionType.AccessIs:
                _playerPowerControl.Show();
                break;

            case ConditionType.KnowsSpell:
                _playerSpellControl.Show();
                break;

            case ConditionType.LevelOrStat:
                _playerStatControl.Show();
                break;

            case ConditionType.CanStartQuest:
                _questCanStartControl.Show();
                break;

            case ConditionType.QuestCompleted:
                _questCompletedControl.Show();
                break;

            case ConditionType.QuestInProgress:
                _questInProgressControl.Show();
                break;

            case ConditionType.SelfSwitch:
                _selfSwitchControl.Show();
                break;

            case ConditionType.TimeBetween:
                _timeBetweenControl.Show();
                break;

            case ConditionType.VariableIs:
                _variableControl.Show();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetupFormValues(dynamic condition)
    {
        switch (condition)
        {
            case IsItemEquippedCondition equippedItemCondition:
                _equippedItemControl.SetupFormValues(equippedItemCondition);
                break;

            case CheckEquippedSlot equippedSlotCondition:
                _equippedSlotControl.SetupFormValues(equippedSlotCondition);
                break;

            case HasItemCondition hasItemCondition:
                _itemControl.SetupFormValues(hasItemCondition);
                break;

            case HasFreeInventorySlots hasFreeInventorySlotsCondition:
                _itemControl.SetupFormValues(hasFreeInventorySlotsCondition);
                break;

            case MapIsCondition mapCondition:
                _mapControl.SetupFormValues(mapCondition);
                break;

            case MapZoneTypeIs mapZoneTypeCondition:
                _mapZoneControl.SetupFormValues(mapZoneTypeCondition);
                break;

            case NoNpcsOnMapCondition npcsOnMapCondition:
                _npcControl.SetupFormValues(npcsOnMapCondition);
                break;

            case ClassIsCondition playerClassCondition:
                _playerClassControl.SetupFormValues(playerClassCondition);
                break;

            case GenderIsCondition playerGenderCondition:
                _playerGenderControl.SetupFormValues(playerGenderCondition);
                break;

            case InGuildWithRank playerGuildCondition:
                _playerGuildControl.SetupFormValues(playerGuildCondition);
                break;

            case AccessIsCondition playerPowerCondition:
                _playerPowerControl.SetupFormValues(playerPowerCondition);
                break;

            case KnowsSpellCondition playerSpellCondition:
                _playerSpellControl.SetupFormValues(playerSpellCondition);
                break;

            case LevelOrStatCondition playerStatCondition:
                _playerStatControl.SetupFormValues(playerStatCondition);
                break;

            case CanStartQuestCondition questCanStartCondition:
                _questCanStartControl.SetupFormValues(questCanStartCondition);
                break;

            case QuestCompletedCondition questCompletedCondition:
                _questCompletedControl.SetupFormValues(questCompletedCondition);
                break;

            case QuestInProgressCondition questInProgressCondition:
                _questInProgressControl.SetupFormValues(questInProgressCondition);
                break;

            case SelfSwitchCondition selfSwitchCondition:
                _selfSwitchControl.SetupFormValues(selfSwitchCondition);
                break;

            case TimeBetweenCondition timeBetweenCondition:
                _timeBetweenControl.SetupFormValues(timeBetweenCondition);
                break;

            case VariableIsCondition variableIsCondition:
                _variableControl.SetupFormValues(variableIsCondition);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SaveFormValues(dynamic condition)
    {
        switch (condition)
        {
            case IsItemEquippedCondition equippedItemCondition:
                _equippedItemControl.SaveFormValues(equippedItemCondition);
                break;

            case CheckEquippedSlot equippedSlotCondition:
                _equippedSlotControl.SaveFormValues(equippedSlotCondition);
                break;

            case HasItemCondition hasItemCondition:
                _itemControl.SaveFormValues(hasItemCondition);
                break;

            case HasFreeInventorySlots hasFreeInventorySlotsCondition:
                _itemControl.SaveFormValues(hasFreeInventorySlotsCondition);
                break;

            case MapIsCondition mapCondition:
                _mapControl.SaveFormValues(mapCondition);
                break;

            case MapZoneTypeIs mapZoneTypeCondition:
                _mapZoneControl.SaveFormValues(mapZoneTypeCondition);
                break;

            case NoNpcsOnMapCondition npcsOnMapCondition:
                _npcControl.SaveFormValues(npcsOnMapCondition);
                break;

            case ClassIsCondition playerClassCondition:
                _playerClassControl.SaveFormValues(playerClassCondition);
                break;

            case GenderIsCondition playerGenderCondition:
                _playerGenderControl.SaveFormValues(playerGenderCondition);
                break;

            case InGuildWithRank playerGuildCondition:
                _playerGuildControl.SaveFormValues(playerGuildCondition);
                break;

            case AccessIsCondition playerPowerCondition:
                _playerPowerControl.SaveFormValues(playerPowerCondition);
                break;

            case KnowsSpellCondition playerSpellCondition:
                _playerSpellControl.SaveFormValues(playerSpellCondition);
                break;

            case LevelOrStatCondition playerStatCondition:
                _playerStatControl.SaveFormValues(playerStatCondition);
                break;

            case CanStartQuestCondition questCanStartCondition:
                _questCanStartControl.SaveFormValues(questCanStartCondition);
                break;

            case QuestCompletedCondition questCompletedCondition:
                _questCompletedControl.SaveFormValues(questCompletedCondition);
                break;

            case QuestInProgressCondition questInProgressCondition:
                _questInProgressControl.SaveFormValues(questInProgressCondition);
                break;

            case SelfSwitchCondition selfSwitchCondition:
                _selfSwitchControl.SaveFormValues(selfSwitchCondition);
                break;

            case TimeBetweenCondition timeBetweenCondition:
                _timeBetweenControl.SaveFormValues(timeBetweenCondition);
                break;

            case VariableIsCondition variableIsCondition:
                _variableControl.SaveFormValues(variableIsCondition);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveFormValues(Condition);
        Condition.Negated = chkNegated.Checked;
        Condition.ElseEnabled = chkHasElse.Checked;

        if (_eventCommand != null)
        {
            _eventCommand.Condition = Condition;
        }

        if (_eventEditor != null)
        {
            _eventEditor.FinishCommandEdit();
        }
        else
        {
            ParentForm?.Close();
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        if (_currentPage != null)
        {
            _eventEditor.CancelCommandEdit();
        }

        Cancelled = true;
        ParentForm?.Close();
    }

    private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        var type = Strings.EventConditional.conditions.FirstOrDefault(x => x.Value == cmbConditionType.Text).Key;
        if (type < 4)
        {
            type = 0;
        }

        var conditionType = (ConditionType)type;
        UpdateFormElements(conditionType);

        if (conditionType != Condition.Type)
        {
            switch (conditionType)
            {
                case ConditionType.IsItemEquipped:
                    Condition = new IsItemEquippedCondition();
                    break;

                case ConditionType.CheckEquipment:
                    Condition = new CheckEquippedSlot();
                    break;

                case ConditionType.HasItem:
                    Condition = new HasItemCondition();
                    break;

                case ConditionType.HasFreeInventorySlots:
                    Condition = new HasFreeInventorySlots();
                    break;

                case ConditionType.MapIs:
                    Condition = new MapIsCondition();
                    break;

                case ConditionType.MapZoneTypeIs:
                    Condition = new MapZoneTypeIs();
                    break;

                case ConditionType.NoNpcsOnMap:
                    Condition = new NoNpcsOnMapCondition();
                    break;

                case ConditionType.ClassIs:
                    Condition = new ClassIsCondition();
                    break;

                case ConditionType.GenderIs:
                    Condition = new GenderIsCondition();
                    break;

                case ConditionType.InGuildWithRank:
                    Condition = new InGuildWithRank();
                    break;

                case ConditionType.AccessIs:
                    Condition = new AccessIsCondition();
                    break;

                case ConditionType.KnowsSpell:
                    Condition = new KnowsSpellCondition();
                    break;

                case ConditionType.LevelOrStat:
                    Condition = new LevelOrStatCondition();
                    break;

                case ConditionType.CanStartQuest:
                    Condition = new CanStartQuestCondition();
                    break;

                case ConditionType.QuestCompleted:
                    Condition = new QuestCompletedCondition();
                    break;

                case ConditionType.QuestInProgress:
                    Condition = new QuestInProgressCondition();
                    break;

                case ConditionType.SelfSwitch:
                    Condition = new SelfSwitchCondition();
                    break;


                case ConditionType.VariableIs:
                    Condition = new VariableIsCondition();
                    SetupFormValues(Condition);
                    break;

                case ConditionType.TimeBetween:
                    Condition = new TimeBetweenCondition();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        SetupFormValues(Condition);
    }
}
