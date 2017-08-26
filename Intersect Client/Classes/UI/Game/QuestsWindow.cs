using System;
using System.Collections.Generic;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class QuestsWindow
    {
        private Button _backButton;
        private ScrollControl _questDescArea;
        private RichLabel _questDescLabel;
        private Label _questDescTemplateLabel;
        private ListBox _questList;

        private Label _questStatus;

        //Controls
        private WindowControl _questsWindow;

        private Label _questTitle;
        private Button _quitButton;
        private QuestBase _selectedQuest;

        //Init
        public QuestsWindow(Canvas _gameCanvas)
        {
            _questsWindow = new WindowControl(_gameCanvas, Strings.Get("questlog", "title"), false, "QuestsWindow");
            _questsWindow.DisableResizing();

            _questList = new ListBox(_questsWindow, "QuestList");
            _questList.EnableScroll(false, true);

            _questTitle = new Label(_questsWindow, "QuestTitle");
            _questTitle.SetText("");

            _questStatus = new Label(_questsWindow, "QuestStatus");
            _questStatus.SetText("");

            _questDescArea = new ScrollControl(_questsWindow, "QuestDescription");

            _questDescTemplateLabel = new Label(_questsWindow, "QuestDescriptionTemplate");

            _questDescLabel = new RichLabel(_questDescArea);

            _backButton = new Button(_questsWindow, "BackButton");
            _backButton.Clicked += _backButton_Clicked;

            _quitButton = new Button(_questsWindow, "AbandonQuestButton");
            _quitButton.SetText(Strings.Get("questlog", "abandon"));
            _quitButton.Clicked += _quitButton_Clicked;
        }

        private void _quitButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_selectedQuest != null)
            {
                new InputBox(Strings.Get("questlog", "abandontitle", _selectedQuest.Name),
                    Strings.Get("questlog", "abandonprompt", _selectedQuest.Name), true, InputBox.InputType.YesNo,
                    AbandonQuest, null,
                    _selectedQuest.Index);
            }
        }

        void AbandonQuest(object sender, EventArgs e)
        {
            PacketSender.SendCancelQuest(((InputBox) sender).UserData);
        }

        private void _backButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _selectedQuest = null;
            UpdateSelectedQuest();
        }

        //Methods
        public void Update(bool shouldUpdateList)
        {
            if (shouldUpdateList)
            {
                UpdateQuestList();
                UpdateSelectedQuest();
            }
            if (_questsWindow.IsHidden)
            {
                return;
            }

            if (_selectedQuest != null)
            {
                if (Globals.Me.QuestProgress.ContainsKey(_selectedQuest.Index))
                {
                    if (Globals.Me.QuestProgress[_selectedQuest.Index].completed == 1 &&
                        Globals.Me.QuestProgress[_selectedQuest.Index].task == -1)
                    {
                        //Completed
                        if (_selectedQuest.LogAfterComplete == 0)
                        {
                            _selectedQuest = null;
                            UpdateSelectedQuest();
                        }
                        return;
                    }
                    else
                    {
                        if (Globals.Me.QuestProgress[_selectedQuest.Index].task == -1)
                        {
                            //Not Started
                            if (_selectedQuest.LogBeforeOffer == 0)
                            {
                                _selectedQuest = null;
                                UpdateSelectedQuest();
                            }
                        }
                        return;
                    }
                }
                if (_selectedQuest.LogBeforeOffer == 0)
                {
                    _selectedQuest = null;
                    UpdateSelectedQuest();
                }
            }
        }

        private void UpdateQuestList()
        {
            _questList.RemoveAllRows();
            if (Globals.Me != null)
            {
                var quests = QuestBase.Lookup.IndexValues;
                foreach (QuestBase quest in quests)
                {
                    if (quest != null)
                    {
                        if (Globals.Me.QuestProgress.ContainsKey(quest.Index))
                        {
                            if (Globals.Me.QuestProgress[quest.Index].task != -1)
                            {
                                AddQuestToList(quest.Name, Color.Yellow, quest.Index);
                            }
                            else
                            {
                                if (Globals.Me.QuestProgress[quest.Index].completed == 1)
                                {
                                    if (quest.LogAfterComplete == 1)
                                    {
                                        AddQuestToList(quest.Name, Color.Green, quest.Index);
                                    }
                                }
                                else
                                {
                                    if (quest.LogBeforeOffer == 1)
                                    {
                                        AddQuestToList(quest.Name, Color.Red, quest.Index);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (quest.LogBeforeOffer == 1)
                            {
                                AddQuestToList(quest.Name, Color.Red, quest.Index);
                            }
                        }
                    }
                }
            }
        }

        private void AddQuestToList(string name, Color clr, int questId)
        {
            var item = _questList.AddRow(name);
            item.UserData = questId;
            item.Clicked += QuestListItem_Clicked;
            item.Selected += Item_Selected;
            item.SetTextColor(clr);
        }

        private void Item_Selected(Base sender, ItemSelectedEventArgs arguments)
        {
            _questList.UnselectAll();
        }

        private void QuestListItem_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var questNum = (int) ((ListBoxRow) sender).UserData;
            var quest = QuestBase.Lookup.Get<QuestBase>(questNum);
            if (quest != null)
            {
                _selectedQuest = quest;
                UpdateSelectedQuest();
            }
            _questList.UnselectAll();
        }

        private void UpdateSelectedQuest()
        {
            if (_selectedQuest == null)
            {
                _questList.Show();
                _questTitle.Hide();
                _questDescArea.Hide();
                _questStatus.Hide();
                _backButton.Hide();
                _quitButton.Hide();
            }
            else
            {
                _questDescLabel.ClearText();
                ListBoxRow rw;
                string[] myText = null;
                List<string> taskString = new List<string>();
                if (Globals.Me.QuestProgress.ContainsKey(_selectedQuest.Index))
                {
                    if (Globals.Me.QuestProgress[_selectedQuest.Index].task != -1)
                    {
                        //In Progress
                        _questStatus.SetText(Strings.Get("questlog", "inprogress"));
                        _questStatus.SetTextColor(Color.Yellow, Label.ControlState.Normal);
                        if (_selectedQuest.InProgressDesc.Length > 0)
                        {
                            _questDescLabel.AddText(_selectedQuest.InProgressDesc, Color.White, Alignments.Left,
                                _questDescTemplateLabel.Font);
                            _questDescLabel.AddLineBreak();
                            _questDescLabel.AddLineBreak();
                        }
                        _questDescLabel.AddText(Strings.Get("questlog", "currenttask"), Color.White, Alignments.Left,
                            _questDescTemplateLabel.Font);
                        _questDescLabel.AddLineBreak();
                        for (int i = 0; i < _selectedQuest.Tasks.Count; i++)
                        {
                            if (_selectedQuest.Tasks[i].Id == Globals.Me.QuestProgress[_selectedQuest.Index].task)
                            {
                                if (_selectedQuest.Tasks[i].Desc.Length > 0)
                                {
                                    _questDescLabel.AddText(_selectedQuest.Tasks[i].Desc, Color.White, Alignments.Left,
                                        _questDescTemplateLabel.Font);
                                    _questDescLabel.AddLineBreak();
                                    _questDescLabel.AddLineBreak();
                                }
                                if (_selectedQuest.Tasks[i].Objective == 1) //Gather Items
                                {
                                    _questDescLabel.AddText(Strings.Get("questlog", "taskitem",
                                            Globals.Me.QuestProgress[_selectedQuest.Index].taskProgress,
                                            _selectedQuest.Tasks[i].Data2,
                                            ItemBase.GetName(_selectedQuest.Tasks[i].Data1)),
                                        Color.White, Alignments.Left, _questDescTemplateLabel.Font);
                                }
                                else if (_selectedQuest.Tasks[i].Objective == 2) //Kill Npcs
                                {
                                    _questDescLabel.AddText(Strings.Get("questlog", "tasknpc",
                                            Globals.Me.QuestProgress[_selectedQuest.Index].taskProgress,
                                            _selectedQuest.Tasks[i].Data2,
                                            NpcBase.GetName(_selectedQuest.Tasks[i].Data1)),
                                        Color.White, Alignments.Left, _questDescTemplateLabel.Font);
                                }
                            }
                        }
                        if (_selectedQuest.Quitable == 1)
                        {
                            _quitButton.Show();
                        }
                    }
                    else
                    {
                        if (Globals.Me.QuestProgress[_selectedQuest.Index].completed == 1)
                        {
                            //Completed
                            if (_selectedQuest.LogAfterComplete == 1)
                            {
                                _questStatus.SetText(Strings.Get("questlog", "completed"));
                                _questStatus.SetTextColor(Color.Green, Label.ControlState.Normal);
                                _questDescLabel.AddText(_selectedQuest.EndDesc, Color.White, Alignments.Left,
                                    _questDescTemplateLabel.Font);
                            }
                        }
                        else
                        {
                            //Not Started
                            if (_selectedQuest.LogBeforeOffer == 1)
                            {
                                _questStatus.SetText(Strings.Get("questlog", "notstarted"));
                                _questStatus.SetTextColor(Color.Red, Label.ControlState.Normal);
                                _questDescLabel.AddText(_selectedQuest.BeforeDesc, Color.White, Alignments.Left,
                                    _questDescTemplateLabel.Font);
                            }
                        }
                    }
                }
                else
                {
                    //Not Started
                    if (_selectedQuest.LogBeforeOffer == 1)
                    {
                        _questStatus.SetText(Strings.Get("questlog", "notstarted"));
                        _questStatus.SetTextColor(Color.Red, Label.ControlState.Normal);
                        _questDescLabel.AddText(_selectedQuest.BeforeDesc, Color.White, Alignments.Left,
                            _questDescTemplateLabel.Font);
                    }
                }
                _questList.Hide();
                _questTitle.IsHidden = false;
                _questTitle.Text = _selectedQuest.Name;
                _questDescArea.IsHidden = false;
                _questDescLabel.Width = _questDescArea.Width - _questDescArea.GetVerticalScrollBar().Width;
                _questDescLabel.SizeToChildren(false, true);
                _questStatus.Show();
                _backButton.Show();
            }
        }

        public void Show()
        {
            _questsWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !_questsWindow.IsHidden;
        }

        public void Hide()
        {
            _questsWindow.IsHidden = true;
            _selectedQuest = null;
        }
    }
}